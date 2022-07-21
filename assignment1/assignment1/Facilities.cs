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
    class Facilities
    {
        public static async Task<APIGatewayProxyResponse> InsertFacilities(APIGatewayProxyRequest input, ILambdaContext context)
        {
            var data = System.Text.Json.JsonSerializer.Deserialize<FacilitiesModel>(input.Body);
            Console.WriteLine(data.ToString());
            try
            {
                var str = "Server=myresidentialnoot.cm9dthnstss7.us-east-1.rds.amazonaws.com, 1433;Database=Residential Noot;User Id=admin;Password=admin123";
                using (SqlConnection conn = new SqlConnection(str))
                {

                    conn.Open();


                    var text = $"Insert into FacilitiesModel (FacilityName, Block, Floor, MaximumPplPerSlot) Values ('{data.FacilityName}','{data.Block}', '{data.Floor}','{data.MaximumPplPerSlot}')";

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
        public static async Task<APIGatewayProxyResponse> RetrieveFacilities(APIGatewayProxyRequest input, ILambdaContext context)
        {

            List<FacilitiesModel> FacilitiesList = new List<FacilitiesModel>();
            try
            {
                var str = "Server=myresidentialnoot.cm9dthnstss7.us-east-1.rds.amazonaws.com, 1433;Database=Residential Noot;User Id=admin;Password=admin123";
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();
                    var text = "select * from FacilitiesModel order by FacilityID DESC ";

                    SqlCommand cmd = new SqlCommand(text, conn);
                    var reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        FacilitiesModel Facilities = new FacilitiesModel()
                        {
                            FacilityID = (int)reader["FacilityID"],
                            FacilityName = (string)reader["FacilityName"],
                            Block = (string)reader["Block"],
                            Floor = (int)reader["Floor"],
                            MaximumPplPerSlot = (int)reader["MaximumPplPerSlot"]
                        };
                        FacilitiesList.Add(Facilities);
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
            if (FacilitiesList.Count > 0)
            {

                return new APIGatewayProxyResponse()
                {
                    Body = JsonConvert.SerializeObject(FacilitiesList),
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

        public static async Task<APIGatewayProxyResponse> RetrieveFacilitiesIndi(APIGatewayProxyRequest input, ILambdaContext context)
        {
            string FacilityID = null;
            input.QueryStringParameters?.TryGetValue("FacilityID", out FacilityID);
            FacilityID = FacilityID ?? "1";
            List<FacilitiesModel> FacilitiesList = new List<FacilitiesModel>();
            try
            {
                var str = "Server=myresidentialnoot.cm9dthnstss7.us-east-1.rds.amazonaws.com, 1433;Database=Residential Noot;User Id=admin;Password=admin123";
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();
                    var text = $"select * from FacilitiesModel where FacilityID = '{FacilityID}'";

                    SqlCommand cmd = new SqlCommand(text, conn);
                    var reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        FacilitiesModel Facilities = new FacilitiesModel()
                        {
                            FacilityID = (int)reader["FacilityID"],
                            FacilityName = (string)reader["FacilityName"],
                            Block = (string)reader["Block"],
                            Floor = (int)reader["Floor"],
                            MaximumPplPerSlot = (int)reader["MaximumPplPerSlot"]
                        };
                        FacilitiesList.Add(Facilities);
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
            if (FacilitiesList.Count > 0)
            {

                return new APIGatewayProxyResponse()
                {
                    Body = JsonConvert.SerializeObject(FacilitiesList),
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


        public static async Task<APIGatewayProxyResponse> DeleteFacilities(APIGatewayProxyRequest input, ILambdaContext context)
        {
            string FacilityID = null;
            input.QueryStringParameters?.TryGetValue("FacilityID", out FacilityID);
            FacilityID = FacilityID ?? "3";
            List<FacilitiesModel> Facilitieslist = new List<FacilitiesModel>();

            try
            {
                var str = "Server=myresidentialnoot.cm9dthnstss7.us-east-1.rds.amazonaws.com, 1433;Database=Residential Noot;User Id=admin;Password=admin123";
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();

                    var text = $"delete from FacilitiesModel where FacilityID = " + FacilityID;

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

        public class FacilitiesModel
    {

        public int FacilityID { get; set; }

        public string FacilityName { get; set; }

        public string Block { get; set; }

        public int Floor { get; set; }

        public int MaximumPplPerSlot { get; set; }

    }

}
