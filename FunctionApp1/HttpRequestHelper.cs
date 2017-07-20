using System;
using System.IO;
using System.Net;

namespace Scorpion.Function
{
    public class HttpRequestHelper
    {
        public string GetCustomVisionResponse(string imageString64)
        {
            WebRequest request = WebRequest.Create("https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/4aefa9bf-ab6f-4616-a1a7-b44069504047/image?iterationId=d59a0782-f0bc-4628-88d5-da9871faec04");
            request.Method = "POST";
            request.Headers.Add("Prediction-Key", "ecf67136b2284c96bfcebd3bd05eec5d");

            byte[] byteArray = Convert.FromBase64String(imageString64);
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse response = request.GetResponse();
            dataStream = response.GetResponseStream();

            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();

            reader.Close();
            dataStream.Close();
            response.Close();

            return responseFromServer;
        }
    }
}
