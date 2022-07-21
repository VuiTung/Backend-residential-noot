using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static assignment1.Feedback;

namespace assignment1
{
    public class SlotBooking
    {
        public static async Task<APIGatewayProxyResponse> RetrieveAvailableBookingSlot(APIGatewayProxyRequest input, ILambdaContext context)
        {
            string FacilityID = null;
            input.QueryStringParameters?.TryGetValue("FacilityID", out FacilityID);
            FacilityID = FacilityID ?? "1";
            string Date = null;
            input.QueryStringParameters?.TryGetValue("Date", out Date);
            Date = Date ?? "19/09/22 00:00:00 AM";
            string NumberOfPerson = null;
            input.QueryStringParameters?.TryGetValue("NumberOfPerson", out NumberOfPerson);
            NumberOfPerson = NumberOfPerson ?? "1";
            int NOP = int.Parse(NumberOfPerson);
            List<BookingsModel> BookingsList = new List<BookingsModel>();
            List<SlotsModel> SlotsList = new List<SlotsModel>();
            List<FacilityModel> FacilityList = new List<FacilityModel>();
            try
            {
                var str = "Server=myresidentialnoot.cm9dthnstss7.us-east-1.rds.amazonaws.com, 1433;Database=Residential Noot;User Id=admin;Password=admin123";
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();
                    var text = $"select * from BookingsModel where FacilityID='{FacilityID}' AND Date = convert(datetime,'{Date}',3)";
                    var text2 = $"select * from SlotsModel";
                    var text3 = $"Select FacilityID, MaximumPplPerSlot from FacilitiesModel where FacilityID=" + FacilityID;
                    SqlCommand cmd3 = new SqlCommand(text3, conn);
                    var reader3 = await cmd3.ExecuteReaderAsync();
                    while (reader3.Read())
                    {
                        FacilityModel facility = new FacilityModel()
                        {
                            FacilityID = (int)reader3["FacilityID"],
                            MaximumPplPerSlot = (int)reader3["MaximumPplPerSlot"],
                        };
                        FacilityList.Add(facility);
                    }
                    reader3.Close();
                    SqlCommand cmd = new SqlCommand(text, conn);
                    var reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        BookingsModel Booking = new BookingsModel()
                        {
                            BookID = (int)reader["BookID"],
                            SlotID = (int)reader["SlotID"],
                            NumberOfPerson = (int)reader["NumberOfPerson"],
                            FacilityID = (int)reader["FacilityID"]
                        };
                        BookingsList.Add(Booking);
                    }
                    reader.Close();
                    SqlCommand cmd2 = new SqlCommand(text2, conn);
                    var reader2 = await cmd2.ExecuteReaderAsync();
                    while (reader2.Read())
                    {
                        SlotsModel Slots = new SlotsModel()
                        {
                            TimeSlot = (string)reader2["TimeSlot"],
                            SlotID = (int)reader2["SlotID"],
                            Spaceleft = FacilityList[0].MaximumPplPerSlot
                        };
                        SlotsList.Add(Slots);
                    }
                    reader2.Close();
                    int lastnum = 16;
                    for( int i =0; i< lastnum; i++)
                    {
                        foreach (BookingsModel B in BookingsList)
                        {
                            if (B.SlotID== SlotsList[i].SlotID)
                            {
                                SlotsList[i].Spaceleft -= B.NumberOfPerson;
                                if (SlotsList[i].Spaceleft< NOP)
                                {
                                    SlotsList.RemoveAt(i);
                                    i--;
                                    lastnum--;
                                }
                            }
                        }
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
        public class BookingsModel
        {
            public int BookID { get; set; }

            public int SlotID { get; set; }

            public int NumberOfPerson { get; set; }

            public int FacilityID { get; set; }

            public int Resident_ID { get; set; }


        }
        public class SlotsModel
        {
            public int SlotID { get; set; }
            public int Spaceleft { get; set; }
            public string TimeSlot { get; set; }
        }
        public class FacilityModel
        {
            public int FacilityID { get; set; }

            public int MaximumPplPerSlot { get; set; }

        }
    }
}
