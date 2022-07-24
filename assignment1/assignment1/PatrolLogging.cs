using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace assignment1
{
    class PatrolLogging
    {

        public static async Task<APIGatewayProxyResponse> InsertPatrolLogging(APIGatewayProxyRequest input, ILambdaContext context)
        {
            var data = System.Text.Json.JsonSerializer.Deserialize<PatrolLoggingModel>(input.Body);
            Console.WriteLine(data.ToString());
            try
            {
                var str = "Server=myresidentialnoot.cm9dthnstss7.us-east-1.rds.amazonaws.com, 1433;Database=Residential Noot;User Id=admin;Password=admin123";
                using (SqlConnection conn = new SqlConnection(str))
                {

                    conn.Open();

                var text = $"Insert into PatrolLoggingModel (SecurityID, Location, LogginData, DataTime) Values ('{data.SecurityID}','{data.Location}','{data.LogginData}',CONVERT(DATETIME, '{data.DataTime}', 3))";

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
        public static async Task<APIGatewayProxyResponse> RetrievePatrolLogging (APIGatewayProxyRequest input, ILambdaContext context)
        {

            List<PatrolLoggingModel> Patrollist = new List<PatrolLoggingModel>();
            try
            {
                var str = "Server=myresidentialnoot.cm9dthnstss7.us-east-1.rds.amazonaws.com, 1433;Database=Residential Noot;User Id=admin;Password=admin123";
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();
                    var text = "select * from PatrolLoggingModel plm inner join AspNetUsers anu on anu.Id= plm.SecurityID order by SecurityID DESC ";

                    SqlCommand cmd = new SqlCommand(text, conn);
                    var reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        PatrolLoggingModel Patrol = new PatrolLoggingModel()
                        {
                            LoggingID = (int)reader["LoggingID"],
                            SecurityID = (string)reader["UserName"],
                            Location = (string)reader["Location"],
                            LogginData = (string)reader["LogginData"],
                            DataTime = Convert.ToDateTime(reader["DataTime"]).ToString("dd/MM/yy hh:mm:ss t")
                        };
                        Patrollist.Add(Patrol);
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());

                return new APIGatewayProxyResponse()
                {
                    StatusCode = (int)HttpStatusCode.NoContent

                };
            }
            if (Patrollist.Count > 0)
            {

                return new APIGatewayProxyResponse()
                {
                    Body = JsonConvert.SerializeObject(Patrollist),
                    StatusCode = (int)HttpStatusCode.OK
                };
            }
            else
            {
                return new APIGatewayProxyResponse()
                {
                    StatusCode = (int)HttpStatusCode.NotFound
                };
            }
        }


        public static async Task<APIGatewayProxyResponse> DeletePatrol(APIGatewayProxyRequest input, ILambdaContext context)
        {
            string LoggingID = null;
            input.QueryStringParameters?.TryGetValue("LoggingID", out LoggingID);
            LoggingID = LoggingID ?? "2";
            List<PatrolLoggingModel> PatrolLoggingList = new List<PatrolLoggingModel>();

            try
            {
                var str = "Server=myresidentialnoot.cm9dthnstss7.us-east-1.rds.amazonaws.com, 1433;Database=Residential Noot;User Id=admin;Password=admin123";
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();

                    var text = $"delete from PatrolLoggingModel where LoggingID = " + LoggingID;

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



    }

    public class PatrolLoggingModel
    {
        public int LoggingID { get; set; }

        public string SecurityID { get; set; }

        public string Location { get; set; }

        public string LogginData { get; set; }

        public string DataTime { get; set; }

    }
}
