using E_Government.Core.Domain.Entities.DataModels;
using E_Government.Core.ServiceContracts;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Application.Services.Prediction
{
   public class PredictionService :IPredictionService
    {
        private readonly MLContext _mlContext;
        private ITransformer _trainedModel;
        private PredictionEngine<BillData, BillPrediction> _predictionEngine;

        private static string ModelPath = @"C:\Users\DELL\source\repos\SmartGovernment\E-Government.Core\Domain\Entities\DataModels\Model\BillRecommendationModel.zip";

        public PredictionService(MLContext mlContext)
        {
            _mlContext = mlContext;
            LoadModel();
        }

        private void LoadModel()
        {
            using var fileStream = File.OpenRead(ModelPath);
            _trainedModel = _mlContext.Model.Load(fileStream, out var modelSchema);
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<BillData, BillPrediction>(_trainedModel);
        }

        public BillPrediction Predict(BillData input)
        {
            return _predictionEngine.Predict(input);
        }
    }
}
