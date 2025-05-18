using E_Government.Core.Domain.Entities.DataModels;
using MathNet.Numerics.Distributions;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers.LightGbm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace E_Government.ModelTrainer
{
    class Program
    {
        private static string DataPath = @"C:\Users\DELL\source\repos\SmartGovernment\E-Government.ModelTrainer\Data\sample_bill_data.csv";
        private static string ModelPath = @"C:\Users\DELL\source\repos\SmartGovernment\E-Government.Core\Domain\Entities\DataModels\Model\BillRecommendationModel.zip";

        // Box-Muller Transform - لتوليد ضوضاء غاوسية بدون MathNet.Numerics
        static void Main(string[] args)
        {
            var mlContext = new MLContext(seed: 42);
            try
            {
                Console.WriteLine("Loading data...");
                IDataView data = LoadDataWithErrorHandling(mlContext);
                if (data == null)
                {
                    Console.WriteLine("Failed to load data. Exiting...");
                    return;
                }

                Console.WriteLine("\nAnalyzing data for leakage...");
                AnalyzeDataForLeakage(mlContext, data);

                Console.WriteLine("\nProcessing data to reduce leakage...");
                data = AddNoise(mlContext, data);

                Console.WriteLine("\nSplitting data randomly...");
                var splitData = mlContext.Data.TrainTestSplit(data, testFraction: 0.2, seed: 42);
                var trainRows = mlContext.Data.CreateEnumerable<BillData>(splitData.TrainSet, false).Count();
                var testRows = mlContext.Data.CreateEnumerable<BillData>(splitData.TestSet, false).Count();

                Console.WriteLine($"Training data size: {trainRows}");
                Console.WriteLine($"Test data size: {testRows}");

                Console.WriteLine("\nBuilding data processing pipeline...");
                var dataProcessPipeline = mlContext.Transforms.Categorical.OneHotEncoding(new[]
                {
                    new InputOutputColumnPair("MeterTypeEncoded", nameof(BillData.MeterType)),
                    new InputOutputColumnPair("AirConditionerTypeEncoded", nameof(BillData.AirConditionerType)),
                    new InputOutputColumnPair("LightTypeEncoded", nameof(BillData.LightType)),
                    new InputOutputColumnPair("HomeTypeEncoded", nameof(BillData.HomeType_Encoded)),
                    new InputOutputColumnPair("ConsumptionTrendEncoded", nameof(BillData.ConsumptionTrend)),
                    new InputOutputColumnPair("SeasonalConsumptionPatternEncoded", nameof(BillData.SeasonalConsumptionPattern)),
                })
                .Append(mlContext.Transforms.Conversion.ConvertType(new[]
                {
                    new InputOutputColumnPair("BillMonthFloat", nameof(BillData.BillMonth)),
                    new InputOutputColumnPair("BillYearFloat", nameof(BillData.BillYear)),
                    new InputOutputColumnPair("DaysInBillingCycleFloat", nameof(BillData.DaysInBillingCycle)),
                    new InputOutputColumnPair("NumberOfAirConditionersFloat", nameof(BillData.NumberOfAirConditioners)),
                    new InputOutputColumnPair("NumberOfLightsFloat", nameof(BillData.NumberOfLights)),
                    new InputOutputColumnPair("OtherMajorAppliances_CountFloat", nameof(BillData.OtherMajorAppliances_Count)),
                    new InputOutputColumnPair("HouseholdSizeFloat", nameof(BillData.HouseholdSize)),
                    new InputOutputColumnPair("ApplianceUsage_EncodedFloat", nameof(BillData.ApplianceUsage_Encoded)),
                }, DataKind.Single))
                .Append(mlContext.Transforms.Concatenate("Features",
                    "BillMonthFloat",
                    "BillYearFloat",
                    "DaysInBillingCycleFloat",
                    "MeterTypeEncoded",
                    "NumberOfAirConditionersFloat",
                    nameof(BillData.AirConditionerUsageHours),
                    "AirConditionerTypeEncoded",
                    "NumberOfLightsFloat",
                    "LightTypeEncoded",
                    nameof(BillData.LightUsageHours),
                    "OtherMajorAppliances_CountFloat",
                    "ApplianceUsage_EncodedFloat",
                    "HouseholdSizeFloat",
                    "HomeTypeEncoded",
                    "ConsumptionTrendEncoded",
                    "SeasonalConsumptionPatternEncoded"))
                .Append(mlContext.Transforms.NormalizeMinMax("Features"));

                Console.WriteLine("\nConfiguring LightGBM settings...");
                var lightGbmOption = new LightGbmBinaryTrainer.Options
                {
                    LabelColumnName = "Label",
                    FeatureColumnName = "Features",
                    NumberOfLeaves = 20,
                    NumberOfIterations = 100,
                    MinimumExampleCountPerLeaf = 20,
                    LearningRate = 0.1,
                    MaximumBinCountPerFeature = 255
                };

                var trainer = mlContext.BinaryClassification.Trainers.LightGbm(lightGbmOption);
                var calibratedTrainer = trainer.Append(mlContext.BinaryClassification.Calibrators.Platt(labelColumnName: "Label"));
                var trainingPipeline = dataProcessPipeline.Append(calibratedTrainer);

                Console.WriteLine("\nPerforming Cross-Validation...");
                var cvResults = mlContext.BinaryClassification.CrossValidate(
                    data: splitData.TrainSet,
                    estimator: trainingPipeline,
                    numberOfFolds: 5,
                    labelColumnName: "Label");

                Console.WriteLine("Cross-Validation Results:");
                Console.WriteLine($"Average Accuracy: {cvResults.Average(r => r.Metrics.Accuracy):P2}");
                Console.WriteLine($"Average AUC: {cvResults.Average(r => r.Metrics.AreaUnderRocCurve):P2}");
                Console.WriteLine($"Average F1 Score: {cvResults.Average(r => r.Metrics.F1Score):P2}");

                Console.WriteLine("\nTraining the final model...");
                var trainedModel = trainingPipeline.Fit(splitData.TrainSet);

                Console.WriteLine("\nEvaluating model on Test Set...");
                if (testRows > 0)
                {
                    var predictions = trainedModel.Transform(splitData.TestSet);
                    var metrics = mlContext.BinaryClassification.Evaluate(predictions, labelColumnName: "Label");
                    Console.WriteLine("Test Set Metrics:");
                    Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
                    Console.WriteLine($"AUC: {metrics.AreaUnderRocCurve:P2}");
                    Console.WriteLine($"F1 Score: {metrics.F1Score:P2}");
                    Console.WriteLine($"Positive Precision: {metrics.PositivePrecision:P2}");
                    Console.WriteLine($"Positive Recall: {metrics.PositiveRecall:P2}");
                    Console.WriteLine($"Negative Precision: {metrics.NegativePrecision:P2}");
                    Console.WriteLine($"Negative Recall: {metrics.NegativeRecall:P2}");

                    Console.WriteLine("\nConfusion Matrix:");
                    var cm = metrics.ConfusionMatrix;
                    Console.WriteLine($"True Positives: {cm.Counts[1][1]}");
                    Console.WriteLine($"False Positives: {cm.Counts[0][1]}");
                    Console.WriteLine($"True Negatives: {cm.Counts[0][0]}");
                    Console.WriteLine($"False Negatives: {cm.Counts[1][0]}");
                }
                else
                {
                    Console.WriteLine("Skipping evaluation: Test Set is empty.");
                }

                Console.WriteLine("\nSaving the model...");
                Directory.CreateDirectory(Path.GetDirectoryName(ModelPath));
                mlContext.Model.Save(trainedModel, splitData.TrainSet.Schema, ModelPath);
                Console.WriteLine($"Model saved to: {ModelPath}");

                Console.WriteLine("\nListing features used...");
                ListFeaturesUsed();

                Console.WriteLine("\nAnalyzing sample predictions...");
                AnalyzeSamplePredictions(mlContext, trainedModel, splitData.TestSet);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner error: {ex.InnerException.Message}");
                }
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        private static IDataView LoadDataWithErrorHandling(MLContext mlContext)
        {
            try
            {
                var textLoader = mlContext.Data.CreateTextLoader<BillData>(
                    separatorChar: ',',
                    hasHeader: true,
                    allowQuoting: true);
                var data = textLoader.Load(DataPath);
                var rowCount = mlContext.Data.CreateEnumerable<BillData>(data, reuseRowObject: false).Count();
                Console.WriteLine($"Successfully loaded {rowCount} rows from {DataPath}");
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
                return null;
            }
        }

        private static void AnalyzeDataForLeakage(MLContext mlContext, IDataView data)
        {
            var dataEnumerable = mlContext.Data.CreateEnumerable<BillData>(data, reuseRowObject: false).ToList();
            var positiveExamples = dataEnumerable.Where(x => x.Label).ToList();
            var negativeExamples = dataEnumerable.Where(x => !x.Label).ToList();

            Console.WriteLine($"Total samples: {dataEnumerable.Count}");
            Console.WriteLine($"Positive samples: {positiveExamples.Count} ({(double)positiveExamples.Count / dataEnumerable.Count:P2})");
            Console.WriteLine($"Negative samples: {negativeExamples.Count} ({(double)negativeExamples.Count / dataEnumerable.Count:P2})");

            Console.WriteLine("\nChecking key features for strong correlations...");

            var acUsagePositive = positiveExamples.Select(x => x.AirConditionerUsageHours).ToList();
            var acUsageNegative = negativeExamples.Select(x => x.AirConditionerUsageHours).ToList();

            Console.WriteLine($"AC Usage Hours (True): Min={acUsagePositive.Min():F2}, Max={acUsagePositive.Max():F2}, Avg={acUsagePositive.Average():F2}");
            Console.WriteLine($"AC Usage Hours (False): Min={acUsageNegative.Min():F2}, Max={acUsageNegative.Max():F2}, Avg={acUsageNegative.Average():F2}");

            var consumptionPositive = positiveExamples.Select(x => x.Consumption).ToList();
            var consumptionNegative = negativeExamples.Select(x => x.Consumption).ToList();

            Console.WriteLine($"Consumption (True): Min={consumptionPositive.Min():F2}, Max={consumptionPositive.Max():F2}, Avg={consumptionPositive.Average():F2}");
            Console.WriteLine($"Consumption (False): Min={consumptionNegative.Min():F2}, Max={consumptionNegative.Max():F2}, Avg={consumptionNegative.Average():F2}");
        }

        private static IDataView AddNoise(MLContext mlContext, IDataView data)
        {
            var dataList = mlContext.Data.CreateEnumerable<BillData>(data, false).ToList();
            var rng = new Random(42);

            var gaussianAc = new Normal(0, 0.5) { RandomSource = rng };
            var gaussianCons = new Normal(0, 10.0) { RandomSource = rng };
            var gaussianBill = new Normal(0, 5.0) { RandomSource = rng };

            foreach (var item in dataList)
            {
                item.AirConditionerUsageHours = (float)Math.Max(0, item.AirConditionerUsageHours + gaussianAc.Sample());
                item.Consumption = (float)Math.Max(0, item.Consumption + gaussianCons.Sample());
                item.BillAmount = (float)Math.Max(0, item.BillAmount + gaussianBill.Sample());

                // تداخل بسيط للبيانات لتقليل Overfitting
                if (rng.NextDouble() < 0.7)
                {
                    if (item.Label && item.AirConditionerUsageHours > 7)
                        item.AirConditionerUsageHours = (float)(rng.NextDouble() * 5 + 2);
                    else if (!item.Label && item.AirConditionerUsageHours < 7)
                        item.AirConditionerUsageHours = (float)(rng.NextDouble() * 5 + 7);
                }
            }

            Console.WriteLine("\nRe-analyzing data after adding noise...");
            AnalyzeDataForLeakage(mlContext, mlContext.Data.LoadFromEnumerable(dataList));
            return mlContext.Data.LoadFromEnumerable(dataList);
        }
        // Box-Muller Transform


        private static void ListFeaturesUsed()
        {
            Console.WriteLine("Features used in the model:");
            var featureNames = new string[]
            {
                "BillMonth",
                "BillYear",
                "DaysInBillingCycle",
                "MeterTypeEncoded",
                "NumberOfAirConditioners",
                "AirConditionerUsageHours",
                "AirConditionerTypeEncoded",
                "NumberOfLights",
                "LightTypeEncoded",
                "LightUsageHours",
                "OtherMajorAppliances_Count",
                "ApplianceUsage_Encoded",
                "HouseholdSize",
                "HomeTypeEncoded",
                "ConsumptionTrendEncoded",
                "SeasonalConsumptionPatternEncoded"
            };
            foreach (var name in featureNames)
            {
                Console.WriteLine($"- {name}");
            }
        }

        private static void AnalyzeSamplePredictions(MLContext mlContext, ITransformer model, IDataView testData)
        {
            var predictionEngine = mlContext.Model.CreatePredictionEngine<BillData, BillPrediction>(model);
            var testSamples = mlContext.Data.CreateEnumerable<BillData>(testData, reuseRowObject: false).Take(5).ToList();

            Console.WriteLine("\nAnalyzing sample predictions:");
            if (testSamples.Count == 0)
            {
                Console.WriteLine("No test samples available for prediction analysis.");
                return;
            }

            foreach (var sample in testSamples)
            {
                var prediction = predictionEngine.Predict(sample);
                Console.WriteLine($"\nAC Usage Hours: {sample.AirConditionerUsageHours:F1}, Consumption: {sample.Consumption:F1}, Bill Amount: {sample.BillAmount:F1}");
                Console.WriteLine($"True Label: {sample.Label}");
                Console.WriteLine($"Predicted Label: {prediction.PredictedLabel}");
                Console.WriteLine($"Probability: {prediction.Probability:F4}");
                Console.WriteLine($"Score: {prediction.Score:F4}");
            }
        }
    }
}