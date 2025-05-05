// E-Government.Application/Services/BillingService.cs
using E_Government.Core.Domain.Entities;
using E_Government.Core.Domain.RepositoryContracts.Infrastructure; // For IPaymentService
using E_Government.Core.Domain.RepositoryContracts.Persistence; // For IUnitOfWork
using E_Government.Core.Domain.Specification.Bills; // For CustomerWithMetersSpec, MeterReadingSpecs
// using E_Government.Core.Domain.Specification.MeterReadings; // Removed, MeterReadingSpecs is in Bills namespace
// using E_Government.Core.Domain.Specification.Users; // Removed, CustomerWithMetersSpec is in Bills namespace
using E_Government.Core.DTO;
using E_Government.Core.ServiceContracts; // For IBillingService, IBillNumberGenerator
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace E_Government.Application.Services
{
    public class BillingService : IBillingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBillNumberGenerator _billNumberGenerator;
        private readonly IPaymentService _paymentService;
        private readonly ILogger<BillingService> _logger;

        public BillingService(
            IUnitOfWork unitOfWork,
            IBillNumberGenerator billNumberGenerator,
            IPaymentService paymentService,
            ILogger<BillingService> logger
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
                _logger.LogInformation("Attempting to generate bill for NID: {NID}, Type: {Type}", request.NID, request.Type);
                // 1. Validate customer with proper error handling
                var userSpec = new CustomerWithMetersSpec(request.NID);
                var user = await _unitOfWork.GetRepository<ApplicationUser>()
                                .GetFirstOrDefaultWithSpecAsync(userSpec);

                if (user is null)
                {
                    _logger.LogWarning("Customer with NID {NID} not found.", request.NID);
                    return new BillPaymentResult
                    {
                        Success = false,
                        ErrorMessage = $"Customer with ID {request.NID} not found",
                    };
                }
                else
                {
                    _logger.LogInformation("Found customer {NID}", user.NID);
                }

                // 2. Validate user has confirmed email
                if (!user.EmailConfirmed)
                {
                    _logger.LogWarning("User {NID} email not confirmed.", user.NID);
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
                    string errorMsg = availableTypes.Any()
                            ? $"No active {request.Type} meter found for user {request.NID}. Available types: {string.Join(", ", availableTypes)}"
                            : $"Customer {request.NID} has no active registered meters";
                    _logger.LogWarning(errorMsg);
                    return new BillPaymentResult
                    {
                        Success = false,
                        ErrorMessage = errorMsg,
                    };
                }
                _logger.LogInformation("Found active meter {MeterId} of type {Type} for user {NID}", meter.Id, meter.Type, user.NID);

                // 4. Get latest reading with validation
                var readingSpec = new MeterReadingSpecs(meter.Id);
                var latestReading = await _unitOfWork.GetRepository<MeterReading>()
                                        .GetFirstOrDefaultWithSpecAsync(readingSpec);
                _logger.LogInformation("Latest reading for meter {MeterId} is {ReadingValue}", meter.Id, latestReading?.Value ?? 0);

                // 5. Validate reading values
                if (request.CurrentReading <= (latestReading?.Value ?? 0))
                {
                    string errorMsg = $"Current reading ({request.CurrentReading}) must be greater than previous reading ({latestReading?.Value ?? 0}) for meter {meter.Id}";
                    _logger.LogWarning(errorMsg);
                    return new BillPaymentResult
                    {
                        Success = false,
                        ErrorMessage = errorMsg,
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
                _logger.LogInformation("Created new bill {BillNumber} for meter {MeterId} with amount {Amount}", bill.BillNumber, bill.MeterId, bill.Amount);

                // 7. Save bill
                await _unitOfWork.GetRepository<Bill>().AddAsync(bill);
                await _unitOfWork.CompleteAsync();
                _logger.LogInformation("Saved bill {BillNumber} (ID: {BillId}) to database.", bill.BillNumber, bill.Id);

                // 8. Create payment intent with retry logic
                var paymentRequest = new BillPaymentRequest
                {
                    BillId = bill.Id,
                    Amount = bill.Amount,
                    UserEmail = user.Email!, // Null forgiveness operator used here, ensure user.Email is not null
                };
                _logger.LogInformation("Creating payment intent for bill {BillId}", bill.Id);
                var paymentResult = await _paymentService.CreatePaymentIntent(paymentRequest);
                if (!paymentResult.Success)
                {
                    _logger.LogError("Failed to create payment intent for bill {BillId}: {ErrorMessage}", bill.Id, paymentResult.ErrorMessage);
                    return new BillPaymentResult
                    {
                        Success = false,
                        ErrorMessage = paymentResult.ErrorMessage,
                    };
                }
                _logger.LogInformation("Payment intent {PaymentIntentId} created successfully for bill {BillId}", paymentResult.PaymentIntentId, bill.Id);

                // 9. Update bill with payment info
                bill.StripePaymentId = paymentResult.PaymentIntentId;
                _unitOfWork.GetRepository<Bill>().Update(bill);
                await _unitOfWork.CompleteAsync();
                _logger.LogInformation("Updated bill {BillId} with Stripe Payment Intent ID {PaymentIntentId}", bill.Id, bill.StripePaymentId);

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
                _logger.LogError(dbEx, "Foreign key violation while generating bill for NID {NID}", request.NID);
                return new BillPaymentResult
                {
                    Success = false,
                    ErrorMessage = "Database relationship error. Please verify all references."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in bill generation and payment for NID {NID}", request.NID);
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
            // Consider moving pricing logic to configuration or a dedicated service
            return type switch
            {
                MeterType.Electricity => category == CustomerCategory.Residential ? 0.5m : 0.8m,
                MeterType.Water => category == CustomerCategory.Residential ? 0.3m : 0.6m,
                MeterType.Gas => category == CustomerCategory.Residential ? 0.4m : 0.7m,
                _ => 0.5m // Default price?
            };
        }



        public async Task<MeterRegistrationResult> RegisterMeter(RegisterMeterDto request)
        {
            try
            {
                _logger.LogInformation("Attempting to register meter for NID: {NID}, Type: {Type}", request.NID, request.Type);
                // Validate User exists
                var user = await _unitOfWork.GetRepository<ApplicationUser>().GetUserByNID(request.NID);

                if (user is null)
                {
                    _logger.LogWarning("Customer with NID {NID} not found during meter registration.", request.NID);
                    return new MeterRegistrationResult
                    {
                        Success = false,
                        ErrorMessage = "Customer not found",
                        ErrorCode = "CUSTOMER_NOT_FOUND"
                    };
                }

                // Check if customer already has a meter of this type
                // Consider a more efficient query if possible (e.g., using Specification pattern)
                var existingMeters = await _unitOfWork.GetRepository<Meter>()
                    .GetAllAsync(); // Get all meters first

                var existingMeter = existingMeters
                    .FirstOrDefault(m => m.UserNID == request.NID && m.Type == request.Type);

                if (existingMeter != null)
                {
                    _logger.LogWarning("Customer {NID} already has a {Type} meter registered (Meter ID: {MeterId}).", request.NID, request.Type, existingMeter.Id);
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
                    InstallationDate = DateTime.Now, // Consider UtcNow
                    IsActive = true
                };
                _logger.LogInformation("Creating new meter {MeterNumber} of type {Type} for user {NID}", meter.MeterNumber, meter.Type, meter.UserNID);

                await _unitOfWork.GetRepository<Meter>().AddAsync(meter);
                await _unitOfWork.CompleteAsync();
                _logger.LogInformation("Successfully registered meter {MeterNumber} (ID: {MeterId}) for user {NID}", meter.MeterNumber, meter.Id, meter.UserNID);

                return new MeterRegistrationResult
                {
                    Success = true,
                    Meter = meter
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while registering meter for NID {NID}", request.NID);
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
            // Consider a more robust generation strategy
            return $"MTR-{type.ToString().ToUpper()}-{DateTime.Now:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";

        }

    }
}

