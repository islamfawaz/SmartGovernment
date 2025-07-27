using E_Government.Application.DTO.FIle;
using E_Government.Application.ServiceContracts;
using E_Government.Application.Services.File;
using Microsoft.AspNetCore.Mvc;

namespace E_Government.APIs.Controllers.File
{
    [Route("api/[controller]")]
    public class FileUploadController : ControllerBase
    {
        private readonly IFileUploadService _fileUploadService;

        public FileUploadController(IFileUploadService fileUploadService)
        {
            _fileUploadService = fileUploadService;
        }

        [HttpPost("single")]
        public async Task<IActionResult> UploadSingleFile(IFormFile file, [FromQuery] string folder = "general")
        {
            var options = new FileUploadOptions
            {
                UploadPath = folder,
                MaxFileSize = 10 * 1024 * 1024, // 10MB
                AllowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf" }
            };

            var result = await _fileUploadService.UploadFileAsync(file, options);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPost("multiple")]
        public async Task<IActionResult> UploadMultipleFiles(IFormFileCollection files, [FromQuery] string folder = "general")
        {
            var options = new FileUploadOptions
            {
                UploadPath = folder,
                MaxFileSize = 5 * 1024 * 1024 // 5MB
            };

            var result = await _fileUploadService.UploadMultipleFilesAsync(files, options);
            return Ok(result);
        }

        [HttpPost("image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            var options = new FileUploadOptions
            {
                UploadPath = "images",
                AllowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" },
                AllowedContentTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" },
                MaxFileSize = 5 * 1024 * 1024 // 5MB
            };

            var result = await _fileUploadService.UploadFileAsync(file, options);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpDelete("{fileName}")]
        public async Task<IActionResult> DeleteFile(string fileName, [FromQuery] string folder = null)
        {
            var result = await _fileUploadService.DeleteFileAsync(fileName, folder);

            if (result)
                return Ok(new { message = "File deleted successfully" });

            return NotFound(new { message = "File not found" });
        }

        [HttpGet("exists/{fileName}")]
        public IActionResult CheckFileExists(string fileName, [FromQuery] string folder = null)
        {
            var exists = _fileUploadService.FileExists(fileName, folder);
            return Ok(new { exists });
        }
    }
}
