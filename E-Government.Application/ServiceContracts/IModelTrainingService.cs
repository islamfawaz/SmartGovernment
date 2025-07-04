using Azure.Storage.Blobs;
using E_Government.Application.DTO;
using E_Government.Domain.Entities.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Government.Application.ServiceContracts
{
    public interface IModelTrainingService
    {
        Task<ModelTrainingResult> TrainAndSaveModelAsync(IEnumerable<BillData> trainingData);
        Task<ModelTrainingResult> TrainRegressionModelAsync(IEnumerable<BillData> trainingData);
        IEnumerable<BillData> GenerateSampleTrainingData();






    }
}
