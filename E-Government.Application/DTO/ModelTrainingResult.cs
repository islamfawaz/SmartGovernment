using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Application.DTO
{
    public class ModelTrainingResult
    {
        public bool IsSuccess { get; set; }
        public double? Accuracy { get; set; }
        public double? AUC { get; set; }
        public double? F1Score { get; set; }
        public double? RSquared { get; set; }
        public double? MeanAbsoluteError { get; set; }
        public string Message { get; set; }
        public string ModelPath { get; set; }

    }
}
