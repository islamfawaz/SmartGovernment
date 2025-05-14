using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.DTO
{
    // DTOs for Chart.js
    public class ChartDataDto
    {
        public List<string> Labels { get; set; }
        public List<ChartDatasetDto> Datasets { get; set; }
    }
}
