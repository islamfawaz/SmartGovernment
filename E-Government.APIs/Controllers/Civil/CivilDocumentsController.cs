using E_Government.APIs.Controllers.Base;
using E_Government.Application.DTO.CivilDocs;
using E_Government.Application.ServiceContracts;
using E_Government.Domain.ServiceContracts.Common;
using Microsoft.AspNetCore.Mvc;

namespace E_Government.APIs.Controllers.Civil
{
    [ApiController]
    public class CivilDocumentsController : ApiControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public CivilDocumentsController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpPost("request")]
        public async Task<IActionResult> SubmitRequest([FromBody] CivilDocumentRequestDto dto)
        {
            var id = await _serviceManager.CivilDocsService.SubmitRequestAsync(dto);
            return Ok(id);
        }

        [HttpGet("request/{requestId}/status")]
        public async Task<IActionResult> GetStatus(Guid requestId)
        {
            var status = await _serviceManager.CivilDocsService.GetRequestStatusAsync(requestId);
            if (status == null) return NotFound();
            return Ok(status);
        }

        [HttpGet("my-requests")]
        public async Task<IActionResult> GetMyRequests([FromQuery] string userNID)
        {
            var requests = await _serviceManager.CivilDocsService.GetUserRequestsAsync(userNID);
            return Ok(requests);
        }

        [HttpPost("request/{requestId}/attachments")]
        public async Task<IActionResult> AddAttachment(Guid requestId, [FromForm] string fileType, [FromForm] string filePath)
        {
            await _serviceManager.CivilDocsService.AddAttachmentAsync(requestId, fileType, filePath);
            return Ok();
        }

        [HttpPut("request/{requestId}/status")]
        public async Task<IActionResult> UpdateStatus(Guid requestId, [FromBody] UpdateRequestStatusDto dto)
        {
            await _serviceManager.CivilDocsService.UpdateRequestStatusAsync(requestId, dto.Status, dto.Note);
            return Ok();
        }
    }
} 