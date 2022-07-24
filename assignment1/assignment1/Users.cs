using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace assignment1
{
    class Users
    {
        public static async Task<APIGatewayProxyResponse> RetrieveUserSecurity (APIGatewayProxyRequest input, ILambdaContext context)
        {

            List<AspNetUsers> Userlist = new List<AspNetUsers>();
            try
            {
                var str = "Server=myresidentialnoot.cm9dthnstss7.us-east-1.rds.amazonaws.com, 1433;Database=Residential Noot;User Id=admin;Password=admin123";
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();
                    var text = "select * from AspNetUsers where UserRole = 'SecurityGuard'";

                    SqlCommand cmd = new SqlCommand(text, conn);
                    var reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        AspNetUsers user = new AspNetUsers()
                        {
                            Id = (string)reader["Id"],
                            UserRole = (string)reader["UserRole"],
                            UserName = (string)reader["UserName"]
                        };
                        Userlist.Add(user);
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
            if (Userlist.Count > 0)
            {

                return new APIGatewayProxyResponse()
                {
                    Body = JsonConvert.SerializeObject(Userlist),
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


        public class AspNetUsers
        {
            public string Id { get; set; }

            public string UserRole { get; set; }

            public string UserName { get; set; }
        }

    }
}
