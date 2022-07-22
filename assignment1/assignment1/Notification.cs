using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static assignment1.SecurityShift;

namespace assignment1
{
    class Notification
    {

        public static async Task<APIGatewayProxyResponse> InsertNotification (APIGatewayProxyRequest input, ILambdaContext context)
        {
            var data = System.Text.Json.JsonSerializer.Deserialize<NotificationModel>(input.Body);

            Console.WriteLine(data.ToString());
            try
            {
                var str = "Server=myresidentialnoot.cm9dthnstss7.us-east-1.rds.amazonaws.com, 1433;Database=Residential Noot;User Id=admin;Password=admin123";
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();

                    var text = $"Insert into NotificationModel (imgsrc) Values ('{data.imgsrc }')";

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

        public static async Task<APIGatewayProxyResponse> RetrieveNotification (APIGatewayProxyRequest input, ILambdaContext context)
        {

            List<NotificationModel> Notilist = new List<NotificationModel>();
            try
            {
                var str = "Server=myresidentialnoot.cm9dthnstss7.us-east-1.rds.amazonaws.com, 1433;Database=Residential Noot;User Id=admin;Password=admin123";
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();
                    var text = "select * from NotificationModel order by NotificationID DESC ";

                    SqlCommand cmd = new SqlCommand(text, conn);
                    var reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        NotificationModel Noti = new NotificationModel()
                        {
                            NotificationID = (int)reader["NotificationID"],
                            imgsrc = (string)reader["imgsrc"]
                        };
                        Notilist.Add(Noti);
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
            if (Notilist.Count > 0)
            {

                return new APIGatewayProxyResponse()
                {
                    Body = JsonConvert.SerializeObject(Notilist),
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

    public class NotificationModel
    {

        public int NotificationID { get; set; }

        public string imgsrc { get; set; }


    }
}
