using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace assignment1
{
    public class WarningRecord
    {
        public static async Task<APIGatewayProxyResponse> InsertWarningRecord(APIGatewayProxyRequest input, ILambdaContext context)
        {
            var data = System.Text.Json.JsonSerializer.Deserialize<WarningRecordModel>(input.Body);
            Console.WriteLine(data.ToString());
            try
            {
                var str = "Server=myresidentialnoot.cm9dthnstss7.us-east-1.rds.amazonaws.com, 1433;Database=Residential Noot;User Id=admin;Password=admin123";
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();

                    var text = $"Insert into WarningRecordModel (WarningInfo, StartTime, EndTime, SecurityID) Values ('{data.WarningInfo}', CONVERT(VARCHAR(25), '{data.StartTime}', 3),CONVERT(VARCHAR(25), '{data.EndTime}', 3), '{data.SecurityID}')";

                    using (SqlCommand cmd2 = new SqlCommand(text, conn))
                    {
                        var rows2 = await cmd2.ExecuteNonQueryAsync();
                        return new APIGatewayProxyResponse()
                        {
                            StatusCode = (int)HttpStatusCode.OK
                        };
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return new APIGatewayProxyResponse()
                {
                    StatusCode = (int)HttpStatusCode.NotFound
                };
            }
        }
        public class WarningRecordModel
        {
            public string WarningInfo { get; set; }

            public string StartTime { get; set; }

            public string EndTime { get; set; }

            public string SecurityID { get; set; }

        }
    }
}
