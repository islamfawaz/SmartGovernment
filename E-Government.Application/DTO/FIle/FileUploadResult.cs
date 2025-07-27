using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Application.DTO.FIle
{
    public class FileUploadResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileUrl { get; set; }
        public long FileSize { get; set; }
        public string ContentType { get; set; }
    }
}
