using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.DTO
{
    public class ChartDatasetDto
    {
        public string Label { get; set; }
        public List<object> Data { get; set; }
        public string BackgroundColor { get; set; } // Optional: for styling
        public string BorderColor { get; set; } // Optional: for styling
        public bool Fill { get; set; } // Optional
    }
}
