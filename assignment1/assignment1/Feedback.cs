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

    public class Feedback
    {
        public static async Task<APIGatewayProxyResponse> RetrieveFeedback(APIGatewayProxyRequest input, ILambdaContext context)
        {

            List<FeedbackModel> Feedbacklist = new List<FeedbackModel>();
            try
            {
                var str = "Server=myresidentialnoot.cm9dthnstss7.us-east-1.rds.amazonaws.com, 1433;Database=Residential Noot;User Id=admin;Password=admin123";
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();
                    var text = "select * from FeedbackModel order by FeedbackID DESC ";

                    SqlCommand cmd = new SqlCommand(text, conn);
                    var reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        FeedbackModel Feedback = new FeedbackModel()
                        {
                            ResidentID = (string)reader["ResidentID"],
                            FeedbackID = (int)reader["FeedbackID"],
                            FeedbackContent = (string)reader["FeedbackContent"]
                        };
                        Feedbacklist.Add(Feedback);
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
            if (Feedbacklist.Count > 0)
            {

                return new APIGatewayProxyResponse()
                {
                    Body = JsonConvert.SerializeObject(Feedbacklist),
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
        public static async Task<APIGatewayProxyResponse> RetrieveIndiFeedback(APIGatewayProxyRequest input, ILambdaContext context)
        {
            string FeedbackID= null;
            input.QueryStringParameters?.TryGetValue("FeedbackID", out FeedbackID);
            FeedbackID = FeedbackID ?? "1";
            List<FeedbackModel> Feedbacklist = new List<FeedbackModel>();
            try
            {
                var str = "Server=myresidentialnoot.cm9dthnstss7.us-east-1.rds.amazonaws.com, 1433;Database=Residential Noot;User Id=admin;Password=admin123";
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();
                    var text = "select * from FeedbackModel where FeedbackID = " + FeedbackID;

                    SqlCommand cmd = new SqlCommand(text, conn);
                    var reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        FeedbackModel Feedback = new FeedbackModel()
                        {
                            ResidentID = (string)reader["ResidentID"],
                            FeedbackID = (int)reader["FeedbackID"],
                            FeedbackContent = (string)reader["FeedbackContent"]
                        };
                        Feedbacklist.Add(Feedback);
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
            if (Feedbacklist.Count > 0)
            {

                return new APIGatewayProxyResponse()
                {
                    Body = JsonConvert.SerializeObject(Feedbacklist),
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

        public static async Task<APIGatewayProxyResponse> DeleteFeedback(APIGatewayProxyRequest input, ILambdaContext context)
        {
            string FeedbackID = null;
            input.QueryStringParameters?.TryGetValue("FeedbackID", out FeedbackID);
            FeedbackID = FeedbackID ?? "1";
            List<FeedbackModel> Feedbacklist = new List<FeedbackModel>();

            try
            {
                var str = "Server=myresidentialnoot.cm9dthnstss7.us-east-1.rds.amazonaws.com, 1433;Database=Residential Noot;User Id=admin;Password=admin123";
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();

                    var text = $"delete from FeedbackModel where FeedbackID = " + FeedbackID;

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

        public static async Task<APIGatewayProxyResponse> InsertFeedback(APIGatewayProxyRequest input, ILambdaContext context)
        {
            var data = System.Text.Json.JsonSerializer.Deserialize<FeedbackModel>(input.Body);

            try
            {
                var str = "Server=myresidentialnoot.cm9dthnstss7.us-east-1.rds.amazonaws.com, 1433;Database=Residential Noot;User Id=admin;Password=admin123";
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();

                    var text = $"Insert into FeedbackModel (FeedbackContent, ResidentID) Values ('{data.FeedbackContent}', '{data.ResidentID}')";

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




        public class FeedbackModel
        {
            public int FeedbackID { get; set; }
            public string FeedbackContent { get; set; }

            public string ResidentID { get; set; }


        }
    }

}
