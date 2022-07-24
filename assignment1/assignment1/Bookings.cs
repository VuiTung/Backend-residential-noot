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
    class Bookings
    {
        public static async Task<APIGatewayProxyResponse> InsertBookings(APIGatewayProxyRequest input, ILambdaContext context)
        {
            var data = System.Text.Json.JsonSerializer.Deserialize<BookingsModel>(input.Body);
            Console.WriteLine(data.ToString());
            try
            {
                var str = "Server=myresidentialnoot.cm9dthnstss7.us-east-1.rds.amazonaws.com, 1433;Database=Residential Noot;User Id=admin;Password=admin123";
                using (SqlConnection conn = new SqlConnection(str))
                {

                    conn.Open();

                    var text = $"Insert into BookingsModel (SlotID, NumberOfPerson, FacilityID, Resident_ID, Date) Values ('{data.SlotID}','{data.NumberOfPerson}', '{data.FacilityID}','{data.Resident_ID}',CONVERT(DATETIME, '{data.Date}', 3))";

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
        public static async Task<APIGatewayProxyResponse> RetrieveBooking(APIGatewayProxyRequest input, ILambdaContext context)
        {

            List<BookingsModel> Bookinglist = new List<BookingsModel>();
            try
            {
                var str = "Server=myresidentialnoot.cm9dthnstss7.us-east-1.rds.amazonaws.com, 1433;Database=Residential Noot;User Id=admin;Password=admin123";
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();
                    var text = "select * from BookingsModel bm inner join AspNetUsers anu on anu.Id= bm.Resident_ID order by BookID DESC ";

                    SqlCommand cmd = new SqlCommand(text, conn);
                    var reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        BookingsModel Booking = new BookingsModel()
                        {
                            BookID = (int)reader["BookID"],
                            SlotID = (int)reader["SlotID"],
                            NumberOfPerson = (int)reader["NumberOfPerson"],
                            FacilityID = (int)reader["FacilityID"],
                            Resident_ID = (string)reader["UserName"],
                            Date= Convert.ToDateTime(reader["Date"]).ToString("dd/MM/yy hh:mm:ss t")
                        };
                        Bookinglist.Add(Booking);
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
            if (Bookinglist.Count > 0)
            {

                return new APIGatewayProxyResponse()
                {
                    Body = JsonConvert.SerializeObject(Bookinglist),
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

        public static async Task<APIGatewayProxyResponse> RetrieveIndiBookings(APIGatewayProxyRequest input, ILambdaContext context)
        {
            string Resident_ID = null;
            input.QueryStringParameters?.TryGetValue("Resident_ID", out Resident_ID);
            Resident_ID = Resident_ID ?? "bcd74606-10e7-468d-b964-e801c87cc2c2";
            List<BookingsModel> BookingList = new List<BookingsModel>();
            try
            {
                var str = "Server=myresidentialnoot.cm9dthnstss7.us-east-1.rds.amazonaws.com, 1433;Database=Residential Noot;User Id=admin;Password=admin123";
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();
                    var text = $"select * from BookingsModel where Resident_ID = '{Resident_ID}'";

                    SqlCommand cmd = new SqlCommand(text, conn);
                    var reader = await cmd.ExecuteReaderAsync();
                    while (reader.Read())
                    {
                        BookingsModel Booking = new BookingsModel()
                        {
                            BookID = (int)reader["BookID"],
                            SlotID = (int)reader["SlotID"],
                            NumberOfPerson = (int)reader["NumberOfPerson"],
                            FacilityID = (int)reader["FacilityID"],
                            Resident_ID = (string)reader["Resident_ID"],
                            Date = Convert.ToDateTime(reader["Date"]).ToString("dd/MM/yy hh:mm:ss t")
                        };
                        BookingList.Add(Booking);
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
            if (BookingList.Count > 0)
            {

                return new APIGatewayProxyResponse()
                {
                    Body = JsonConvert.SerializeObject(BookingList),
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


        public static async Task<APIGatewayProxyResponse> DeleteBooking(APIGatewayProxyRequest input, ILambdaContext context)
        {
            string BookID = null;
            input.QueryStringParameters?.TryGetValue("BookID", out BookID);
            BookID = BookID ?? "4";
            List<BookingsModel> Bookinglist = new List<BookingsModel>();

            try
            {
                var str = "Server=myresidentialnoot.cm9dthnstss7.us-east-1.rds.amazonaws.com, 1433;Database=Residential Noot;User Id=admin;Password=admin123";
                using (SqlConnection conn = new SqlConnection(str))
                {
                    conn.Open();

                    var text = $"delete from BookingsModel where BookID = " + BookID;

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

    public class BookingsModel
    {
        public int BookID { get; set; }

        public int SlotID { get; set; }

        public int NumberOfPerson { get; set; }

        public int FacilityID { get; set; }

        public string Resident_ID { get; set; }

        public string Date { get; set; }

    }
}
