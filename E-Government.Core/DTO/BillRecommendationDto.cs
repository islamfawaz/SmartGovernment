using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.DTO
{
   public class BillRecommendationDto
    {
        public bool IsHighBill { get; set; }
        public float BillAmount { get; set; }
        public float Consumption { get; set; }
        public List<string> Recommendations { get; set; } = new List<string>();
        public Dictionary<string, float> FeatureImportance { get; set; } = new Dictionary<string, float>();

    }
}
