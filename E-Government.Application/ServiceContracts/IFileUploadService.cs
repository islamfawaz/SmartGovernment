using E_Government.Application.DTO.FIle;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Application.ServiceContracts
{
    public interface IFileUploadService
    {
        Task<FileUploadResult> UploadFileAsync(IFormFile file, FileUploadOptions options = null);
        Task<MultipleFileUploadResult> UploadMultipleFilesAsync(IFormFileCollection files, FileUploadOptions options = null);
        Task<bool> DeleteFileAsync(string fileName, string subPath = null);
        bool FileExists(string fileName, string subPath = null);
        FileUploadResult ValidateFile(IFormFile file, FileUploadOptions options);
        string GetFileUrl(string fileName, string subPath = null);
    }
}
