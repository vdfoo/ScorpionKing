using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Cognitive.CustomVision.Models;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Scorpion.Function
{
    public static class ScorpionFinder
    {
        [FunctionName("ScorpionFinder")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("ScorpionFinder function is triggered through HTTP request");
            NameValueCollection col = req.Content.ReadAsFormDataAsync().Result;

            var image = string.Empty;
            if (col.Count > 0)
            {
                log.Info($"Image text length before replace {col[0].Length}");
                image = col[0]/*.Replace(" ", string.Empty)*/;
                //log.Info($"Image text length after replace {image.Length}");
            }
            else
            {
                log.Error("No content is posted to function.");
            }

            //log.Info("Calling CustomVision using custom Http request");
            //HttpRequestHelper request = new HttpRequestHelper();
            //string response = request.GetCustomVisionResponse(image);

            log.Info("Calling CustomVision using Prediction SDK");
            CustomVisionHelper customVision = new CustomVisionHelper();
            List<ImageTagPrediction> predictions = customVision.GetPrediction(image);
            string tag = string.Empty;
            string percentage = string.Empty;
            if(predictions.Count > 0)
            {
                tag = predictions[0].Tag;
                percentage = (predictions[0].Probability * 100).ToString("0.00");
                log.Info($"Highest probability is tag: '{tag}' with {percentage}% probability.");
            }
            else
            {
                log.Info("Uh-oh... CustomVision couldn't predict anything"); 
            }

            DocumentDbHelper documentDb = new DocumentDbHelper();
            Model.Scorpion scorpion = documentDb.SearchByTag(tag);

            if(scorpion != null)
            {
                scorpion.Confidence = percentage + "%";
            }


            return req.CreateResponse(HttpStatusCode.OK, scorpion, "application/json");
        }
    }
}