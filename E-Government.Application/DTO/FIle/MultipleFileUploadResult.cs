using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Application.DTO.FIle
{
    public class MultipleFileUploadResult
    {
        public List<FileUploadResult> Results { get; set; } = new List<FileUploadResult>();
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
    }
}
