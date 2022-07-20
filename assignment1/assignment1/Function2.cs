using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace assignment1
{
    class Function2
    {
        public List<WeatherForecast> FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
        {
            Console.WriteLine(JsonSerializer.Serialize(input));
            string cityName = null;
            input.QueryStringParameters?.TryGetValue("cityName", out cityName);
            cityName = cityName ?? "Tawau";

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                City = $"{cityName}-test",
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
            })
            .ToList();
        }
        public class WeatherForecast
        {
            public string City { get; set; }
            public DateTime Date { get; set; }

            public int TemperatureC { get; set; }

            public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        }
    }
}
