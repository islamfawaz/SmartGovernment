using Azure.Storage.Blobs;
using E_Government.Application.DTO.AdminDashBoard;
using E_Government.Application.ServiceContracts;
using E_Government.Domain.Entities.DataModels;
using Microsoft.Extensions.Logging;
using Microsoft.ML;

namespace E_Government.Application.Services.Prediction
{
    public class ModelService : IModelService
    {
        private readonly MLContext _mlContext;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<ModelService> _logger;
        private readonly string _containerName = "models";
        private readonly string _modelBlobName = "BillRecommendationModel.zip";

        private ITransformer _trainedModel;
        private PredictionEngine<BillData, BillPrediction> _predictionEngine;
        private bool _isModelLoaded = false;

        public ModelService(
            BlobServiceClient blobServiceClient,
            ILogger<ModelService> logger)
        {
            _mlContext = new MLContext();
            _blobServiceClient = blobServiceClient;
            _logger = logger;
        }

        public async Task<bool> IsBillHighAsync(BillData billData)
        {
            try
            {
                // تأكد من تحميل الموديل قبل التوقع
                if (!_isModelLoaded)
                {
                    await LoadModelFromBlobAsync();
                }

                if (_predictionEngine == null)
                    throw new InvalidOperationException("Model not loaded properly");

                var prediction = _predictionEngine.Predict(billData);
                return prediction.PredictedLabel;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in IsBillHighAsync");
                throw new Exception($"Error in IsBillHighAsync: {ex.Message}");
            }
        }

        public async Task<BillRecommendationDto> GetRecommendationsAsync(BillData input)
        {
            try
            {
                // تأكد من تحميل الموديل قبل التوقع
                if (!_isModelLoaded)
                {
                    await LoadModelFromBlobAsync();
                }

                if (_predictionEngine == null)
                    throw new InvalidOperationException("Model not loaded properly");

                var basePrediction = _predictionEngine.Predict(input);

                // حساب Feature Importance
                var featureImportance = CalculateRealFeatureImportance(basePrediction, _predictionEngine);

                var result = new BillRecommendationDto
                {
                    IsHighBill = basePrediction.PredictedLabel,
                    BillAmount = input.BillAmount,
                    Consumption = input.Consumption,
                    FeatureImportance = featureImportance,
                    Recommendations = basePrediction.PredictedLabel ? GenerateRecommendations(input) : new List<string>()
                };

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetRecommendationsAsync");
                throw new Exception($"Error in GetRecommendationsAsync: {ex.Message}");
            }
        }

