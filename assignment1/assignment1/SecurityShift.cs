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
    public class SecurityShift
    {
        public static async Task<APIGatewayProxyResponse> InsertSecurityShift(APIGatewayProxyRequest input, ILambdaContext context)
        {
            var data = System.Text.Json.JsonSerializer.Deserialize<SecurityShiftModel>(input.Body);

            Console.WriteLine(data.ToString());
            try
            {
                var str = "Server=myresidentialnoot.cm9dthnstss7.us-east-1.rds.amazonaws.com, 1433;Database=Residential Noot;User Id=admin;Password=admin123";
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();

                    var text = $"Insert into SecurityShiftModel (SecurityID, StartTime, EndTime, Location) Values ('{data.SecurityID }', CONVERT(DATETIME, '{data.StartTime}', 3),CONVERT(DATETIME, '{data.EndTime}', 3), '{data.Location}')";

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
        public static async Task<APIGatewayProxyResponse> RetrieveSecurityShift(APIGatewayProxyRequest input, ILambdaContext context)
        {

            List<SecurityShiftModel>Securitylist = new List<SecurityShiftModel>();
            try
            {
                var str = "Server=myresidentialnoot.cm9dthnstss7.us-east-1.rds.amazonaws.com, 1433;Database=Residential Noot;User Id=admin;Password=admin123";
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();
                    var text = "select * from SecurityShiftModel order by SecurityID DESC ";

                    SqlCommand cmd = new SqlCommand(text, conn);
                    var reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        SecurityShiftModel SecurityShift = new SecurityShiftModel()
                        {
                            SecurityID = (string)reader["SecurityID"],
                            ShiftID = (int)reader["ShiftID"],
                            StartTime = Convert.ToDateTime(reader["StartTime"]).ToString("dd/MM/yy hh:mm:ss t"),
                            EndTime = Convert.ToDateTime(reader["EndTime"]).ToString("dd/MM/yy hh:mm:ss t"),
                            Location = (string)reader["Location"]
                        };
                        Securitylist.Add(SecurityShift);
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
            if (Securitylist.Count > 0)
            {

                return new APIGatewayProxyResponse()
                {
                    Body = JsonConvert.SerializeObject(Securitylist),
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

        public static async Task<APIGatewayProxyResponse> DeleteShift(APIGatewayProxyRequest input, ILambdaContext context)
        {
            string ShiftID = null;
            input.QueryStringParameters?.TryGetValue("ShiftID", out ShiftID);
            ShiftID = ShiftID ?? "1";
            List<SecurityShiftModel> Shiftlist = new List<SecurityShiftModel>();

            try
            {
                var str = "Server=myresidentialnoot.cm9dthnstss7.us-east-1.rds.amazonaws.com, 1433;Database=Residential Noot;User Id=admin;Password=admin123";
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();

                    var text = $"delete from SecurityShiftModel where ShiftID = " + ShiftID;

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





        public class SecurityShiftModel
        {
            public string SecurityID { get; set; }

            public int ShiftID { get; set; }

            public string StartTime { get; set; }

            public string EndTime { get; set; }

            public string Location { get; set; }

        }
    }
}
