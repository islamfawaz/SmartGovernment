using E_Government.Application.DTO.FIle;
using E_Government.Application.ServiceContracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace E_Government.Application.Services.File
{
 
        public class FileUploadService : IFileUploadService
        {
            private readonly IWebHostEnvironment _environment;
            private readonly ILogger<FileUploadService> _logger;
            private readonly string _baseUploadPath;

            public FileUploadService(IWebHostEnvironment environment, ILogger<FileUploadService> logger)
            {
                _environment = environment;
                _logger = logger;
                _baseUploadPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, "uploads");
            }

            public async Task<FileUploadResult> UploadFileAsync(IFormFile file, FileUploadOptions options = null)
            {
                options ??= new FileUploadOptions();

                try
                {
                    // التحقق من صحة الملف
                    var validationResult = ValidateFile(file, options);
                    if (!validationResult.Success)
                        return validationResult;

                    // إنشاء مجلد الرفع
                    var uploadPath = Path.Combine(_baseUploadPath, options.UploadPath);
                    Directory.CreateDirectory(uploadPath);

                    // إنشاء اسم الملف
                    var fileName = options.GenerateUniqueFileName
                        ? GenerateUniqueFileName(file.FileName)
                        : file.FileName;

                    var filePath = Path.Combine(uploadPath, fileName);

                    // رفع الملف
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    _logger.LogInformation($"File uploaded successfully: {fileName}");

                    return new FileUploadResult
                    {
                        Success = true,
                        Message = "File uploaded successfully",
                        FileName = fileName,
                        FilePath = filePath,
                        FileUrl = GetFileUrl(fileName, options.UploadPath),
                        FileSize = file.Length,
                        ContentType = file.ContentType
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error uploading file");
                    return new FileUploadResult
                    {
                        Success = false,
                        Message = $"Error uploading file: {ex.Message}"
                    };
                }
            }

            public async Task<MultipleFileUploadResult> UploadMultipleFilesAsync(IFormFileCollection files, FileUploadOptions options = null)
            {
                var result = new MultipleFileUploadResult();

                foreach (var file in files)
                {
                    var uploadResult = await UploadFileAsync(file, options);
                    result.Results.Add(uploadResult);

                    if (uploadResult.Success)
                        result.SuccessCount++;
                    else
                        result.FailedCount++;
                }

                return result;
            }

            public async Task<bool> DeleteFileAsync(string fileName, string subPath = null)
            {
                try
                {
                    var uploadPath = string.IsNullOrEmpty(subPath)
                        ? _baseUploadPath
                        : Path.Combine(_baseUploadPath, subPath);

                    var filePath = Path.Combine(uploadPath, fileName);

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                        _logger.LogInformation($"File deleted successfully: {fileName}");
                        return true;
                    }

                    return false;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error deleting file: {fileName}");
                    return false;
                }
            }

            public bool FileExists(string fileName, string subPath = null)
            {
                var uploadPath = string.IsNullOrEmpty(subPath)
                    ? _baseUploadPath
                    : Path.Combine(_baseUploadPath, subPath);

                var filePath = Path.Combine(uploadPath, fileName);
                  return System.IO.File.Exists(filePath);
        }

        public FileUploadResult ValidateFile(IFormFile file, FileUploadOptions options)
            {
                if (file == null || file.Length == 0)
                {
                    return new FileUploadResult
                    {
                        Success = false,
                        Message = "No file provided or file is empty"
                    };
                }

                // فحص حجم الملف
                if (file.Length > options.MaxFileSize)
                {
                    return new FileUploadResult
                    {
                        Success = false,
                        Message = $"File size exceeds maximum allowed size of {options.MaxFileSize / (1024 * 1024)}MB"
                    };
                }

                // فحص امتداد الملف
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (options.AllowedExtensions != null && !options.AllowedExtensions.Contains(fileExtension))
                {
                    return new FileUploadResult
                    {
                        Success = false,
                        Message = $"File extension '{fileExtension}' is not allowed"
                    };
                }

                // فحص نوع الملف
                if (options.AllowedContentTypes != null && !options.AllowedContentTypes.Contains(file.ContentType))
                {
                    return new FileUploadResult
                    {
                        Success = false,
                        Message = $"File type '{file.ContentType}' is not allowed"
                    };
                }

                return new FileUploadResult { Success = true };
            }

            public string GetFileUrl(string fileName, string subPath = null)
            {
                var urlPath = string.IsNullOrEmpty(subPath)
                    ? $"/uploads/{fileName}"
                    : $"/uploads/{subPath}/{fileName}";

                return urlPath;
            }

            private string GenerateUniqueFileName(string originalFileName)
            {
                var extension = Path.GetExtension(originalFileName);
                var fileName = Path.GetFileNameWithoutExtension(originalFileName);
                var uniqueId = Guid.NewGuid().ToString("N")[..8];
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

                return $"{fileName}_{timestamp}_{uniqueId}{extension}";
            }
        }
    }
   
