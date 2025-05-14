// Assuming these are the correct namespaces from the user's project
using E_Government.Core.Domain.Entities;
using E_Government.Core.DTO;
using E_Government.Core.Helper.Hub;
using E_Government.Core.ServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace E_Government.APIs.Controllers.Admin
{
    [Authorize(Roles = "Admin")] // Assuming Admin role for authorization
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ILicenseService _licenseService;
        private readonly ICivilDocumentsService _civilDocumentsService;
        private readonly UserManager<ApplicationUser> _userManager; // From user's ApplicationUser.cs
        private readonly IHubContext<DashboardHub, IHubService> _dashboardHubContext; // Using IHubService for typed client calls
        private readonly ILogger<AdminController> _logger;
        private readonly IBillingService _billingService; // Added based on GitHub review
        // private readonly IUnitOfWork _unitOfWork; // If used for transactions across services

        public AdminController(
            ILicenseService licenseService,
            ICivilDocumentsService civilDocumentsService,
            UserManager<ApplicationUser> userManager,
            IHubContext<DashboardHub, IHubService> dashboardHubContext,
            ILogger<AdminController> logger,
            IBillingService billingService)
        {
            _licenseService = licenseService;
            _civilDocumentsService = civilDocumentsService;
            _userManager = userManager;
            _dashboardHubContext = dashboardHubContext;
            _logger = logger;
            _billingService = billingService;
        }

        [HttpGet("statistics")]
        public async Task<ActionResult<DashboardStatisticsDto>> GetDashboardStatistics()
        {
            _logger.LogInformation("Fetching dashboard statistics.");
            try
            {
                // TODO: Replace with actual calls to services to get counts
                // Example: var pendingLicenses = await _licenseService.GetRequestsByStatusAsync("Pending");
                // Example: var activeUsers = _userManager.Users.Count(u => u.IsActive); // Assuming IsActive property exists

                var stats = new DashboardStatisticsDto
                {
                    TotalLicenseRequests = 150, // Placeholder
                    PendingLicenseRequests = 25, // Placeholder
                    ApprovedLicenseRequests = 100, // Placeholder
                    RejectedLicenseRequests = 25, // Placeholder
                    TotalCivilDocRequests = 300, // Placeholder
                    PendingCivilDocRequests = 40, // Placeholder
                    ApprovedCivilDocRequests = 250, // Placeholder
                    RejectedCivilDocRequests = 10, // Placeholder
                    ActiveUsers = _userManager.Users.Count(), // Example, refine with actual active status logic
                    TotalUsers = _userManager.Users.Count(),
                    OtherStats = new Dictionary<string, int>
                    {
                        { "BillsPaidToday", 50 }, // Placeholder
                        { "TrafficViolationsPending", 15 } // Placeholder
                    }
                };
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dashboard statistics.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error fetching statistics");
            }
        }

        [HttpGet("requests")]
        public async Task<ActionResult<PagedResult<RequestSummaryDto>>> GetAllRequests(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null,
            [FromQuery] string? type = null, // "License", "CivilDocument"
            [FromQuery] string? searchTerm = null)
        {
            _logger.LogInformation($"Fetching requests: page {pageNumber}, size {pageSize}, status {status}, type {type}, search {searchTerm}");
            try
            {
                var allRequests = new List<RequestSummaryDto>();

                // TODO: Implement actual fetching and filtering logic from _licenseService and _civilDocumentsService
                // This will involve combining results if type is null or specific type is requested.
                // Example for licenses:
                // var licenseRequests = await _licenseService.GetAllRequestsAsync(pageNumber, pageSize, status, searchTerm);
                // Map licenseRequests to RequestSummaryDto

                // Example for civil docs:
                // var civilDocRequests = await _civilDocumentsService.GetAllRequestsAsync(pageNumber, pageSize, status, searchTerm);
                // Map civilDocRequests to RequestSummaryDto

                // Placeholder data:
                if (type == "License" || string.IsNullOrEmpty(type))
                {
                    allRequests.Add(new RequestSummaryDto { RequestId = Guid.NewGuid().ToString(), ApplicantName = "License Applicant 1", ApplicantNID = "12345678901234", RequestType = "Driving License Renewal", RequestDate = DateTime.UtcNow.AddDays(-2), Status = "Pending", DetailsApiEndpoint = $"/api/admin/requests/license/{allRequests.Count + 1}" });
                }
                if (type == "CivilDocument" || string.IsNullOrEmpty(type))
                {
                    allRequests.Add(new RequestSummaryDto { RequestId = Guid.NewGuid().ToString(), ApplicantName = "Civil Applicant 1", ApplicantNID = "98765432109876", RequestType = "Birth Certificate", RequestDate = DateTime.UtcNow.AddDays(-5), Status = "Approved", DetailsApiEndpoint = $"/api/admin/requests/civil/{allRequests.Count + 1}" });
                }

                var pagedResult = new PagedResult<RequestSummaryDto>
                {
                    Items = allRequests.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(),
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = allRequests.Count,
                    TotalPages = (int)Math.Ceiling(allRequests.Count / (double)pageSize)
                };

                return Ok(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching requests.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error fetching requests");
            }
        }

        [HttpGet("requests/license/{id}")]
        public async Task<ActionResult<LicenseRequestDetailsDto>> GetLicenseRequestDetails(Guid id)
        {
            _logger.LogInformation($"Fetching license request details for ID: {id}");
            try
            {
                // TODO: Replace with actual call to _licenseService.GetLicenseRequestByIdAsync(id);
                // var licenseRequest = await _licenseService.GetLicenseByIdAsync(id); // Assuming a method like this
                // if (licenseRequest == null) return NotFound();
                // Map to LicenseRequestDetailsDto

                // Placeholder data:
                var placeholderDetails = new LicenseRequestDetailsDto
                {
                    Id = id,
                    ApplicantNID = "12345678901234",
                    ApplicantName = "License Applicant 1",
                    ApplicantAddress = "123 Main St",
                    DateOfBirth = new DateTime(1990, 1, 1),
                    LicenseType = "Private",
                    PhotoUrl = "/path/to/photo.jpg",
                    MedicalTestStatus = "Passed",
                    TheoryTestStatus = "Passed",
                    PracticalTestStatus = "Pending",
                    RequestDate = DateTime.UtcNow.AddDays(-2),
                    CurrentStatus = "Pending",
                    History = new List<RequestHistoryDto> { new RequestHistoryDto { Date = DateTime.UtcNow.AddDays(-2), Status = "Submitted", Remarks = "Initial submission" } }
                };
                return Ok(placeholderDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching license request details for ID: {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error fetching license request details");
            }
        }

        [HttpGet("requests/civil/{id}")]
        public async Task<ActionResult<CivilDocumentRequestDetailsDto>> GetCivilDocumentRequestDetails(Guid id)
        {
            _logger.LogInformation($"Fetching civil document request details for ID: {id}");
            try
            {
                // TODO: Replace with actual call to _civilDocumentsService.GetRequestByIdAsync(id);
                // var civilRequest = await _civilDocumentsService.GetRequestStatusAsync(id); // This seems to return CivilDocumentRequestDto based on interface
                // if (civilRequest == null) return NotFound();
                // Map to CivilDocumentRequestDetailsDto, potentially fetching more details if needed

                // Placeholder data:
                var placeholderDetails = new CivilDocumentRequestDetailsDto
                {
                    Id = id,
                    ApplicantNID = "98765432109876",
                    ApplicantName = "Civil Applicant 1",
                    DocumentType = "Birth Certificate",
                    RequestDate = DateTime.UtcNow.AddDays(-5),
                    CurrentStatus = "Approved",
                    Notes = "All documents verified.",
                    Attachments = new List<CivilDocumentAttachmentDto> { new CivilDocumentAttachmentDto { Id = Guid.NewGuid() , FilePath = "/uploads/id_copy.pdf", FileType = "application/pdf", UploadedAt = DateTime.UtcNow.AddDays(-5) } },
                    History = new List<RequestHistoryDto> { new RequestHistoryDto { Date = DateTime.UtcNow.AddDays(-5), Status = "Submitted", Remarks = "Initial submission" }, new RequestHistoryDto { Date = DateTime.UtcNow.AddDays(-4), Status = "Approved", Remarks = "Processed by admin" } }
                };
                return Ok(placeholderDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching civil document request details for ID: {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error fetching civil document request details");
            }
        }

        [HttpPost("requests/license/{id}/approve")]
        public async Task<IActionResult> ApproveLicenseRequest(Guid id, [FromBody] UpdateRequestStatusInputDto input)
        {
            _logger.LogInformation($"Approving license request ID: {id}");
            try
            {
                // TODO: Implement logic using _licenseService.UpdateRequestStatusAsync(id, "Approved", input.Notes);
                // After successful update:
                // var updatedRequestSummary = await _licenseService.GetRequestSummaryByIdAsync(id); // Fetch updated summary
                // await _dashboardHubContext.Clients.All.SendRequestUpdated(updatedRequestSummary);
                // var newStats = await GetDashboardStatisticsInternal(); // Internal method to get stats
                // await _dashboardHubContext.Clients.All.SendStatisticsUpdate(newStats);
                await _dashboardHubContext.Clients.All.SendAdminNotification($"License request {id} approved.");
                return Ok(new { message = "License request approved successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error approving license request ID: {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error approving license request");
            }
        }

        [HttpPost("requests/license/{id}/reject")]
        public async Task<IActionResult> RejectLicenseRequest(Guid id, [FromBody] UpdateRequestStatusInputDto input)
        {
            _logger.LogInformation($"Rejecting license request ID: {id} with reason: {input.Notes}");
            try
            {
                // TODO: Implement logic using _licenseService.UpdateRequestStatusAsync(id, "Rejected", input.Notes);
                // After successful update:
                // var updatedRequestSummary = await _licenseService.GetRequestSummaryByIdAsync(id); // Fetch updated summary
                // await _dashboardHubContext.Clients.All.SendRequestUpdated(updatedRequestSummary);
                // var newStats = await GetDashboardStatisticsInternal();
                // await _dashboardHubContext.Clients.All.SendStatisticsUpdate(newStats);
                await _dashboardHubContext.Clients.All.SendAdminNotification($"License request {id} rejected.");
                return Ok(new { message = "License request rejected successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error rejecting license request ID: {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error rejecting license request");
            }
        }

        [HttpPost("requests/civil/{id}/approve")]
        public async Task<IActionResult> ApproveCivilDocumentRequest(Guid id, [FromBody] UpdateRequestStatusInputDto input)
        {
            _logger.LogInformation($"Approving civil document request ID: {id}");
            try
            {
                // Based on ICivilDocumentsService: await _civilDocumentsService.UpdateRequestStatusAsync(id, "Approved", input.Notes);
                var result =  _civilDocumentsService.UpdateRequestStatusAsync(id, "Approved", input.Notes ?? "Approved by Admin");
                // if (!result.Succeeded) return BadRequest(result.Errors); // Assuming result has Succeeded and Errors

                // After successful update:
                // var updatedRequestSummary = await _civilDocumentsService.GetRequestSummaryByIdAsync(id); // Fetch updated summary
                // await _dashboardHubContext.Clients.All.SendRequestUpdated(updatedRequestSummary);
                // var newStats = await GetDashboardStatisticsInternal();
                // await _dashboardHubContext.Clients.All.SendStatisticsUpdate(newStats);
                await _dashboardHubContext.Clients.All.SendAdminNotification($"Civil document request {id} approved.");
                return Ok(new { message = "Civil document request approved successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error approving civil document request ID: {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error approving civil document request");
            }
        }

        [HttpPost("requests/civil/{id}/reject")]
        public async Task<IActionResult> RejectCivilDocumentRequest(Guid id, [FromBody] UpdateRequestStatusInputDto input)
        {
            _logger.LogInformation($"Rejecting civil document request ID: {id} with reason: {input.Notes}");
            try
            {
                // Based on ICivilDocumentsService: await _civilDocumentsService.UpdateRequestStatusAsync(id, "Rejected", input.Notes);
                var result =  _civilDocumentsService.UpdateRequestStatusAsync(id, "Rejected", input.Notes ?? "Rejected by Admin");
                // if (!result.Succeeded) return BadRequest(result.Errors);

                // After successful update:
                // var updatedRequestSummary = await _civilDocumentsService.GetRequestSummaryByIdAsync(id); // Fetch updated summary
                // await _dashboardHubContext.Clients.All.SendRequestUpdated(updatedRequestSummary);
                // var newStats = await GetDashboardStatisticsInternal();
                // await _dashboardHubContext.Clients.All.SendStatisticsUpdate(newStats);
                await _dashboardHubContext.Clients.All.SendAdminNotification($"Civil document request {id} rejected.");
                return Ok(new { message = "Civil document request rejected successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error rejecting civil document request ID: {id}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error rejecting civil document request");
            }
        }

        [HttpGet("users")]
        public async Task<ActionResult<PagedResult<UserSummaryDto>>> GetUsers(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null)
        {
            _logger.LogInformation($"Fetching users: page {pageNumber}, size {pageSize}, search {searchTerm}");
            try
            {
                var query = _userManager.Users;
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(u => u.Email.Contains(searchTerm) || u.UserName.Contains(searchTerm) || (u.NID != null && u.NID.Contains(searchTerm)));
                }

                var totalCount = query.Count();
                var users = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                var userSummaries = new List<UserSummaryDto>();
                foreach (var user in users)
                {
                    userSummaries.Add(new UserSummaryDto
                    {
                        UserId = user.Id,
                        FullName = user.UserName, // Assuming UserName is used for FullName or a specific property exists
                        Email = user.Email,
                        NID = user.NID, // From ApplicationUser.cs
                        PhoneNumber = user.PhoneNumber,
                        Address = user.Address, // From ApplicationUser.cs
                        // Roles = (await _userManager.GetRolesAsync(user)).ToList(), // This can be performance intensive here
                        IsActive = user.EmailConfirmed // Example: using EmailConfirmed as IsActive, adjust as needed
                    });
                }

                var pagedResult = new PagedResult<UserSummaryDto>
                {
                    Items = userSummaries,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };
                return Ok(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching users.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error fetching users");
            }
        }

        [HttpGet("charts/request-trends")]
        public async Task<ActionResult<ChartDataDto>> GetRequestTrends()
        {
            _logger.LogInformation("Fetching chart data for request trends.");
            // TODO: Implement actual data retrieval for chart
            var chartData = new ChartDataDto
            {
                Labels = new List<string> { "Jan", "Feb", "Mar", "Apr", "May", "Jun" },
                Datasets = new List<ChartDatasetDto>
                {
                    new ChartDatasetDto { Label = "License Requests", Data = new List<object> { 12, 19, 3, 5, 2, 3 } },
                    new ChartDatasetDto { Label = "Civil Document Requests", Data = new List<object> { 7, 11, 5, 8, 3, 7 } }
                }
            };
            return Ok(chartData);
        }

        // Add other chart endpoints (requests-by-type, request-status-distribution) similarly

        // Internal helper to refresh stats - consider abstracting this to a service
        private async Task<DashboardStatisticsDto> GetDashboardStatisticsInternal()
        {
            // TODO: Implement actual calls to services to get counts
            return new DashboardStatisticsDto
            {
                TotalLicenseRequests = 150,
                PendingLicenseRequests = 25,
                ApprovedLicenseRequests = 100,
                RejectedLicenseRequests = 25,
                TotalCivilDocRequests = 300,
                PendingCivilDocRequests = 40,
                ApprovedCivilDocRequests = 250,
                RejectedCivilDocRequests = 10,
                ActiveUsers = _userManager.Users.Count(),
                TotalUsers = _userManager.Users.Count(),
                OtherStats = new Dictionary<string, int> { { "BillsPaidToday", 50 }, { "TrafficViolationsPending", 15 } }
            };
        }
    }
}

