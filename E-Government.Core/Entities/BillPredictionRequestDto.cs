namespace E_Government.Domain.Entities
{

   public class BillPredictionRequestDto
    {

        public float BillAmount { get; set; }
        public float Consumption { get; set; }
        public int BillMonth { get; set; }
        public int BillYear { get; set; }
        public int DaysInBillingCycle { get; set; }

        public string MeterType { get; set; } = string.Empty;
        public int NumberOfAirConditioners { get; set; }
        public float AirConditionerUsageHours { get; set; }
        public string AirConditionerType { get; set; } = string.Empty;

        public int NumberOfLights { get; set; }
        public string LightType { get; set; } = string.Empty;
        public float LightUsageHours { get; set; }

        public int OtherMajorAppliances_Count { get; set; }
        public string ApplianceUsage_Encoded { get; set; } = string.Empty;
        public int HouseholdSize { get; set; }
        public string HomeType_Encoded { get; set; } = string.Empty;
        public string ConsumptionTrend { get; set; } = string.Empty;
        public string SeasonalConsumptionPattern { get; set; } = string.Empty;
    }
}
