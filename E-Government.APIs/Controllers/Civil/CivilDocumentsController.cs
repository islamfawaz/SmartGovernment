using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using E_Government.Core.DTO;
using E_Government.Application.Services;
using E_Government.UI.Controllers.Base;

namespace E_Government.APIs.Controllers.Civil
{
    [ApiController]
    public class CivilDocumentsController : ApiControllerBase
    {
        private readonly CivilDocumentsService _service;
        public CivilDocumentsController(CivilDocumentsService service)
        {
            _service = service;
        }

        [HttpPost("request")]
        public async Task<IActionResult> SubmitRequest([FromBody] CivilDocumentRequestDto dto)
        {
            var id = await _service.SubmitRequestAsync(dto);
            return Ok(id);
        }

        [HttpGet("request/{requestId}/status")]
        public async Task<IActionResult> GetStatus(Guid requestId)
        {
            var status = await _service.GetRequestStatusAsync(requestId);
            if (status == null) return NotFound();
            return Ok(status);
        }

        [HttpGet("my-requests")]
        public async Task<IActionResult> GetMyRequests([FromQuery] string userNID)
        {
            var requests = await _service.GetUserRequestsAsync(userNID);
            return Ok(requests);
        }

        [HttpPost("request/{requestId}/attachments")]
        public async Task<IActionResult> AddAttachment(Guid requestId, [FromForm] string fileType, [FromForm] string filePath)
        {
            await _service.AddAttachmentAsync(requestId, fileType, filePath);
            return Ok();
        }

        [HttpPut("request/{requestId}/status")]
        public async Task<IActionResult> UpdateStatus(Guid requestId, [FromBody] UpdateRequestStatusDto dto)
        {
            await _service.UpdateRequestStatusAsync(requestId, dto.Status, dto.Note);
            return Ok();
        }
    }
} 