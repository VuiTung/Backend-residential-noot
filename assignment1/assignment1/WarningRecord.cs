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
    public class WarningRecord
    {
        public static async Task<APIGatewayProxyResponse> InsertWarningRecord(APIGatewayProxyRequest input, ILambdaContext context)
        {   
            
            var data = System.Text.Json.JsonSerializer.Deserialize<WarningRecordModel>(input.Body);
            try
            {   
                var str = "Server=myresidentialnoot.cm9dthnstss7.us-east-1.rds.amazonaws.com, 1433;Database=Residential Noot;User Id=admin;Password=admin123";
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();

                    var text = $"Insert into WarningRecordModel (WarningInfo, StartTime, EndTime, SecurityID) Values ('{data.WarningInfo}', CONVERT(DATETIME, '{data.StartTime}', 3),CONVERT(DATETIME, '{data.EndTime}', 3), '{data.SecurityID}')";

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

        public static async Task<APIGatewayProxyResponse> RetrieveWarning (APIGatewayProxyRequest input, ILambdaContext context)
        {

            List<WarningRecordModel> WarningList = new List<WarningRecordModel>();
            try
            {
                var str = "Server=myresidentialnoot.cm9dthnstss7.us-east-1.rds.amazonaws.com, 1433;Database=Residential Noot;User Id=admin;Password=admin123";
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();
                    var text = "select * from WarningRecordModel wrm inner join AspNetUsers anu on anu.Id= wrm.SecurityID order by WarningID DESC ";

                    SqlCommand cmd = new SqlCommand(text, conn);
                    var reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        WarningRecordModel WarningRecord = new WarningRecordModel()
                        {
                            WarningID = (int)reader["WarningID"],
                            WarningInfo = (string)reader["WarningInfo"],
                            StartTime = Convert.ToDateTime(reader["StartTime"]).ToString("dd/MM/yy hh:mm:ss t"),
                            EndTime = Convert.ToDateTime(reader["EndTime"]).ToString("dd/MM/yy hh:mm:ss t"),
                            SecurityID = (string)reader["UserName"]
                        };
                        WarningList.Add(WarningRecord);
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
            if (WarningList.Count > 0)
            {

                return new APIGatewayProxyResponse()
                {
                    Body = JsonConvert.SerializeObject(WarningList),
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

        public static async Task<APIGatewayProxyResponse> DeleteWarning(APIGatewayProxyRequest input, ILambdaContext context)
        {
            string WarningID = null;
            input.QueryStringParameters?.TryGetValue("WarningID", out WarningID);
            WarningID = WarningID ?? "3";
            List<WarningRecordModel> WarningRecordList = new List<WarningRecordModel>();

            try
            {
                var str = "Server=myresidentialnoot.cm9dthnstss7.us-east-1.rds.amazonaws.com, 1433;Database=Residential Noot;User Id=admin;Password=admin123";
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();

                    var text = $"delete from WarningRecordModel where WarningID = " + WarningID;

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

        public static async Task<APIGatewayProxyResponse> RetrieveWarningLatest(APIGatewayProxyRequest input, ILambdaContext context)
        {

            List<WarningRecordModel> WarningList = new List<WarningRecordModel>();
            try
            {
                var str = "Server=myresidentialnoot.cm9dthnstss7.us-east-1.rds.amazonaws.com, 1433;Database=Residential Noot;User Id=admin;Password=admin123";
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();
                    var text = "select TOP 1 * from WarningRecordModel where (select dateadd(MINUTE,datepart(tz,cast( getdate() as datetime) AT Time Zone 'Singapore Standard Time'), getdate())) < EndTime order by WarningID DESC";

                    SqlCommand cmd = new SqlCommand(text, conn);
                    var reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        WarningRecordModel WarningRecord = new WarningRecordModel()
                        {
                            WarningID = (int)reader["WarningID"],
                            WarningInfo = (string)reader["WarningInfo"],
                            StartTime = Convert.ToDateTime(reader["StartTime"]).ToString("dd/MM/yy hh:mm:ss t"),
                            EndTime = Convert.ToDateTime(reader["EndTime"]).ToString("dd/MM/yy hh:mm:ss t"),
                            SecurityID = (string)reader["SecurityID"]
                        };
                        WarningList.Add(WarningRecord);
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
            if (WarningList.Count > 0)
            {

                return new APIGatewayProxyResponse()
                {
                    Body = JsonConvert.SerializeObject(WarningList),
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



    }
    public class WarningRecordModel
        {
            public int WarningID { get; set; }

            public string WarningInfo { get; set; }

            public string StartTime { get; set; }

            public string EndTime { get; set; }

            public string SecurityID { get; set; }

        }
}
