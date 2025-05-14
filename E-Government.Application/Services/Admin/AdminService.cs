using E_Government.Core.Domain.Entities;
using E_Government.Core.Domain.RepositoryContracts.Persistence;
using E_Government.Core.DTO;
using E_Government.Core.Exceptions;
using E_Government.Core.Helper.Hub;
using E_Government.Core.ServiceContracts;
using MapsterMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Application.Services.Admin
{
    public class AdminService : IAdminService // Implements the IAdminService interface
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminService> _logger;
        private readonly IHubContext<DashboardHub, IHubService> _dashboardHubContext; // Injected Hub Context

        public AdminService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<AdminService> logger,
            IHubContext<DashboardHub, IHubService> dashboardHubContext // Dependency Injection for Hub Context
            )
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dashboardHubContext = dashboardHubContext ?? throw new ArgumentNullException(nameof(dashboardHubContext));
        }

        public async Task<IEnumerable<ApplicationUserDto>> GetAllUsersAsync()
        {
            _logger.LogInformation("AdminService: Fetching all users.");
            var users = await _unitOfWork.GetRepository<ApplicationUser, string>().GetAllAsync();

            if (users == null || !users.Any())
            {
                _logger.LogWarning("AdminService: No users found.");
                return new List<ApplicationUserDto>();
            }
            return _mapper.Map<IEnumerable<ApplicationUserDto>>(users);
        }

        public async Task<DashboardStatisticsDto> GetDashboardStatisticsAsync()
        {
            _logger.LogInformation("AdminService: Fetching dashboard statistics.");
            var civilStats = await _unitOfWork.GetRepository<CivilDocumentRequest, Guid>().GetStatusCountsAsync();

            var licenseStats = new Dictionary<string, int> { ["Total"] = 0, ["Pending"] = 0, ["Approved"] = 0, ["Rejected"] = 0 };
            try
            {
                var licenseTypes = new List<Type> { typeof(DrivingLicenseRenewal), typeof(LicenseReplacementRequest), typeof(VehicleLicenseRenewal) };
                foreach (var type in licenseTypes)
                {
                    var method = _unitOfWork.GetType().GetMethod("GetRepository").MakeGenericMethod(type, typeof(int));
                    dynamic repository = method.Invoke(_unitOfWork, null);
                    // Assuming GetStatusCountsAsync is available for these repositories as per IGenericRepository
                    var counts = await repository.GetStatusCountsAsync();
                    licenseStats["Total"] += counts.ContainsKey("Total") ? counts["Total"] : 0;
                    licenseStats["Pending"] += counts.ContainsKey("Pending") ? counts["Pending"] : 0;
                    licenseStats["Approved"] += counts.ContainsKey("Approved") ? counts["Approved"] : 0;
                    licenseStats["Rejected"] += counts.ContainsKey("Rejected") ? counts["Rejected"] : 0;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting license requests for dashboard. GetStatusCountsAsync might be missing or fail for some license repositories.");
            }

            var totalUsers = await _unitOfWork.GetRepository<ApplicationUser, string>().CountAsync();
            var activeUsers = await _unitOfWork.GetRepository<ApplicationUser, string>().CountAsync(u => u.LastLoginDate >= DateTime.UtcNow.AddDays(-30));

            var stats = new DashboardStatisticsDto
            {
                TotalCivilDocRequests = civilStats.ContainsKey("Total") ? civilStats["Total"] : 0,
                PendingCivilDocRequests = civilStats.ContainsKey("Pending") ? civilStats["Pending"] : 0,
                ApprovedCivilDocRequests = civilStats.ContainsKey("Approved") ? civilStats["Approved"] : 0,
                RejectedCivilDocRequests = civilStats.ContainsKey("Rejected") ? civilStats["Rejected"] : 0,
                TotalLicenseRequests = licenseStats["Total"],
                PendingLicenseRequests = licenseStats["Pending"],
                ApprovedLicenseRequests = licenseStats["Approved"],
                RejectedLicenseRequests = licenseStats["Rejected"],
                TotalUsers = (int)totalUsers,
                ActiveUsers = (int)activeUsers,
                OtherStats = new Dictionary<string, int>()
            };

            // Corrected SignalR call based on provided IHubService
            await _dashboardHubContext.Clients.Group("AdminGroup").ReceiveStatisticsUpdate(stats);
            return stats;
        }

        public async Task<PagedResult<RequestSummaryDto>> GetAllRequestsAsync(int pageNumber, int pageSize, string? statusFilter, string? typeFilter, string? searchTerm)
        {
            _logger.LogInformation($"AdminService: Fetching requests. Page: {pageNumber}, Size: {pageSize}, Status: {statusFilter}, Type: {typeFilter}, Search: {searchTerm}");

            var combinedRequests = new List<RequestSummaryDto>();
            int totalCount = 0;

            // 1. Civil Document Requests
            if (string.IsNullOrEmpty(typeFilter) || typeFilter.Equals("CivilDocument", StringComparison.OrdinalIgnoreCase))
            {
                Expression<Func<CivilDocumentRequest, bool>> civilPredicate = r => true;
                if (!string.IsNullOrEmpty(statusFilter)) civilPredicate = civilPredicate.And(r => r.Status.Equals(statusFilter, StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    civilPredicate = civilPredicate.And(r =>
                        (r.ApplicantName != null && r.ApplicantName.Contains(searchTerm)) ||
                        (r.ApplicantNID != null && r.ApplicantNID.Contains(searchTerm)) ||
                        (r.DocumentType != null && r.DocumentType.Contains(searchTerm))
                    );
                }
                // Using GetPagedListAsync from IGenericRepository
                var civilPagedResult = await _unitOfWork.GetRepository<CivilDocumentRequest, Guid>()
                    .GetPagedListAsync(pageNumber, pageSize, civilPredicate, q => q.OrderByDescending(r => r.CreatedAt));

                combinedRequests.AddRange(_mapper.Map<List<RequestSummaryDto>>(civilPagedResult.Items));
                totalCount += civilPagedResult.TotalCount; // This is still problematic for combined pagination
            }

            // 2. License Requests (Example for DrivingLicenseRenewal)
            // TODO: Implement for other license types (LicenseReplacementRequest, VehicleLicenseRenewal)
            // and address the ID type mismatch (Guid vs Int) for RequestSummaryDto.RequestId
            // Also, pagination for combined results needs a more robust strategy.
            if (string.IsNullOrEmpty(typeFilter) || typeFilter.Equals("DrivingLicenseRenewal", StringComparison.OrdinalIgnoreCase))
            {
                Expression<Func<DrivingLicenseRenewal, bool>> dlrPredicate = r => true;
                // if (!string.IsNullOrEmpty(statusFilter)) dlrPredicate = dlrPredicate.And(r => r.StatusProperty == statusFilter); // Needs StatusProperty
                // if (!string.IsNullOrEmpty(searchTerm)) dlrPredicate = dlrPredicate.And(r => r.NID.ToString().Contains(searchTerm));

                // Using GetPagedListAsync
                var dlrPagedResult = await _unitOfWork.GetRepository<DrivingLicenseRenewal, int>()
                    .GetPagedListAsync(pageNumber, pageSize, dlrPredicate, q => q.OrderByDescending(r => r.RenewalDate)); // Assuming RenewalDate exists

                // combinedRequests.AddRange(_mapper.Map<List<RequestSummaryDto>>(dlrPagedResult.Items)); // Mapping needs to handle Int ID
                // totalCount += dlrPagedResult.TotalCount;
                _logger.LogWarning("GetAllRequestsAsync: Mapping for DrivingLicenseRenewal to RequestSummaryDto needs to handle Int ID to String RequestId. Combined pagination is still simplified.");
            }

            // The current combined pagination logic is flawed. A proper solution would be to:
            // 1. Fetch all items matching filters from all sources.
            // 2. Combine them into a single List<RequestSummaryDto>.
            // 3. Sort the combined list.
            // 4. Manually paginate the sorted combined list.
            // This is complex and potentially inefficient. For now, returning a simplified structure.

            var orderedRequests = combinedRequests.OrderByDescending(r => r.RequestDate).ToList();
            // totalCount should be the sum of all filtered items from all sources BEFORE pagination.
            // The current totalCount is not accurate for a truly combined list.
            var pagedItems = orderedRequests.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new PagedResult<RequestSummaryDto>
            {
                Items = pagedItems,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount, // This is an approximation for now.
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<CivilDocumentRequestDetailsDto> GetCivilDocumentRequestDetailsAsync(Guid id)
        {
            _logger.LogInformation($"AdminService: Fetching civil document request details for ID: {id}");
            var entity = await _unitOfWork.GetRepository<CivilDocumentRequest, Guid>()
                .GetByIdWithIncludeAsync(id, q => q.Include(r => r.Attachments).Include(r => r.History));

            if (entity == null)
            {
                _logger.LogWarning($"AdminService: CivilDocumentRequest with ID {id} not found.");
                throw new NotFoundException($"CivilDocumentRequest with ID {id} not found.");
            }
            return _mapper.Map<CivilDocumentRequestDetailsDto>(entity);
        }

        private async Task<bool> UpdateCivilDocumentRequestStatusAsync(Guid requestId, string newStatus, string? notes, bool sendSignalRNotification = true)
        {
            var request = await _unitOfWork.GetRepository<CivilDocumentRequest, Guid>().GetAsync(requestId);
            if (request == null)
            {
                _logger.LogWarning($"AdminService: CivilDocumentRequest with ID {requestId} not found for status update.");
                return false;
            }

            string oldStatus = request.Status;
            request.Status = newStatus;
            request.LastUpdated = DateTime.UtcNow;

            var history = new CivilDocumentRequestHistory
            {
                Id = Guid.NewGuid(),
                RequestId = requestId,
                Status = newStatus,
                Note = notes ?? string.Empty,
                ChangedAt = DateTime.UtcNow
            };
            await _unitOfWork.GetRepository<CivilDocumentRequestHistory, Guid>().AddAsync(history);

            await _unitOfWork.CompleteAsync();
            _logger.LogInformation($"AdminService: CivilDocumentRequest ID {requestId} status updated from {oldStatus} to {newStatus}.");

            if (sendSignalRNotification)
            {
                var updatedRequestSummary = _mapper.Map<RequestSummaryDto>(request);
                await _dashboardHubContext.Clients.Group("AdminGroup").ReceiveRequestUpdated(updatedRequestSummary);
                // Corrected SignalR call based on provided IHubService
                await _dashboardHubContext.Clients.Group("AdminGroup").SendAdminNotification($"Civil Document Request {updatedRequestSummary.RequestId} status changed to {newStatus}.");
            }
            return true;
        }

        public async Task<bool> ApproveCivilDocumentRequestAsync(Guid id, UpdateRequestStatusInputDto input)
        {
            _logger.LogInformation($"AdminService: Approving civil document request ID: {id} with notes: {input.Notes}");
            return await UpdateCivilDocumentRequestStatusAsync(id, "Approved", input.Notes);
        }

        public async Task<bool> RejectCivilDocumentRequestAsync(Guid id, UpdateRequestStatusInputDto input)
        {
            _logger.LogInformation($"AdminService: Rejecting civil document request ID: {id} with notes: {input.Notes}");
            if (string.IsNullOrWhiteSpace(input.Notes))
            {
                _logger.LogWarning("AdminService: Rejection notes are required for civil document request.");
                return false;
            }
            return await UpdateCivilDocumentRequestStatusAsync(id, "Rejected", input.Notes);
        }

        public async Task<LicenseRequestDetailsDto> GetLicenseRequestDetailsAsync(Guid id)
        {
            _logger.LogError($"AdminService: GetLicenseRequestDetailsAsync called with GUID {id}. License entities use INT IDs. This needs redesign (e.g., string composite ID).");
            throw new NotImplementedException("Cannot fetch license details by GUID due to ID type mismatch. Consider using a composite string ID like 'LicenseType_IntId'.");
        }

        public async Task<bool> ApproveLicenseRequestAsync(Guid id, UpdateRequestStatusInputDto input)
        {
            _logger.LogError($"AdminService: ApproveLicenseRequestAsync called with GUID {id}. ID mismatch. Needs redesign.");
            // Example if ID was parsed: 
            // (string licenseType, int actualId) = ParseCompositeLicenseId(id.ToString()); 
            // ... then find repo by type, update status, and send SignalR notification ...
            // await _dashboardHubContext.Clients.Group("AdminGroup").SendAdminNotification($"License Request {actualId} ({licenseType}) approved.");
            throw new NotImplementedException("Cannot approve license request by GUID. ID mismatch. Needs redesign.");
        }

        public async Task<bool> RejectLicenseRequestAsync(Guid id, UpdateRequestStatusInputDto input)
        {
            _logger.LogError($"AdminService: RejectLicenseRequestAsync called with GUID {id}. ID mismatch. Needs redesign.");
            if (string.IsNullOrWhiteSpace(input.Notes))
            {
                _logger.LogWarning("AdminService: Rejection notes are required for license request.");
                return false;
            }
            // await _dashboardHubContext.Clients.Group("AdminGroup").SendAdminNotification($"License Request ... rejected.");
            throw new NotImplementedException("Cannot reject license request by GUID. ID mismatch. Needs redesign.");
        }
    }

    // Helper for combining expressions (optional, can be in a utility class)
    public static class PredicateBuilder
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.OrElse);
        }

        private static Expression<Func<T, bool>> Compose<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second, Func<Expression, Expression, Expression> merge)
        {
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);
            return Expression.Lambda<Func<T, bool>>(merge(first.Body, secondBody), first.Parameters);
        }

        private class ParameterRebinder : ExpressionVisitor
        {
            private readonly Dictionary<ParameterExpression, ParameterExpression> _map;

            private ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
            {
                _map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
            }

            public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
            {
                return new ParameterRebinder(map).Visit(exp);
            }

            protected override Expression VisitParameter(ParameterExpression p)
            {
                ParameterExpression replacement;
                if (_map.TryGetValue(p, out replacement))
                {
                    p = replacement;
                }
                return base.VisitParameter(p);
            }
        }
    }
}
