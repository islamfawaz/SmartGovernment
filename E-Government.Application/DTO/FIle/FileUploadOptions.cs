using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Application.DTO.FIle
{
    public class FileUploadOptions
    {
        public string[] AllowedExtensions { get; set; } = { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".doc", ".docx" };
        public string[] AllowedContentTypes { get; set; } = { "image/jpeg", "image/png", "image/gif", "application/pdf" };
        public long MaxFileSize { get; set; } = 5 * 1024 * 1024; // 5MB
        public string UploadPath { get; set; } = "uploads";
        public bool GenerateUniqueFileName { get; set; } = true;
    }
}
