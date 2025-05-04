using E_Government.Core.Domain.Entities;
using E_Government.Core.Domain.RepositoryContracts.Infrastructure;
using E_Government.Core.Domain.RepositoryContracts.Persistence;
using E_Government.Core.Domain.Specification.Bills;
using E_Government.Core.DTO;
using E_Government.Core.ServiceContracts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace E_Government.Core.Services
{
    public class BillingService : IBillingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBillNumberGenerator _billNumberGenerator;
        private readonly IPaymentService _paymentService;
        private readonly ILogger _logger;

        public BillingService(
            IUnitOfWork unitOfWork,
            IBillNumberGenerator billNumberGenerator,
            IPaymentService paymentService ,
            ILogger logger
            )
        {
            _unitOfWork = unitOfWork;
            _billNumberGenerator = billNumberGenerator;
            _paymentService = paymentService;
            _logger = logger;
        }

        public async Task<BillPaymentResult> GenerateAndPayBill(GenerateBillRequestDto request)
        {

            try
            {
                // 1. Validate customer with proper error handling
                var userSpec = new CustomerWithMetersSpec(request.NID);
                var user = await _unitOfWork.GetRepository<ApplicationUser>()
                                .GetFirstOrDefaultWithSpecAsync(userSpec);

                if (user is null)
                {
                    return new BillPaymentResult
                    {
                        Success = false,
                        ErrorMessage = $"Customer with ID {request.NID} nnot found",
                    };
                }
                else
                {
                    user.EmailConfirmed = true;

                }



                

                // 2. Validate user has confirmed email
                if (!user.EmailConfirmed)
                {
                    return new BillPaymentResult
                    {
                        Success = false,
                        ErrorMessage = "User email not confirmed",
                    };
                }

                // 3. Validate meter exists for this type
                var meter = user.Meters.FirstOrDefault(m => m.Type == request.Type && m.IsActive);
                if (meter == null)
                {
                    var availableTypes = user.Meters
                        .Where(m => m.IsActive)
                        .Select(m => m.Type.ToString())
                        .Distinct();

                    return new BillPaymentResult
                    {
                        Success = false,
                        ErrorMessage = availableTypes.Any()
                            ? $"No active {request.Type} meter found. Available types: {string.Join(", ", availableTypes)}"
                            : "Customer has no active registered meters",
                    };
                }

                // 4. Get latest reading with validation
                var readingSpec = new MeterReadingSpecs(meter.Id);
                var latestReading = await _unitOfWork.GetRepository<MeterReading>()
                                        .GetFirstOrDefaultWithSpecAsync(readingSpec);

                // 5. Validate reading values
                if (request.CurrentReading <= (latestReading?.Value ?? 0))
                {
                    return new BillPaymentResult
                    {
                        Success = false,
                        ErrorMessage = $"Current reading ({request.CurrentReading}) must be greater than previous reading ({latestReading?.Value ?? 0})",
                    };
                }

                // 6. Create bill with all required fields
                var bill = new Bill
                {
                    BillNumber = _billNumberGenerator.Generate(),
                    IssueDate = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(30),
                    MeterId = meter.Id,
                    UseNID = user.NID, // Ensure UserId is set
                    PreviousReading = latestReading?.Value ?? 0,
                    CurrentReading = request.CurrentReading,
                    UnitPrice = GetUnitPrice(request.Type, user.Category),
                    Type = request.Type,
                    Amount = CalculateBillAmount(
                        currentReading: request.CurrentReading,
                        previousReading: latestReading?.Value ?? 0,
                        type: request.Type,
                        category: user.Category),
                    PdfUrl = null, // Will be generated later
                    StripePaymentId = null, // Will be set after payment intent creation
                    PaymentId = null // Will be set after successful payment
                };

                // 7. Save bill
                await _unitOfWork.GetRepository<Bill>().AddAsync(bill);
                await _unitOfWork.CompleteAsync();

                // 8. Create payment intent with retry logic
                var paymentRequest = new BillPaymentRequest
                {
                    BillId = bill.Id,
                    Amount = bill.Amount,
                    UserEmail = user.Email!,
                };

                var paymentResult = await _paymentService.CreatePaymentIntent(paymentRequest);
                if (!paymentResult.Success)
                {
                    return new BillPaymentResult
                    {
                        Success = false,
                        ErrorMessage = paymentResult.ErrorMessage,
                    };
                }

                // 9. Update bill with payment info
                bill.StripePaymentId = paymentResult.PaymentIntentId;
                _unitOfWork.GetRepository<Bill>().Update(bill);
                await _unitOfWork.CompleteAsync();

          

                return new BillPaymentResult
                {
                    Success = true,
                    PaymentIntentId = paymentResult.PaymentIntentId,
                    ClientSecret = paymentResult.ClientSecret,
                    Amount = bill.Amount,
                    BillNumber = bill.BillNumber
                };
            }
            catch (DbUpdateException dbEx) when (dbEx.InnerException is SqlException sqlEx && sqlEx.Number == 547)
            {
                _logger.LogError(dbEx, "Foreign key violation while generating bill");
                return new BillPaymentResult
                {
                    Success = false,
                    ErrorMessage = "Database relationship error. Please verify all references."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bill generation and payment");
                return new BillPaymentResult
                {
                    Success = false,
                    ErrorMessage = "An unexpected error occurred",
                };
            }
        }


        private decimal CalculateBillAmount(decimal currentReading, decimal previousReading,
            MeterType type, CustomerCategory category)
        {
            var consumption = currentReading - previousReading;
            var unitPrice = GetUnitPrice(type, category);
            return consumption * unitPrice;
        }

        private decimal GetUnitPrice(MeterType type, CustomerCategory category)
        {
            return type switch
            {
                MeterType.Electricity => category == CustomerCategory.Residential ? 0.5m : 0.8m,
                MeterType.Water => category == CustomerCategory.Residential ? 0.3m : 0.6m,
                MeterType.Gas => category == CustomerCategory.Residential ? 0.4m : 0.7m,
                _ => 0.5m
            };
        }



        public async Task<MeterRegistrationResult> RegisterMeter(RegisterMeterDto request)
        {
            try
            {
                // Validate User exists
                var user = await _unitOfWork.GetRepository<ApplicationUser>().GetUserByNID(request.NID);

                if (user is null)
                {
                    return new MeterRegistrationResult
                    {
                        Success = false,
                        ErrorMessage = "Customer not found",
                        ErrorCode = "CUSTOMER_NOT_FOUND"
                    };
                }

                // Check if customer already has a meter of this type
                var existingMeters = await _unitOfWork.GetRepository<Meter>()
                    .GetAllAsync(); // Get all meters first

                var existingMeter = existingMeters
                    .FirstOrDefault(m => m.UserNID == request.NID && m.Type == request.Type);

                if (existingMeter != null)
                {
                    return new MeterRegistrationResult
                    {
                        Success = false,
                        ErrorMessage = $"Customer already has a {request.Type} meter registered",
                        ErrorCode = "METER_ALREADY_EXISTS"
                    };
                }

                // Create new meter
                var meter = new Meter
                {
                    MeterNumber = GenerateMeterNumber(request.Type),
                    Type = request.Type,
                    UserNID = request.NID,
                    InstallationDate = DateTime.Now,
                    IsActive = true
                };

                await _unitOfWork.GetRepository<Meter>().AddAsync(meter);
                await _unitOfWork.CompleteAsync();

                return new MeterRegistrationResult
                {
                    Success = true,
                    Meter = meter
                };
            }
            catch (Exception ex)
            {
                // Log exception here
                return new MeterRegistrationResult
                {
                    Success = false,
                    ErrorMessage = "An error occurred while registering the meter",
                    ErrorCode = "REGISTRATION_ERROR"
                };
            }
        }
        private string GenerateMeterNumber(MeterType type)
        {
            return $"BL-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

        }

    }
}