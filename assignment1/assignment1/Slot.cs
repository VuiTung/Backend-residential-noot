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
    class Slot
    {

        public static async Task<APIGatewayProxyResponse> RetrieveSlots(APIGatewayProxyRequest input, ILambdaContext context)
        {

            List<SlotsModel> SlotsList = new List<SlotsModel>();
            try
            {
                var str = "Server=myresidentialnoot.cm9dthnstss7.us-east-1.rds.amazonaws.com, 1433;Database=Residential Noot;User Id=admin;Password=admin123";
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();
                    var text = "select * from SlotsModel order by SlotID ";

                    SqlCommand cmd = new SqlCommand(text, conn);
                    var reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        SlotsModel Slots = new SlotsModel()
                        {
                            SlotID = (int)reader["SlotID"],
                            TimeSlot = (string)reader["TimeSlot"],

                        };
                        SlotsList.Add(Slots);
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
            if (SlotsList.Count > 0)
            {

                return new APIGatewayProxyResponse()
                {
                    Body = JsonConvert.SerializeObject(SlotsList),
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

    public class SlotsModel
    {
        public int SlotID { get; set; }

        public string TimeSlot { get; set; }

        internal void Add(List<SlotsModel> slots)
        {
            throw new NotImplementedException();
        }
    }
}
