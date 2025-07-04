using E_Government.Application.DTO.Bills;
using E_Government.Application.DTO.CivilDocs;
using E_Government.Application.DTO.License;
using E_Government.Application.Exceptions;
using E_Government.Application.ServiceContracts;
using E_Government.Application.ServiceContracts.Common.Contracts.Infrastructure;
using E_Government.Domain.Entities;
using E_Government.Domain.Entities.Bills;
using E_Government.Domain.Entities.Liscenses;
using E_Government.Domain.Helper.Hub;
using E_Government.Domain.RepositoryContracts.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace E_Government.Application.Services.License
{
    public class LicenseService : ILicenseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBillNumberGenerator _billNumber;
        private readonly ILogger<LicenseService> _logger;
        private readonly IHubContext<DashboardHub, IHubService> _hubContext;


        private readonly IPaymentService _paymentService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<DashboardHub> _hubContexts;

        public LicenseService(IUnitOfWork unitOfWork,IBillNumberGenerator billNumber ,ILogger<LicenseService> logger, IHubContext<DashboardHub, IHubService> hubContext, IPaymentService paymentService,UserManager<ApplicationUser> userManager , IHubContext<DashboardHub> hubContexts)
        {
            _unitOfWork = unitOfWork;
            _billNumber = billNumber;
            _logger = logger;
            _hubContext = hubContext;
            _paymentService = paymentService;
            _userManager = userManager;
            _hubContexts = hubContexts;
        }

        public async Task<Guid> SubmitRequest(LicenseRequestDto dto)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            // Validate required fields
            if (string.IsNullOrWhiteSpace(dto.LicenseType))
                throw new ArgumentException("Document type is required", nameof(dto.LicenseType));
           if (string.IsNullOrWhiteSpace(dto.ApplicantNID))
                throw new ArgumentException("Applicant NID is required", nameof(dto.ApplicantNID));
            if (string.IsNullOrWhiteSpace(dto.ServiceCode))
                throw new ArgumentException("Service Coe is required", nameof(dto.ServiceCode));

            if (string.IsNullOrEmpty(dto.UploadedDocumentUrl))
                throw new ArgumentException("Uploaded document URL is required", nameof(dto.UploadedDocumentUrl));

            var user = await _unitOfWork.GetRepository<ApplicationUser,string>().GetUserByNID(dto.ApplicantNID);

            if (user == null)
                throw new ArgumentException("User with the provided NID does not exist", nameof(dto.ApplicantNID));

            var servicePrice = await _unitOfWork.GetRepository<ServicePrice, int>().GetAllWithIncludeAsync(q => q.Where(s => s.ServiceCode==dto.ServiceCode &&s.IsActive));
            var price = servicePrice.FirstOrDefault();
            if (price == null)
                throw new ArgumentException("Service price not found for the provided service code", nameof(dto.ServiceCode));



            var requestId = Guid.NewGuid();
            var request = new LicenseRequest
            {
                Id = requestId,
                LicenseType = dto.LicenseType,
                ServiceCode = dto.ServiceCode,
                ApplicantNID = dto.ApplicantNID,
                ApplicantName = user.DisplayName!,
                RequestDate = DateTime.UtcNow,
                Status = "Pending",
                Notes = dto.Notes,
                UploadedDocumentUrl = dto.UploadedDocumentUrl,
                ExtraFieldsJson = dto.ExtraFields != null ?
                    System.Text.Json.JsonSerializer.Serialize(dto.ExtraFields) : "{}"
            };

            // Create the bill for the request
            var billNumber =_billNumber.Generate().Result;

            var bill = new Bill
            {
                UseNID=user.NID,
                NID = user.NID,
                BillNumber = billNumber,
                ServiceCode = dto.ServiceCode,
                Amount = price.Price,
                Description = $"License Fee - {dto.LicenseType}",
                IssueDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(30),
                Status = BillStatus.Pending,
                RequestId = requestId
            };

            request.Bill = bill;


            var history = new LicenseRequestHistory
            {
                Id = Guid.NewGuid(),
                Status = "Pending",
                Note = "Request Created",
                ChangedAt = DateTime.UtcNow,
                RequestId = requestId
            };

            // Save to database
            await _unitOfWork.GetRepository<LicenseRequest, Guid>().AddAsync(request);
            await _unitOfWork.GetRepository<LicenseRequestHistory, Guid>().AddAsync(history);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("License request {RequestId} created successfully for user {NID}", requestId, dto.ApplicantNID);

            return requestId;
        }



        public async Task<PaymentCodeDto> GeneratePaymentCode(LicenseRequest request)
        {
            if (request is null)
                throw new NotFoundException("Request not found" );

            // Generate a unique payment code
            var paymentCode = $"PAY-{DateTime.Now:yyyyMMdd}-{request.Id.ToString("N")[..8].ToUpper()}";

            var bill = request.Bill;
            if (bill is not null)
            {
                bill.PaymentId = paymentCode;
               _unitOfWork.GetRepository<Bill,int>().Update(bill);
                await _unitOfWork.CompleteAsync();
            }

            return new PaymentCodeDto { 
                PaymentCode=paymentCode,
                Amount=bill?.Amount??0,
                BillNumber=bill?.BillNumber??"",
                DueDate = bill?.DueDate ?? DateTime.UtcNow.AddDays(30),
                ServiceDescription = $"License Fee - {request.LicenseType}"
            };

        }

        public async Task NotifyUserForPayment(LicenseRequest request, PaymentCodeDto paymentCode)
        {
            // Send notification to user about payment
            await _hubContexts.Clients.User(request.ApplicantNID).SendAsync("PaymentCodeGenerated", new
            {
                RequestId = request.Id,
                PaymentCode = paymentCode.PaymentCode,
                Amount = paymentCode.Amount,
                DueDate = paymentCode.DueDate,
                Message = $"Your license request has been approved. Please use payment code {paymentCode.PaymentCode} to complete payment."
            });

            _logger.LogInformation("Payment notification sent to user {NID} for request {RequestId}",
                request.ApplicantNID, request.Id);
        }

        public async Task<StripePaymentDto> InitiateStripePayment(string paymentCode)
        {
            // تم تغيير البحث ليبحث عن PaymentId بدلاً من ServiceCode
            var bills = await _unitOfWork.GetRepository<Bill, int>().GetAllWithIncludeAsync(q => q.Where(b => b.PaymentId == paymentCode));
            var bill = bills.FirstOrDefault();

            if (bill is null)
                throw new NotFoundException("Payment code not found");

            if (bill.Status is not BillStatus.Pending)
                throw new InvalidOperationException("Bill is not in a payable state");

            var paymentRequest = new BillPaymentRequest
            {
                BillId = bill.Id,
                Amount = bill.Amount,
                PaymentCode = paymentCode
            };

            var paymentResult = await _paymentService.CreatePaymentIntent(paymentRequest);
            if (!paymentResult.Success)
                throw new Exception($"Failed to create payment intent: {paymentResult.ErrorMessage}");

            return new StripePaymentDto
            {
                PaymentIntentId = paymentResult.PaymentIntentId,
                Amount = paymentResult.Amount,
                ClientSecret = paymentResult.ClientSecret,
                BillNumber = paymentResult.BillNumber,
                PaymentCode = paymentCode
            };
        }
        public async Task<bool> CompletePayment(string paymentIntentId)
        {
            // Step 1: Find the bill using the Stripe payment ID
            var bills = await _unitOfWork
                .GetRepository<Bill, int>()
                .GetAllWithIncludeAsync(q => q.Where(b => b.StripePaymentId == paymentIntentId));

            var bill = bills.FirstOrDefault();

            if (bill is null)
                throw new NotFoundException($"Bill with Stripe Payment Id {paymentIntentId} is not found");

            // Step 2: Update the bill status
            bill.Status = BillStatus.Paid;
            bill.PaymentDate = DateTime.Now;

            _unitOfWork.GetRepository<Bill, int>().Update(bill);

            // Step 3: Update the license request status if it exists
            if (bill.RequestId is Guid requestId)
            {
                var request = await _unitOfWork
                    .GetRepository<LicenseRequest, Guid>()
                    .GetAsync(requestId);

                if (request is not null)
                {
                    await UpdateLicenseRequestStatusAsync(request.Id, "Paid", request.Notes);
                }
            }

            // Step 4: Save changes to the database
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Payment completed for bill {BillId}, request {RequestId}", bill.Id, bill.RequestId);

            return true;
        }

        public async Task<bool> UpdateLicenseRequestStatusAsync(Guid id, string newStatus, string? notes, bool sendSignalRNotification = true)
        {
            var request = await _unitOfWork.GetRepository<LicenseRequest, Guid>().GetAsync(id);

            if (request is null)
                throw new NotFoundException($"License request with Id {id} not found.");

             var user = await _unitOfWork.GetRepository<ApplicationUser, string>().GetUserByNID(request.ApplicantNID);
            if (user is null)
                throw new NotFoundException($"User with NID {request.ApplicantNID} not found.");

            string oldStatus = request.Status;
            request.Status = newStatus;
            request.LastUpdated = DateTime.UtcNow;

            _logger.LogInformation($"AdminService: License request {id} (Type: {request.LicenseType}) status updated from {oldStatus} to {newStatus}.");

            var history = new LicenseRequestHistory
            {
                ChangedAt = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                Note = notes ?? $"Status changed from {oldStatus} to {newStatus}",
                RequestId = id,
                Status = newStatus,
            };

            _unitOfWork.GetRepository<LicenseRequest, Guid>().Update(request);
            await _unitOfWork.GetRepository<LicenseRequestHistory, Guid>().AddAsync(history);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("License request {RequestId} status updated from {OldStatus} to {NewStatus}",
                id, oldStatus, newStatus);

            if (sendSignalRNotification)
            {
                var updatedRequestSummary = new RequestSummaryDto
                {
                    RequestId = id,
                    RequestType = request.LicenseType,
                    ApplicantName = user.DisplayName ?? request.ApplicantName,
                    ApplicantNID = request.ApplicantNID,
                    RequestDate = request.RequestDate,
                    Status = newStatus,
                    DetailsApiEndpoint = $"/api/admin/requests/license/{id}"
                };

                await _hubContext.Clients.Group("AdminGroup").ReceiveRequestUpdated(updatedRequestSummary);
                await _hubContext.Clients.Group("AdminGroup").SendAdminNotification(
                    $"License Request {id} status changed to {newStatus}.");
            }

            return true;
        }
        public async Task<PaymentCodeDto?> ApproveLicenseRequestAsync(Guid id, UpdateRequestStatusInputDto input)
        {
            var requests = await _unitOfWork.GetRepository<LicenseRequest, Guid>().GetAllWithIncludeAsync(q => q.Where(l => l.Id == id).Include(l => l.Bill));
            var  request= requests.FirstOrDefault();
            if (request is null)
                throw new NotFoundException($"License request with ID {id} not found.");


            if (request.Status != "Pending")
                throw new InvalidOperationException("Only pending requests can be approved.");

            // Update request status to approved
            await UpdateLicenseRequestStatusAsync(id, "Approved", input.Notes);

            // Generate payment code and notify user
            var paymentCode =await GeneratePaymentCode(request);

            await NotifyUserForPayment(request, paymentCode);

            _logger.LogInformation("License request {RequestId} approved and payment code generated", id);

            return paymentCode;
        }

        public async Task<bool> RejectLicenseRequestAsync(Guid id, UpdateRequestStatusInputDto input)
        {
            if (string.IsNullOrWhiteSpace(input.Notes)) return false;
            return await UpdateLicenseRequestStatusAsync(id, "Rejected", input.Notes);
        }

        public async Task<LicenseRequest?> GetRequestByIdAsync(Guid id)
        {
            var request =await _unitOfWork.GetRepository<LicenseRequest, Guid>().GetAsync(id);
            if (request is null)
                throw new NotFoundException($"License request with ID {id} not found.");

            return request;
 
        }
    }
}
