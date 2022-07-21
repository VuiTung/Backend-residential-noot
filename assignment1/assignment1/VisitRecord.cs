using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace assignment1
{
    class VisitRecord
    {
        public static async Task<APIGatewayProxyResponse> InsertVisitRecord(APIGatewayProxyRequest input, ILambdaContext context)
        {
            var data = System.Text.Json.JsonSerializer.Deserialize<VisitRecordModel>(input.Body);
            Console.WriteLine(data.ToString());
            try
            {
                var str = "Server=myresidentialnoot.cm9dthnstss7.us-east-1.rds.amazonaws.com, 1433;Database=Residential Noot;User Id=admin;Password=admin123";
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();

                    var text = $"Insert into VisitRecordModel (Reason, EnterTime, LeaveTime) Values ('{data.Reason}',CONVERT(DATETIME, '{data.EnterTime}', 3), CONVERT(DATETIME, '{data.LeaveTime}', 3))";

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
        public static async Task<APIGatewayProxyResponse> RetrieveVisitRecord(APIGatewayProxyRequest input, ILambdaContext context)
        {

            List<VisitRecordModel> VisitRecordList = new List<VisitRecordModel>();
            try
            {
                var str = "Server=myresidentialnoot.cm9dthnstss7.us-east-1.rds.amazonaws.com, 1433;Database=Residential Noot;User Id=admin;Password=admin123";
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();
                    var text = "select * from VisitRecordModel order by VisitorID DESC ";

                    SqlCommand cmd = new SqlCommand(text, conn);
                    var reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        VisitRecordModel VisitRecord = new VisitRecordModel()
                        {
                            VisitorID = (int)reader["VisitorID"],
                            Reason = (string)reader["Reason"],
                            EnterTime = Convert.ToDateTime(reader["EnterTime"]).ToString("dd/MM/yy hh:mm:ss t"),
                            LeaveTime = Convert.ToDateTime(reader["LeaveTime"]).ToString("dd/MM/yy hh:mm:ss t")
                        };
                        VisitRecordList.Add(VisitRecord);
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
            if (VisitRecordList.Count > 0)
            {

                return new APIGatewayProxyResponse()
                {
                    Body = JsonConvert.SerializeObject(VisitRecordList),
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

        public static async Task<APIGatewayProxyResponse> DeleteVisit(APIGatewayProxyRequest input, ILambdaContext context)
        {
            string VisitorID = null;
            input.QueryStringParameters?.TryGetValue("VisitorID", out VisitorID);
            VisitorID = VisitorID ?? "1";
            List<VisitRecordModel> Bookinglist = new List<VisitRecordModel>();

            try
            {
                var str = "Server=myresidentialnoot.cm9dthnstss7.us-east-1.rds.amazonaws.com, 1433;Database=Residential Noot;User Id=admin;Password=admin123";
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();

                    var text = $"delete from VisitRecordModel where VisitorID = " + VisitorID;

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

    public class VisitRecordModel
    {
        public int VisitorID { get; set; }

        public string Reason { get; set; }

        public string EnterTime { get; set; }

        public string LeaveTime { get; set; }

    }
}
