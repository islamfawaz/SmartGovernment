﻿// Assuming these are the correct namespaces from the user's project
using E_Government.Application.DTO.AdminDashboard;
using E_Government.Application.DTO.CivilDocs;
using E_Government.Application.DTO.License;
using E_Government.Application.ServiceContracts;
using E_Government.Application.Services.License;
using E_Government.Domain.Entities;
using E_Government.Domain.Helper.Hub;
using E_Government.Domain.ServiceContracts.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace E_Government.APIs.Controllers.Admin
{
    // [Authorize(Roles = "Admin")] // Assuming Admin role for authorization
    [Route("api/admin")]
    [ApiController]
    public class AdminController : ControllerBase // Or Controller if you need Views, but for API, ControllerBase is fine
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminController> _logger;
        private readonly IServiceManager _serviceManager;
        private readonly ILicenseService licenseService;


         private readonly IHubContext<DashboardHub, IHubService> _dashboardHubContext; // Example if Hub is used directly

        public AdminController(IAdminService adminService,ILogger<AdminController> logger,IServiceManager serviceManager,ILicenseService licenseService,

             IHubContext<DashboardHub, IHubService> dashboardHubContext // Example
            )
        {
            _adminService = adminService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceManager = serviceManager;
            this.licenseService = licenseService;
            _dashboardHubContext = dashboardHubContext; 
        }

        [HttpGet("statistics")]
        [ProducesResponseType(typeof(DashboardStatisticsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<DashboardStatisticsDto>> GetDashboardStatistics()
        {
            _logger.LogInformation("AdminController: Attempting to fetch dashboard statistics.");
            try
            {
                var statistics = await _adminService.GetDashboardStatisticsAsync();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AdminController: Error fetching dashboard statistics.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching dashboard statistics.");
            }
        }


        [HttpGet("requests")]
        [ProducesResponseType(typeof(PagedResult<RequestSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedResult<RequestSummaryDto>>> GetAllRequests(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null,
            [FromQuery] string? type = null, // Expected types: "License", "CivilDocument"
            [FromQuery] string? searchTerm = null)
        {
            _logger.LogInformation($"AdminController: Attempting to fetch requests. Page: {pageNumber}, Size: {pageSize}, Status: {status}, Type: {type}, Search: {searchTerm}");
            try
            {
                var requests = await _adminService.GetAllRequestsAsync(pageNumber, pageSize, status, type, searchTerm);
                return Ok(requests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AdminController: Error fetching requests.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching requests.");
            }
        }

        [HttpGet("requests/license/{id:guid}")]
        [ProducesResponseType(typeof(LicenseRequestDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<LicenseRequestDetailsDto>> GetLicenseRequestDetails(Guid id)
        {
            _logger.LogInformation($"AdminController: Attempting to fetch license request details for ID: {id}");
            try
            {
                var details = await _adminService.GetLicenseRequestDetailsAsync(id);
                if (details == null)
                {
                    _logger.LogWarning($"AdminController: License request with ID {id} not found.");
                    return NotFound();
                }
                return Ok(details);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"AdminController: Error fetching license request details for ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching license request details.");
            }
        }

        [HttpGet("requests/civil/{id}")]
        [ProducesResponseType(typeof(CivilDocumentRequestDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CivilDocumentRequestDetailsDto>> GetCivilDocumentRequestDetails(Guid id)
        {
            _logger.LogInformation($"AdminController: Attempting to fetch civil document request details for ID: {id}");
            try
            {
                var details = await _adminService.GetCivilDocumentRequestDetailsAsync(id);
                if (details == null)
                {
                    _logger.LogWarning($"AdminController: Civil document request with ID {id} not found.");
                    return NotFound();
                }
                return Ok(details);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"AdminController: Error fetching civil document request details for ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching civil document request details.");
            }
        }

        [HttpPost("requests/license/{id:guid}/approve")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ApproveLicenseRequest(Guid id, [FromBody] UpdateRequestStatusInputDto input)
        {
            _logger.LogInformation($"AdminController: Attempting to approve license request ID: {id}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var paymentCode = await licenseService.ApproveLicenseRequestAsync(id, input);
                if (paymentCode is null)
                {
                    _logger.LogWarning($"AdminController: Failed to approve license request ID {id} or request not found.");
                    return NotFound("Request not found or approval failed."); // Or BadRequest depending on why it failed
                }
                // TODO: Consider what to return. Frontend might expect updated request summary or just success.
                await _dashboardHubContext.Clients.All.ReceiveRequestUpdated(new RequestSummaryDto
                {
                    RequestId = id,
                    RequestType = "License",
                    Status = "Approved",
                     RequestDate = DateTime.UtcNow,
                    DetailsApiEndpoint = $"/api/admin/requests/license/{id}"
                });
                await _dashboardHubContext.Clients.All.SendAdminNotification($"License request {id} approved.");
                return Ok(new
                {
                    message = "License request approved successfully.",
                    paymentCode = paymentCode.PaymentCode,
                    amount = paymentCode.Amount,
                    dueDate = paymentCode.DueDate
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"AdminController: Error approving license request ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while approving the license request.");
            }
        }

        [HttpPost("requests/license/{id:guid}/reject")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RejectLicenseRequest(Guid id, [FromBody] UpdateRequestStatusInputDto input)
        {
            _logger.LogInformation($"AdminController: Attempting to reject license request ID: {id}");
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(input.Notes))
            {
                ModelState.AddModelError("Notes", "Rejection notes are required."); // Ensure frontend sends notes
                return BadRequest(ModelState);
            }
            try
            {
                var success = await licenseService.RejectLicenseRequestAsync(id, input);
                if (!success)
                {
                    _logger.LogWarning($"AdminController: Failed to reject license request ID {id} or request not found.");
                    return NotFound("Request not found or rejection failed.");
                }
                // await _dashboardHubContext.Clients.All.SendRequestUpdated(id.ToString(), "License", "Rejected"); // Example SignalR notification
                // await _dashboardHubContext.Clients.All.SendAdminNotification($"License request {id} rejected.");
                return Ok(new { message = "License request rejected successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"AdminController: Error rejecting license request ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while rejecting the license request.");
            }
        }

        [HttpPost("requests/civil/{id:guid}/approve")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ApproveCivilDocumentRequest(Guid id, [FromBody] UpdateRequestStatusInputDto input)
        {
            _logger.LogInformation($"AdminController: Attempting to approve civil document request ID: {id}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var success = await _adminService.ApproveCivilDocumentRequestAsync(id, input);
                if (!success)
                {
                    _logger.LogWarning($"AdminController: Failed to approve civil document request ID {id} or request not found.");
                    return NotFound("Request not found or approval failed.");
                }
                // await _dashboardHubContext.Clients.All.SendRequestUpdated(id.ToString(), "CivilDocument", "Approved"); // Example SignalR notification
                // await _dashboardHubContext.Clients.All.SendAdminNotification($"Civil document request {id} approved.");
                return Ok(new { message = "Civil document request approved successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"AdminController: Error approving civil document request ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while approving the civil document request.");
            }
        }


        [HttpGet("payment-code/{requestId}")]
        public async Task<IActionResult> GetPaymentCode(Guid requestId)
        {
            try
            {
                var request = await licenseService.GetRequestByIdAsync(requestId);
                if (request == null)
                    return NotFound(new { Success = false, Message = "Request not found" });

                if (request.Status != "Approved")
                    return BadRequest(new { Success = false, Message = "Request is not approved yet" });

                var paymentCode = await licenseService.GeneratePaymentCode(request);
                return Ok(new { Success = true, PaymentCode = paymentCode });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting payment code for request {RequestId}", requestId);
                return StatusCode(500, new { Success = false, Message = "An error occurred while generating payment code" });
            }
        }

        [HttpPost("requests/civil/{id:guid}/reject")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RejectCivilDocumentRequest(Guid id, [FromBody] UpdateRequestStatusInputDto input)
        {
            _logger.LogInformation($"AdminController: Attempting to reject civil document request ID: {id}");
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(input.Notes))
            {
                ModelState.AddModelError("Notes", "Rejection notes are required.");
                return BadRequest(new
                {
                    Success = false,
                    Message = "Rejection notes are required.",
                    Errors = ModelState
                });
            }
            try
            {
                var success = await _adminService.RejectCivilDocumentRequestAsync(id, input);
                if (!success)
                {
                    _logger.LogWarning($"AdminController: Failed to reject civil document request ID {id} or request not found.");
                    return NotFound("Request not found or rejection failed.");
                }
                await _dashboardHubContext.Clients.All.SendAdminNotification($"Civil document request {id} rejected.");
                return Ok(new { message = "Civil document request rejected successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"AdminController: Error rejecting civil document request ID: {id}.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while rejecting the civil document request.");
            }



        }
    }
}

