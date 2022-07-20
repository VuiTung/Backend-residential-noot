using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace assignment1
{
    public class Function
    {
        private static readonly string[] Summaries = new[]
          {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public List<WeatherForecast> FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
        {
            Console.WriteLine(JsonSerializer.Serialize(input));
            string cityName = null;
            input.QueryStringParameters?.TryGetValue("cityName", out cityName);
            cityName = cityName ?? "Brisbane";

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                City = $"{cityName}-test",
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToList();
        }

        public APIGatewayProxyResponse FunctionHandlerPost(APIGatewayProxyRequest input, ILambdaContext context)
        {
            var data = JsonSerializer.Deserialize<WeatherForecast>(input.Body);

            string cityName = "Brisbane";
            input.PathParameters?.TryGetValue("cityName", out cityName);

            data.City = cityName;
            return new APIGatewayProxyResponse()
            {
                Body = JsonSerializer.Serialize(data),
                StatusCode = (int)HttpStatusCode.OK
            };
        }
        public class WeatherForecast
        {
            public string City { get; set; }
            public DateTime Date { get; set; }

            public int TemperatureC { get; set; }

            public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

            public string Summary { get; set; }
        }
    }
}
