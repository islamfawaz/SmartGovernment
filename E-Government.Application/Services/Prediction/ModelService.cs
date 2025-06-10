using E_Government.Application.DTO.AdminDashBoard;
using E_Government.Application.ServiceContracts;
using E_Government.Domain.Entities.DataModels;
using E_Government.Domain.ServiceContracts;
using Microsoft.ML;

namespace E_Government.Application.Services.Prediction
{
    public class ModelService : IModelService
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _model;

        public ModelService()
        {
            _mlContext = new MLContext();
            _model = LoadModel();
        }

        private ITransformer LoadModel()
        {
            var modelPath = @"C:\Users\DELL\source\repos\SmartGovernment\E-Government.Core\Domain\Entities\DataModels\Model\BillRecommendationModel.zip";
            if (!File.Exists(modelPath))
                throw new FileNotFoundException("Model file not found.");

            using var fileStream = File.OpenRead(modelPath);
            return _mlContext.Model.Load(fileStream, out _);
        }

        public Task<bool> IsBillHighAsync(BillData billData)
        {
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<BillData, BillPrediction>(_model);
            var prediction = predictionEngine.Predict(billData);

            return Task.FromResult(prediction.PredictedLabel);
        }

        public async Task<BillRecommendationDto> GetRecommendationsAsync(BillData input)
        {
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<BillData, BillPrediction>(_model);
            var basePrediction = predictionEngine.Predict(input);

            // حساب Feature Importance
            var featureImportance = CalculateRealFeatureImportance(basePrediction, predictionEngine);

            var result = new BillRecommendationDto
            {
                IsHighBill = basePrediction.PredictedLabel,
                BillAmount = input.BillAmount,
                Consumption = input.Consumption,
                FeatureImportance = featureImportance,
                Recommendations = basePrediction.PredictedLabel ? GenerateRecommendations(input) : new List<string>()
            };

            return await Task.FromResult(result);
        }

        /// <summary>
        /// تحسب أهمية الميزات باستخدام القيم من Features vector
        /// </summary>
        private Dictionary<string, float> CalculateRealFeatureImportance(BillPrediction prediction, PredictionEngine<BillData, BillPrediction> engine)
        {
            var importanceDict = new Dictionary<string, float>();

            var features = prediction.Features.GetValues().ToArray();

            var featureNames = new[]
            {
                "BillMonth",
                "BillYear",
                "DaysInBillingCycle",
                "MeterTypeEncoded",
                "NumberOfAirConditionersFloat",
                "AirConditionerUsageHours",
                "AirConditionerTypeEncoded",
                "NumberOfLightsFloat",
                "LightTypeEncoded",
                "LightUsageHours",
                "OtherMajorAppliances_CountFloat",
                "ApplianceUsage_EncodedFloat",
                "HouseholdSizeFloat",
                "HomeTypeEncoded",
                "ConsumptionTrendEncoded",
                "SeasonalConsumptionPatternEncoded"
            };

            for (int i = 0; i < features.Length && i < featureNames.Length; i++)
            {
                importanceDict[featureNames[i]] = Math.Abs(features[i]);
            }

            return importanceDict;
        }

        /// <summary>
        /// إنشاء التوصيات عند اكتشاف فاتورة مرتفعة
        /// </summary>
        private List<string> GenerateRecommendations(BillData data)
        {
            var recommendations = new List<string>();

            if (data.AirConditionerUsageHours > 8)
                recommendations.Add("حاول تقليل تشغيل التكييف أو ضبط درجة الحرارة لتوفير الطاقة.");

            if (data.LightUsageHours > 6)
                recommendations.Add("لا تنسَ إطفاء الأنوار في الغرف غير المستخدمة.");

            if (data.OtherMajorAppliances_Count > 3)
                recommendations.Add("فصل الأجهزة غير المستخدمة يمكن أن يساعد في خفض الفاتورة.");

            if (data.HouseholdSize > 4)
                recommendations.Add("اتبع عادات توفير الطاقة في المنازل ذات الأحجام الكبيرة.");

            return recommendations;
        }
    }
}