        private async Task LoadModelFromBlobAsync()
        {
            try
            {
                _logger.LogInformation("Loading ML model from Azure Blob Storage...");

                // التحقق من وجود الـ container
                var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
                var containerExists = await containerClient.ExistsAsync();

                if (!containerExists.Value)
                {
                    throw new InvalidOperationException($"Container '{_containerName}' does not exist in blob storage");
                }

                // تحميل الموديل من Blob
                var blobClient = containerClient.GetBlobClient(_modelBlobName);
                var blobExists = await blobClient.ExistsAsync();

                if (!blobExists.Value)
                {
                    throw new InvalidOperationException($"Model blob '{_modelBlobName}' does not exist in container '{_containerName}'");
                }

                using var stream = new MemoryStream();
                await blobClient.DownloadToAsync(stream);
                stream.Position = 0;

                // تحميل الموديل من الـ stream
                _trainedModel = _mlContext.Model.Load(stream, out var modelSchema);
                _predictionEngine = _mlContext.Model.CreatePredictionEngine<BillData, BillPrediction>(_trainedModel);

                _isModelLoaded = true;
                _logger.LogInformation("ML model loaded successfully from blob storage");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load ML model from blob storage");
                throw new InvalidOperationException($"Failed to load ML model from Azure Blob Storage: {ex.Message}", ex);
            }
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
        /// <summary>
        /// إنشاء التوصيات المخصصة بناءً على تحليل مفصل لبيانات الفاتورة
        /// </summary>
        private List<string> GenerateRecommendations(BillData data)
        {
            var recommendations = new List<string>();
            var priorityRecommendations = new List<string>(); // توصيات عالية الأولوية

            // تحليل استخدام التكييف - الأولوية الأولى
            if (data.AirConditionerUsageHours > 16)
            {
                priorityRecommendations.Add("🌡️ تكييفك شغال أكتر من 16 ساعة يومياً! جرب ترفع الحرارة درجة واحدة لتوفير حتى 10% من الفاتورة.");
                priorityRecommendations.Add("⏰ استخدم التايمر لإطفاء التكييف قبل النوم بساعة - الغرفة هتفضل باردة.");
            }
            else if (data.AirConditionerUsageHours > 12)
            {
                recommendations.Add("🌡️ حاول تقليل ساعات التكييف أو ضبط الحرارة على 24-25 درجة بدلاً من أقل.");
            }
            else if (data.AirConditionerUsageHours > 8)
            {
                recommendations.Add("💨 استخدم المراوح مع التكييف لتوزيع الهواء البارد بشكل أفضل.");
            }

            // تحليل عدد المكيفات
            if (data.NumberOfAirConditioners > 4)
            {
                priorityRecommendations.Add("❄️ عندك أكتر من 4 مكيفات! شغل بس اللي في الغرف المستخدمة واقفل الباقي.");
                if (data.AirConditionerType == "Central")
                {
                    recommendations.Add("🏠 التكييف المركزي يستهلك طاقة أكتر - تأكد من إغلاق فتحات الهواء في الغرف الفارغة.");
                }
            }
            else if (data.NumberOfAirConditioners > 2)
            {
                recommendations.Add("❄️ حاول تشغيل مكيف واحد بس في الغرفة اللي انت فيها.");
            }

            // تحليل نوع التكييف
            if (data.AirConditionerType == "Window")
            {
                recommendations.Add("🪟 مكيفات الشباك تستهلك طاقة أكتر - فكر في التطوير لمكيف سبليت موفر للطاقة.");
            }
            else if (data.AirConditionerType == "Central")
            {
                recommendations.Add("🏠 التكييف المركزي قوي بس محتاج صيانة دورية لضمان الكفاءة.");
            }

            // تحليل الإضاءة
            if (data.LightUsageHours > 12)
            {
                priorityRecommendations.Add("💡 الإضاءة شغالة أكتر من 12 ساعة يومياً! اطفي الأنوار في الغرف اللي مش مستخدمة.");
            }
            else if (data.LightUsageHours > 8)
            {
                recommendations.Add("💡 حاول تقليل ساعات الإضاءة واستخدم الضوء الطبيعي في النهار.");
            }

            // تحليل نوع اللمبات
            if (data.LightType == "Incandescent")
            {
                priorityRecommendations.Add("💡 اللمبات التقليدية بتستهلك طاقة كتير! غيرها بـ LED هتوفر حتى 80% من كهربا الإضاءة.");
            }
            else if (data.LightType == "Fluorescent")
            {
                recommendations.Add("💡 لمبات الفلورسنت كويسة بس LED أوفر وأطول عمراً.");
            }
            else if (data.LightType == "Halogen")
            {
                priorityRecommendations.Add("💡 لمبات الهالوجين بتولد حرارة وتستهلك كتير - استبدلها بـ LED فوراً!");
            }

            // تحليل عدد اللمبات
            if (data.NumberOfLights > 30)
            {
                recommendations.Add("💡 عندك إضاءة كتيرة! استبدل اللمبات الأقل استخداماً بـ LED وحط مفاتيح منفصلة.");
            }

            // تحليل الأجهزة الكبيرة
            if (data.OtherMajorAppliances_Count > 7)
            {
                priorityRecommendations.Add("🔌 عندك أجهزة كتيرة! افصل اللي مش بتستخدمها من الكهربا تماماً (مش بس إقفال).");
                recommendations.Add("⚡ الأجهزة في وضع الـ Standby لسه بتستهلك كهربا - افصلها من المفيش.");
            }
            else if (data.OtherMajorAppliances_Count > 5)
            {
                recommendations.Add("🔌 حاول تقليل تشغيل الأجهزة الكبيرة في نفس الوقت لتوزيع الاستهلاك.");
            }

            // تحليل حجم الأسرة
            if (data.HouseholdSize > 6)
            {
                recommendations.Add("👨‍👩‍👧‍👦 الأسرة الكبيرة محتاجة تنظيم: اتفقوا على مواعيد استخدام الأجهزة عالية الاستهلاك.");
                recommendations.Add("🏠 جربوا تجمعوا في غرفة واحدة في أوقات معينة لتوفير التكييف والإضاءة.");
            }
            else if (data.HouseholdSize > 4)
            {
                recommendations.Add("👨‍👩‍👧‍👦 علموا الأطفال عادات توفير الطاقة - اطفوا الأنوار والأجهزة بعد الاستخدام.");
            }

            // تحليل نوع المسكن
            if (data.HomeType_Encoded == "Villa")
            {
                recommendations.Add("🏠 الفيلا محتاجة عزل حراري جيد - تأكد من النوافذ والأبواب محكمة الإغلاق.");
                recommendations.Add("🌿 ازرع نباتات حول البيت لتقليل الحرارة وتوفير التكييف.");
            }

            // تحليل الشهر (موسمي)
            if (data.BillMonth >= 6 && data.BillMonth <= 9) // أشهر الصيف
            {
                recommendations.Add("☀️ في الصيف: استخدم الستائر العاتمة وتجنب فتح النوافذ في النهار.");
                recommendations.Add("🌙 شغل الأجهزة الكبيرة في الليل لما الكهربا أرخص والحرارة أقل.");
            }
            else if (data.BillMonth >= 12 || data.BillMonth <= 2) // أشهر الشتاء
            {
                recommendations.Add("❄️ في الشتاء: استخدم الضوء الطبيعي أكتر وقلل التدفئة.");
            }

            // تحليل الاتجاه العام للاستهلاك
            if (data.ConsumptionTrend == "Increasing")
            {
                priorityRecommendations.Add("📈 استهلاكك في زيادة مستمرة! لازم تتخذ إجراءات فورية لتوفير الطاقة.");
                recommendations.Add("📊 راقب استهلاكك اليومي وحدد الأجهزة اللي بتستهلك أكتر.");
            }

            // تحليل النمط الموسمي
            if (data.SeasonalConsumptionPattern == "HigherThanUsual")
            {
                recommendations.Add("⚠️ استهلاكك أعلى من المعتاد للموسم ده - دور على السبب وراجع عاداتك.");
            }

            // دمج التوصيات حسب الأولوية
            var finalRecommendations = new List<string>();
            finalRecommendations.AddRange(priorityRecommendations.Take(3)); // أهم 3 توصيات
            finalRecommendations.AddRange(recommendations.Take(4)); // 4 توصيات إضافية

            // إضافة توصية عامة بناءً على مستوى الاستهلاك
            if (data.Consumption > 1000)
            {
                finalRecommendations.Insert(0, "🚨 استهلاكك عالي جداً! طبق هذه النصائح فوراً لتوفير حتى 30% من فاتورتك.");
            }
            else if (data.Consumption > 700)
            {
                finalRecommendations.Insert(0, "⚡ استهلاكك فوق المتوسط - تطبيق هذه النصائح هيوفرلك مبلغ كويس.");
            }

            return finalRecommendations.Distinct().Take(6).ToList(); // أهم 6 توصيات مختلفة
        }
        // Method لإعادة تحميل الموديل إذا تم تحديثه
        public async Task ReloadModelAsync()
        {
            _isModelLoaded = false;
            _predictionEngine?.Dispose();
            _trainedModel = null;

            await LoadModelFromBlobAsync();
        }

        public void Dispose()
        {
            _predictionEngine?.Dispose();
        }
    }
}