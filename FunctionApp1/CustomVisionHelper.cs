using Microsoft.Cognitive.CustomVision;
using Microsoft.Cognitive.CustomVision.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Scorpion.Function
{
    public class CustomVisionHelper
    {
        private const string TrainingKey = "954c3b6410a8427098091af236c51ccc";
        private const string ProjectId = "4aefa9bf-ab6f-4616-a1a7-b44069504047";

        public List<ImageTagPrediction> GetPrediction(string imageString64)
        {
            TrainingApiCredentials trainingCredentials = new TrainingApiCredentials(TrainingKey);
            TrainingApi trainingApi = new TrainingApi(trainingCredentials);

            Guid projectId = Guid.Parse(ProjectId);
            var project = trainingApi.GetProject(projectId);
            var account = trainingApi.GetAccountInfo();
            var predictionKey = account.Keys.PredictionKeys.PrimaryKey;

            PredictionEndpointCredentials predictionEndpointCredentials = new PredictionEndpointCredentials(predictionKey);
            PredictionEndpoint endpoint = new PredictionEndpoint(predictionEndpointCredentials);

            using (var imageStream = GetImageStream(imageString64))
            {
                var result = endpoint.PredictImage(project.Id, imageStream);
                return result.Predictions.ToList();
            }
        }

        public static Stream GetImageStream(string imageString64)
        {
            byte[] byteArray = Convert.FromBase64String(imageString64);
            MemoryStream dataStream = new MemoryStream(byteArray);
            return dataStream;
        }
    }
}
