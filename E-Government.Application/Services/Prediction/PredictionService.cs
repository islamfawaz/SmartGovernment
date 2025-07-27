using Azure.Storage.Blobs;
using E_Government.Application.ServiceContracts;
using E_Government.Domain.Entities.DataModels;
using Microsoft.ML;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace E_Government.Application.Services.Prediction
{
    public class PredictionService : IPredictionService
    {
        private readonly MLContext _mlContext;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<PredictionService> _logger;
        private readonly string _containerName = "models";
        private readonly string _modelBlobName = "BillRecommendationModel.zip";

        private ITransformer _trainedModel;
        private PredictionEngine<BillData, BillPrediction> _predictionEngine;
        private bool _isModelLoaded = false;

        public PredictionService(
            MLContext mlContext,
            BlobServiceClient blobServiceClient,
            ILogger<PredictionService> logger)
        {
            _mlContext = mlContext;
            _blobServiceClient = blobServiceClient;
            _logger = logger;
        }

        public async Task<BillPrediction> PredictAsync(BillData input)
        {
            // تأكد من تحميل الموديل قبل التوقع
            if (!_isModelLoaded)
            {
                await LoadModelFromBlobAsync();
            }

            if (_predictionEngine == null)
                throw new InvalidOperationException("Model not loaded properly");

            return _predictionEngine.Predict(input);
        }

        // Synchronous version for compatibility
        public BillPrediction Predict(BillData input)
        {
            return PredictAsync(input).GetAwaiter().GetResult();
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
                throw new InvalidOperationException("Failed to load ML model from Azure Blob Storage", ex);
            }
        }

        // Method لإعادة تحميل الموديل إذا تم تحديثه
        public async Task ReloadModelAsync()
        {
            _isModelLoaded = false;
            _predictionEngine?.Dispose();
            _trainedModel = null; // مش محتاج dispose

            await LoadModelFromBlobAsync();
        }

        public void Dispose()
        {
            // ITransformer مش محتاج dispose
            _predictionEngine?.Dispose();
        }
    }
}