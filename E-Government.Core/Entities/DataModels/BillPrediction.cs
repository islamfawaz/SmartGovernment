using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Domain.Entities.DataModels
{
    public class BillPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool PredictedLabel { get; set; }
        [ColumnName("Score")]
        public float Score { get; set; }
        [ColumnName("Probability")]
        public float Probability { get; set; }
        [ColumnName("Features")] 
        public VBuffer<float> Features { get; set; }

        public string PredictionText => PredictedLabel ? "High Bill Expected" : "Normal Bill Expected";
        public float ConfidencePercentage => Probability * 100;

    }
}
