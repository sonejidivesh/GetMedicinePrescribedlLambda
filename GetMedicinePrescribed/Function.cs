using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

using Newtonsoft.Json;
using System.Data.SqlClient;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace GetMedicinePrescribed;

public class Function
{

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<APIGatewayProxyResponse> FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
    {
        string jsonresponse = string.Empty;
        try {

            List<MedicationPrescribed> medicationPrescribeds = new List<MedicationPrescribed>();
            string id = string.Empty;
            input.PathParameters?.TryGetValue("id", out id);
            string query = $"select * from MedicationPrescribeds where PrescriptionId = {id}";
            string connectionString = "Server=clinic-mssql.cyhbv4dqbk22.us-east-1.rds.amazonaws.com,1433;User Id=admin;Password=#$Suadmin#$;Trusted_Connection=false; MultipleActiveResultSets=true;database=clinicDb;TrustServerCertificate=True";
            using (var conn = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand(query, conn))
                {
                    Console.WriteLine("Try connecting to RDS");
                    conn.Open();
                    Console.WriteLine("connection successfull!");
                    var rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        medicationPrescribeds.Add(new MedicationPrescribed
                        {

                            Id = Convert.ToInt32(rdr[0]),
                            MedicationId = Convert.ToInt32(rdr[1]),
                            Dosage = Convert.ToInt32(rdr[2]),
                            PrescriptionId = Convert.ToInt32(rdr[3]),
                            Frequency = Convert.ToInt32(rdr[4]),
                        });
                    }
                }
            }

            jsonresponse = JsonConvert.SerializeObject(medicationPrescribeds);



            return new APIGatewayProxyResponse
            {

                StatusCode = 200,
                Body = jsonresponse,
                Headers = { }
            };



        }
        catch(Exception ex){
            jsonresponse = ex.Message;

            return new APIGatewayProxyResponse
            {

                StatusCode = 502,
                Body = jsonresponse,
                Headers = { }
            };


        }

    }


}


public class MedicationPrescribed
{
    public int Id { get; set; }

    public int MedicationId { get; set; }

    public double Dosage { get; set; }

    public int PrescriptionId { get; set; }

    public int Frequency { get; set; }

}
