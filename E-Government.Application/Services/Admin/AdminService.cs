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
        private readonly ILicenseRepositoryFactory _licenseRepositoryFactory;

        public AdminService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<AdminService> logger,
            IHubContext<DashboardHub, IHubService> dashboardHubContext,
            UserManager<ApplicationUser> userManager,
            ILicenseRepositoryFactory licenseRepositoryFactory

            )
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dashboardHubContext = dashboardHubContext ?? throw new ArgumentNullException(nameof(dashboardHubContext));
            _userManager = userManager;
            _licenseRepositoryFactory = licenseRepositoryFactory;
        }

        public async Task<DashboardStatisticsDto> GetDashboardStatisticsAsync()
        {
            _logger.LogInformation("AdminService: Fetching dashboard statistics.");
            var civilStatsRepo = _unitOfWork.GetRepository<CivilDocumentRequest, Guid>();

            // --- Temporary Diagnostic Logging ---
            try
            {
                var allCivilRequests = await civilStatsRepo.GetAllAsync(); // Fetches all civil document requests
                var distinctStatuses = allCivilRequests.Select(r => r.Status).Distinct().ToList();
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

            var licenseAggregatedStats = new Dictionary<string, int> { { "Total", 0 }, { "Pending", 0 }, { "Approved", 0 }, { "Rejected", 0 } };

            foreach (var licenseTypeEntry in LicenseEntityTypes.TypeNameMap)
            {
                try
                {
                    var repoMethod = typeof(IUnitOfWork).GetMethod("GetRepository").MakeGenericMethod(licenseTypeEntry.Value, typeof(int));
                    dynamic repository = repoMethod.Invoke(_unitOfWork, null);
                    var counts = await repository.GetStatusCountsAsync();

                    licenseAggregatedStats["Total"] += counts.TryGetValue("Total", out int t) ? t : 0;
                    licenseAggregatedStats["Pending"] += counts.TryGetValue("Pending", out int p) ? p : 0;
                    licenseAggregatedStats["Approved"] += counts.TryGetValue("Approved", out int a) ? a : 0;
                    licenseAggregatedStats["Rejected"] += counts.TryGetValue("Rejected", out int r) ? r : 0;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error counting {licenseTypeEntry.Key} requests for dashboard. Ensure entities have a 'Status' property and GetStatusCountsAsync is implemented in generic repository for int IDs.");
                }
            }

            var appUserRepo = _unitOfWork.GetRepository<ApplicationUser, string>();
            var totalUsers = await appUserRepo.CountAsync();
            var activeUsers = await appUserRepo.CountAsync(u => u.LastLoginDate >= DateTime.UtcNow.AddDays(-30));

            var stats = new DashboardStatisticsDto
            {
                TotalCivilDocRequests = civilStats.TryGetValue("Total", out int tc) ? tc : 0,
                PendingCivilDocRequests = civilStats.TryGetValue("Pending", out int pc) ? pc : 0,
                ApprovedCivilDocRequests = civilStats.TryGetValue("Approved", out int ac) ? ac : 0,
                RejectedCivilDocRequests = civilStats.TryGetValue("Rejected", out int rc) ? rc : 0,
                TotalLicenseRequests = licenseAggregatedStats["Total"],
                PendingLicenseRequests = licenseAggregatedStats["Pending"],
                ApprovedLicenseRequests = licenseAggregatedStats["Approved"],
                RejectedLicenseRequests = licenseAggregatedStats["Rejected"],
                TotalUsers = (int)totalUsers,
                ActiveUsers = (int)activeUsers,
                OtherStats = new Dictionary<string, int>()
            };

            await _dashboardHubContext.Clients.Group("AdminGroup").ReceiveStatisticsUpdate(stats);
            return stats;
        }
        public async Task<PagedResult<RequestSummaryDto>> GetAllRequestsAsync(int pageNumber, int pageSize, string? statusFilter, string? typeFilter, string? searchTerm)
        {
            _logger.LogInformation($"AdminService: Fetching requests. Page: {pageNumber}, Size: {pageSize}, Status: 	'{statusFilter}'	, Type: 	'{typeFilter}'	, Search: 	'{searchTerm}'	");
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
                    RequestType = "CivilDocument",
                    ApplicantName = r.ApplicantName,
                    ApplicantNID = r.ApplicantNID,
                    RequestDate = r.CreatedAt,
                    Status = r.Status,
                    DetailsApiEndpoint = $"/api/admin/requests/civil/{r.Id}"
                }));
            }

            // 2. License Requests (This part is filtered in-memory after fetching all, so StringComparison.OrdinalIgnoreCase should work here)
            foreach (var licenseTypeEntry in LicenseEntityTypes.TypeNameMap)
            {
                string licenseTypeName = licenseTypeEntry.Key;
                Type entityType = licenseTypeEntry.Value;

                if (string.IsNullOrEmpty(typeFilter) ||
                    typeFilter.Equals(licenseTypeName, StringComparison.OrdinalIgnoreCase) ||
                    (typeFilter.Equals("License", StringComparison.OrdinalIgnoreCase) && LicenseEntityTypes.TypeNameMap.ContainsKey(licenseTypeName)))
                {
                    try
                    {
                        var repo =_licenseRepositoryFactory.GetRepository(licenseTypeName);
                        var licenses =await repo.GetAllAsync();

                      //  var repoMethod = typeof(IUnitOfWork).GetMethod("GetRepository").MakeGenericMethod(entityType, typeof(int));
                       // dynamic repository = repoMethod.Invoke(_unitOfWork, null);
                       // IEnumerable<dynamic> licenses = await repository.GetAllAsync();

                        foreach (var lic in licenses)
                        {
                            bool matches = true;
                            if (!string.IsNullOrEmpty(statusFilter) && lic.Status != null && !((string)lic.Status).Equals(statusFilter, StringComparison.OrdinalIgnoreCase))
                            {
                                matches = false;
                            }

                            if (!string.IsNullOrEmpty(searchTerm))
                            {
                                bool nameMatch = lic.Applicant.DisplayName != null && ((string)lic.Applicant.DisplayName).Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
                                bool nidMatch = lic.ApplicantNID != null && ((string)lic.ApplicantNID).ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
                                if (!nameMatch && !nidMatch) matches = false;
                            }

                            if (matches)
                            {
                                combinedRequests.Add(new RequestSummaryDto
                                {
                                    RequestId = lic.PublicId,
                                    RequestType = licenseTypeName,
                                    ApplicantName = lic.Applicant.DisplayName!,
                                    ApplicantNID = lic.ApplicantNID!.ToString(),
                                    RequestDate = lic.RequestDate,
                                    Status = lic.Status!,
                                    DetailsApiEndpoint = $"/api/admin/requests/license/{lic.PublicId}"
                                });
                            }
                        }
                    }
                     catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error fetching or processing {licenseTypeName} requests. Ensure entities have PublicId, ApplicantName, ApplicantNID, RequestDate, Status properties.");
                    }
                }
            }

            var totalCount = combinedRequests.Count;
            var orderedRequests = combinedRequests.OrderByDescending(r => r.RequestDate).ToList(); // Order combined list

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
            return _mapper.Map<CivilDocumentRequestDetailsDto>(entity);
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
                Note = notes ?? string.Empty,
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

        private async Task<(dynamic? entity, dynamic? repository, string entityTypeName, int entityIntId)>
            GetLicenseEntityAndRepoByPublicIdAsync(Guid publicId, string operationName)
        {
            _logger.LogInformation($"AdminService ({operationName}): Locating license entity with PublicID: {publicId}");
            foreach (var licenseTypeEntry in LicenseEntityTypes.TypeNameMap)
            {
                Type concreteEntityType = licenseTypeEntry.Value;
                var repoMethod = typeof(IUnitOfWork).GetMethod("GetRepository").MakeGenericMethod(concreteEntityType, typeof(int));
                dynamic repository = repoMethod.Invoke(_unitOfWork, null);

                IEnumerable<dynamic> allEntities = await repository.GetAllAsync();
                var entity = allEntities.FirstOrDefault(e => e.PublicId == publicId);

                if (entity != null)
                {
                    _logger.LogInformation($"Found entity of type {licenseTypeEntry.Key} with PublicID {publicId}");
                    return (entity, repository, licenseTypeEntry.Key, (int)entity.Id);
                }
            }
            _logger.LogWarning($"AdminService ({operationName}): No license entity found with PublicID {publicId}.");
            throw new NotFoundException($"License request with PublicID {publicId} not found.");
        }

        public async Task<LicenseRequestDetailsDto> GetLicenseRequestDetailsAsync(Guid publicId)
        {
            var (entity, _, entityTypeName, _) = await GetLicenseEntityAndRepoByPublicIdAsync(publicId, nameof(GetLicenseRequestDetailsAsync));

            var detailsDto = new LicenseRequestDetailsDto
            {
                Id = publicId,
                ApplicantNID = entity.ApplicantNID.ToString(),
                ApplicantName = entity.ApplicantName,
                LicenseType = entityTypeName,
                RequestDate = entity.RequestDate,
                CurrentStatus = entity.Status,
            };

            if (entity is DrivingLicenseRenewal dlr)
            {
                detailsDto.LicenseNumber = dlr.CurrentLicenseNumber.ToString();
            }
            return detailsDto;
        }

        private async Task<bool> UpdateLicenseRequestStatusAsync(Guid publicId, string newStatus, string? notes, bool sendSignalRNotification = true)
        {
            var (request, repository, licenseType, entityIntId) = await GetLicenseEntityAndRepoByPublicIdAsync(publicId, "UpdateLicenseRequestStatus");

            string oldStatus = request.Status;
            request.Status = newStatus;
            request.LastUpdated = DateTime.UtcNow;
            if (notes != null) request.Notes = notes;

            _logger.LogWarning($"LicenseRequestHistory persistence for licenses is conceptual in this AdminService version and not fully implemented.");

            await _unitOfWork.CompleteAsync();
            _logger.LogInformation($"AdminService: License request {publicId} (Type: {licenseType}, IntID: {entityIntId}) status updated from {oldStatus} to {newStatus}.");

            if (sendSignalRNotification)
            {
                var updatedRequestSummary = new RequestSummaryDto
                {
                    RequestId = publicId,
                    RequestType = licenseType,
                    ApplicantName = request.ApplicantName,
                    ApplicantNID = request.ApplicantNID.ToString(),
                    RequestDate = request.RequestDate,
                    Status = newStatus,
                    DetailsApiEndpoint = $"/api/admin/requests/license/{publicId}"
                };
                await _dashboardHubContext.Clients.Group("AdminGroup").ReceiveRequestUpdated(updatedRequestSummary);
                await _dashboardHubContext.Clients.Group("AdminGroup").SendAdminNotification($"License Request {publicId} status changed to {newStatus}.");
            }
            return true;
        }

        public async Task<bool> ApproveLicenseRequestAsync(Guid id, UpdateRequestStatusInputDto input)
        {
            return await UpdateLicenseRequestStatusAsync(id, "Approved", input.Notes);
        }

        public async Task<bool> RejectLicenseRequestAsync(Guid id, UpdateRequestStatusInputDto input)
        {
            if (string.IsNullOrWhiteSpace(input.Notes)) return false;
            return await UpdateLicenseRequestStatusAsync(id, "Rejected", input.Notes);
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



