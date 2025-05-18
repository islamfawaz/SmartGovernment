using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Core.Domain.Entities.DataModels
{
   public class BillData
    {
        [LoadColumn(0)] public float BillAmount { get; set; }
        [LoadColumn(1)] public float Consumption { get; set; }
        [LoadColumn(2)] public int BillMonth { get; set; }
        [LoadColumn(3)] public int BillYear { get; set; }
        [LoadColumn(4)] public int DaysInBillingCycle { get; set; }
        [LoadColumn(5)] public float AverageDailyConsumption { get; set; }
        [LoadColumn(6)] public string MeterType { get; set; }
        [LoadColumn(7)] public int NumberOfAirConditioners { get; set; }
        [LoadColumn(8)] public float AirConditionerUsageHours { get; set; }
        [LoadColumn(9)] public string AirConditionerType { get; set; }
        [LoadColumn(10)] public int NumberOfLights { get; set; }
        [LoadColumn(11)] public string LightType { get; set; }
        [LoadColumn(12)] public float LightUsageHours { get; set; }
        [LoadColumn(13)] public int OtherMajorAppliances_Count { get; set; }
        [LoadColumn(14)] public string ApplianceUsage_Encoded { get; set; }
        [LoadColumn(15)] public int HouseholdSize { get; set; }
        [LoadColumn(16)] public string HomeType_Encoded { get; set; }
        [LoadColumn(17)] public float AverageConsumptionLast3Months { get; set; }
        [LoadColumn(18)] public float AverageBillAmountLast3Months { get; set; }
        [LoadColumn(19)] public string ConsumptionTrend { get; set; }
        [LoadColumn(20)] public string SeasonalConsumptionPattern { get; set; }
        [LoadColumn(21)] public bool Label { get; set; }
    }
}
