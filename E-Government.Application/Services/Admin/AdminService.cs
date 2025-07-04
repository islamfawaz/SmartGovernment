using E_Government.Application.DTO.AdminDashboard;
using E_Government.Application.DTO.CivilDocs;
using E_Government.Application.DTO.License;
using E_Government.Application.Exceptions;
using E_Government.Application.ServiceContracts;
using E_Government.Domain.Entities;
using E_Government.Domain.Entities.CivilDocs;
using E_Government.Domain.Entities.Liscenses;
using E_Government.Domain.Helper.Hub;
using E_Government.Domain.RepositoryContracts.Persistence;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace E_Government.Application.Services.Admin
{


    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminService> _logger;
        private readonly IHubContext<DashboardHub, IHubService> _dashboardHubContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<AdminService> logger,
            IHubContext<DashboardHub, IHubService> dashboardHubContext,
            UserManager<ApplicationUser> userManager,
            ILicenseService licenseService

            )
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dashboardHubContext = dashboardHubContext ?? throw new ArgumentNullException(nameof(dashboardHubContext));
            _userManager = userManager;
        }

        public async Task<DashboardStatisticsDto> GetDashboardStatisticsAsync()
        {
            _logger.LogInformation("AdminService: Fetching dashboard statistics.");
            var civilStatsRepo = _unitOfWork.GetRepository<CivilDocumentRequest, Guid>();
            var licenseStatsRepo=  _unitOfWork.GetRepository<LicenseRequest,Guid>();


            // --- Temporary Diagnostic Logging ---
            try
            {
                var allCivilRequests = await civilStatsRepo.GetAllAsync(); // Fetches all civil document requests
                var allLicenseRequests = await licenseStatsRepo.GetAllAsync();

                var distinctStatuses = allCivilRequests.Select(r => r.Status).Distinct().ToList();
                var distenctLicenseStatuses = allLicenseRequests.Select(r => r.Status).Distinct().ToList();
                _logger.LogInformation($"AdminService: Distinct CivilDocumentRequest Statuses found in DB: {string.Join(", ", distinctStatuses)}");
                foreach (var req in allCivilRequests)
                {
                    _logger.LogInformation($"AdminService: CivilDoc ID: {req.Id}, Status: '{req.Status}'");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AdminService: Error during diagnostic logging of CivilDocumentRequest statuses.");
            }
            // --- End Temporary Diagnostic Logging ---

            var civilStats = await civilStatsRepo.GetStatusCountsAsync();
            var licenseStats = await licenseStatsRepo.GetStatusCountsAsync();

           //var licenseAggregatedStats = new Dictionary<string, int> { { "Total", 0 }, { "Pending", 0 }, { "Approved", 0 }, { "Rejected", 0 } };

            //foreach (var licenseTypeEntry in LicenseEntityTypes.TypeNameMap)
            //{
            //    try
            //    {
            //        var repoMethod = typeof(IUnitOfWork).GetMethod("GetRepository").MakeGenericMethod(licenseTypeEntry.Value, typeof(int));
            //        dynamic repository = repoMethod.Invoke(_unitOfWork, null);
            //        var counts = await repository.GetStatusCountsAsync();

            //        licenseAggregatedStats["Total"] += counts.TryGetValue("Total", out int t) ? t : 0;
            //        licenseAggregatedStats["Pending"] += counts.TryGetValue("Pending", out int p) ? p : 0;
            //        licenseAggregatedStats["Approved"] += counts.TryGetValue("Approved", out int a) ? a : 0;
            //        licenseAggregatedStats["Rejected"] += counts.TryGetValue("Rejected", out int r) ? r : 0;
            //    }
            //    catch (Exception ex)
            //    {
            //        _logger.LogError(ex, $"Error counting {licenseTypeEntry.Key} requests for dashboard. Ensure entities have a 'Status' property and GetStatusCountsAsync is implemented in generic repository for int IDs.");
            //    }
            //}

            var appUserRepo = _unitOfWork.GetRepository<ApplicationUser, string>();
            var totalUsers = await appUserRepo.CountAsync();
            var activeUsers = await appUserRepo.CountAsync(u => u.LastLoginDate >= DateTime.UtcNow.AddDays(-30));

            var stats = new DashboardStatisticsDto
            {
                TotalCivilDocRequests = civilStats.TryGetValue("Total", out int tc) ? tc : 0,
                PendingCivilDocRequests = civilStats.TryGetValue("Pending", out int pc) ? pc : 0,
                ApprovedCivilDocRequests = civilStats.TryGetValue("Approved", out int ac) ? ac : 0,
                RejectedCivilDocRequests = civilStats.TryGetValue("Rejected", out int rc) ? rc : 0,
                TotalLicenseRequests = licenseStats.TryGetValue("Total", out int tl) ? tl : 0,
                PendingLicenseRequests = licenseStats.TryGetValue("Pending", out int pl) ? pl : 0,
                ApprovedLicenseRequests = licenseStats.TryGetValue("Approved", out int al) ? al : 0,
                RejectedLicenseRequests = licenseStats.TryGetValue("Rejected", out int rl) ? rl : 0,
                TotalUsers = (int)totalUsers,
                ActiveUsers = (int)activeUsers,
                OtherStats = new Dictionary<string, int>()
            };

            await _dashboardHubContext.Clients.Group("AdminGroup").ReceiveStatisticsUpdate(stats);
            return stats;
        }
        public async Task<PagedResult<RequestSummaryDto>> GetAllRequestsAsync(int pageNumber, int pageSize, string? statusFilter, string? typeFilter, string? searchTerm)
        {
            _logger.LogInformation($"AdminService: Fetching requests. Page: {pageNumber}, Size: {pageSize}, Status: '{statusFilter}', Type: '{typeFilter}', Search: '{searchTerm}'");
            var combinedRequests = new List<RequestSummaryDto>();

            // 1. Civil Document Requests
            if (string.IsNullOrEmpty(typeFilter) || typeFilter.Equals("CivilDocument", StringComparison.OrdinalIgnoreCase))
            {
                Expression<Func<CivilDocumentRequest, bool>> civilPredicate = PredicateBuilder.True<CivilDocumentRequest>();
                if (!string.IsNullOrEmpty(statusFilter))
                {
                    // Corrected to use ToLower() for case-insensitive comparison translatable by EF Core
                    string lowerStatusFilter = statusFilter.ToLower();
                    civilPredicate = civilPredicate.And(r => r.Status != null && r.Status.ToLower() == lowerStatusFilter);
                }
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    civilPredicate = civilPredicate.And(r =>
                        (r.ApplicantName != null && EF.Functions.Like(r.ApplicantName, $"%{searchTerm}%")) ||
                        (r.ApplicantNID != null && EF.Functions.Like(r.ApplicantNID, $"%{searchTerm}%")) ||
                        (r.DocumentType != null && EF.Functions.Like(r.DocumentType, $"%{searchTerm}%"))
                    );
                }
                var civilPagedResult = await _unitOfWork.GetRepository<CivilDocumentRequest, Guid>()
                    .GetPagedListAsync(pageNumber, pageSize, civilPredicate, q => q.OrderByDescending(r => r.CreatedAt));

                combinedRequests.AddRange(civilPagedResult.Items.Select(r => new RequestSummaryDto
                {
                    RequestId = r.Id,
                    RequestType = r.DocumentType,
                    ApplicantName = r.ApplicantName,
                    ApplicantNID = r.ApplicantNID,
                    RequestDate = r.CreatedAt,
                    Status = r.Status,
                    DetailsApiEndpoint = $"/api/admin/requests/civil/{r.Id}"
                }));
            }

            // 2. License Requests
            if (string.IsNullOrEmpty(typeFilter) || typeFilter.Equals("License", StringComparison.OrdinalIgnoreCase))
            {
                Expression<Func<LicenseRequest, bool>> licensePredicate = PredicateBuilder.True<LicenseRequest>();
                if (!string.IsNullOrEmpty(statusFilter))
                {
                    // Corrected to use ToLower() for case-insensitive comparison translatable by EF Core
                    string lowerStatusFilter = statusFilter.ToLower();
                    licensePredicate = licensePredicate.And(r => r.Status != null && r.Status.ToLower() == lowerStatusFilter);
                }
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    licensePredicate = licensePredicate.And(r =>
                        (r.ApplicantName != null && EF.Functions.Like(r.ApplicantName, $"%{searchTerm}%")) ||
                        (r.ApplicantNID != null && EF.Functions.Like(r.ApplicantNID, $"%{searchTerm}%")) ||
                        (r.LicenseType != null && EF.Functions.Like(r.LicenseType, $"%{searchTerm}%"))
                    );
                }
                var licensePagedResult = await _unitOfWork.GetRepository<LicenseRequest, Guid>()
                    .GetPagedListAsync(pageNumber, pageSize, licensePredicate, q => q.OrderByDescending(r => r.CreatedAt));

                combinedRequests.AddRange(licensePagedResult.Items.Select(r => new RequestSummaryDto
                {
                    RequestId = r.Id,
                    RequestType = r.LicenseType,
                    ApplicantName = r.ApplicantName,
                    ApplicantNID = r.ApplicantNID,
                    RequestDate = r.CreatedAt,
                    Status = r.Status,
                    DetailsApiEndpoint = $"/api/admin/requests/license/{r.Id}"
                }));
            }

            // Apply ordering and pagination to the combined results
            var totalCount = combinedRequests.Count;
            var orderedRequests = combinedRequests.OrderByDescending(r => r.RequestDate).ToList();

            // Apply pagination to the combined, filtered, and ordered list
            var pagedItems = orderedRequests.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new PagedResult<RequestSummaryDto>
            {
                Items = pagedItems,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }
        public async Task<CivilDocumentRequestDetailsDto> GetCivilDocumentRequestDetailsAsync(Guid id)
        {
            _logger.LogInformation($"AdminService: Fetching civil document request details for ID: {id}");
            var entity = await _unitOfWork.GetRepository<CivilDocumentRequest, Guid>()
                .GetByIdWithIncludeAsync(id, q => q.Include(r => r.Attachments).Include(r => r.History));

            if (entity == null) throw new NotFoundException($"CivilDocumentRequest with ID {id} not found.");
            return new CivilDocumentRequestDetailsDto
            {
                Id = entity.Id,
                ApplicantNID = entity.ApplicantNID,
                ApplicantName = entity.ApplicantName,
                DocumentType = entity.DocumentType,
                RequestDate = entity.CreatedAt,
                CurrentStatus = entity.Status,
                Attachments = _mapper.Map<List<CivilDocumentAttachmentDto>>(entity.Attachments),
                History = _mapper.Map<List<RequestHistoryDto>>(entity.History)
            };

        }
        private async Task<bool> UpdateCivilDocumentRequestStatusAsync(Guid requestId, string newStatus, string? notes, bool sendSignalRNotification = true)
        {
            var request = await _unitOfWork.GetRepository<CivilDocumentRequest, Guid>().GetAsync(requestId);
            if (request == null) return false;

            string oldStatus = request.Status;
            request.Status = newStatus;
            request.LastUpdated = DateTime.UtcNow;

            var civilHistory = new CivilDocumentRequestHistory
            {
                Id = Guid.NewGuid(),
                RequestId = requestId,
                Status = newStatus,
                Note = notes ?? $"Status changed from {oldStatus} to {newStatus}",
                ChangedAt = DateTime.UtcNow
            };
            await _unitOfWork.GetRepository<CivilDocumentRequestHistory, Guid>().AddAsync(civilHistory);

            await _unitOfWork.CompleteAsync();
            _logger.LogInformation($"AdminService: CivilDocumentRequest ID {requestId} status updated from {oldStatus} to {newStatus}.");

            if (sendSignalRNotification)
            {
                var updatedRequestSummary = _mapper.Map<RequestSummaryDto>(request);
                await _dashboardHubContext.Clients.Group("AdminGroup").ReceiveRequestUpdated(updatedRequestSummary);
                await _dashboardHubContext.Clients.Group("AdminGroup").SendAdminNotification($"Civil Document Request {request.Id} status changed to {newStatus}.");
            }
            return true;
        }
        public async Task<bool> ApproveCivilDocumentRequestAsync(Guid id, UpdateRequestStatusInputDto input)
        {
            return await UpdateCivilDocumentRequestStatusAsync(id, "Approved", input.Notes);
        }
        public async Task<bool> RejectCivilDocumentRequestAsync(Guid id, UpdateRequestStatusInputDto input)
        {
            if (string.IsNullOrWhiteSpace(input.Notes)) return false;
            return await UpdateCivilDocumentRequestStatusAsync(id, "Rejected", input.Notes);
        }

        public async Task<LicenseRequestDetailsDto> GetLicenseRequestDetailsAsync(Guid id)
        {
            var entity = await _unitOfWork.GetRepository<LicenseRequest, Guid>().GetByIdWithIncludeAsync(id, q => q.Include(r => r.LicenseRequestHistories));

            if (entity == null) throw new NotFoundException($"CivilDocumentRequest with ID {id} not found.");

            return new LicenseRequestDetailsDto
            {
                Id = entity.Id,
                ApplicantNID = entity.ApplicantNID,
                ApplicantName = entity.ApplicantName,
                LicenseType = entity.LicenseType,
                RequestDate = entity.CreatedAt,
                CurrentStatus = entity.Status,
                History = _mapper.Map<List<RequestHistoryDto>>(entity.LicenseRequestHistories)
            };
        }
      
    

    }

    public static class PredicateBuilder
    {
        public static Expression<Func<T, bool>> True<T>() { return f => true; }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
                                                            Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
                                                             Expression<Func<T, bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>
                  (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }
    }


}



