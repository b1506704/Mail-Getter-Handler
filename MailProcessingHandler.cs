using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MailProcessingFunc
{
    public static class MailProcessingHandler
    {
        [FunctionName("MailProcessingHandler")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("HttpWebhook triggered");

            var jsonSerializerSettings = new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };

            string emailBodyContent = await new StreamReader(req.Body).ReadToEndAsync();
            var prompt = JsonConvert.DeserializeObject<InputDto>(emailBodyContent, jsonSerializerSettings);
            //// Replace HTML with other characters
            //string updatedBody = Regex.Replace(emailBodyContent, "<.*?>", string.Empty);
            //updatedBody = updatedBody.Replace("\\r\\n", " ");
            //updatedBody = updatedBody.Replace(@"&nbsp;", " ");
            var url = "https://flask-endpoint.azurewebsites.net/api/prompt_handler";
            //var testToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjFFQTVCOTlCQkI0RkMwODE4Q0RCQUJDQTg0QTdCNUJCQ0JBODFEQTlSUzI1NiIsInR5cCI6IkpXVCIsIng1dCI6IkhxVzVtN3RQd0lHTTI2dktoS2UxdTh1b0hhayJ9.eyJuYmYiOjE3MDk1NjU1NTMsImV4cCI6MTcwOTU2OTE1MywiaXNzIjoiaHR0cHM6Ly93ZWJzdGVyY2FyZS1hdXRoZW50aWNhdGlvbi1hcGktZGV2LmF6dXJld2Vic2l0ZXMubmV0IiwiYXVkIjpbImdhdGV3YXktYXBpIiwidmlldy1hcGkiLCJodHRwczovL3dlYnN0ZXJjYXJlLWF1dGhlbnRpY2F0aW9uLWFwaS1kZXYuYXp1cmV3ZWJzaXRlcy5uZXQvcmVzb3VyY2VzIl0sImNsaWVudF9pZCI6InJ4bWVkY2hhcnQucmVzb3VyY2Vvd25lciIsInN1YiI6IjJkNWQwODc2LTNlMDctNDYyOC04Yzc3LTU2NDdjZmMzM2NhYyIsImF1dGhfdGltZSI6MTcwOTU2NTU1MiwiaWRwIjoibG9jYWwiLCJuYW1lIjoiZGV2IGhlYWx0aGNhcmVzaXRlLmFkbWluIiwiZ2l2ZW5fbmFtZSI6ImRldiIsImZhbWlseV9uYW1lIjoiaGVhbHRoY2FyZXNpdGUuYWRtaW4iLCJpZGVudGlmaWVycyI6IntcInJvbGVcIjpcIkhlYWx0aGNhcmVTaXRlLUFkbWluaXN0cmF0b3JcIixcImhlYWx0aGNhcmVfc2l0ZV9pZHNcIjpbXCJBVU5TVzAwMDAwMU1TXCJdfSIsInJvbGUiOiJIZWFsdGhjYXJlU2l0ZS1BZG1pbmlzdHJhdG9yIiwiaWF0IjoxNzA5NTY1NTUzLCJzY29wZSI6WyJnYXRld2F5LWFwaSIsInZpZXctYXBpIl0sImFtciI6WyJjdXN0b20iXX0.EG71_XWHs9s5AKjCmcgC5oyn7IuAtvrTV-sK7YmEClcoKlM19C-ZeJavy5cWt94prmiBXBWTr-WQOoes6Qty6X6SHnE5A5n5FkVskGP13hTk20dHthc9qhsVCtpHci6u2AeVj5Bb6Og8Lx2bQxb6zKxpQtMAX-4Run98JCS7fMPpTTmvHkvd9Y3PoSbTs4UpbMCTmLRnmSXR2nGQkMYZoLsaxDRas7z5YsxuI0cNK1u7vAUUaj7BreyJUkW23gojjfuCyoNjRQ_UAPMIv9ySUEn-oalreIsTbMAjhnzxCcj9FVkRdiA_Xthajfk0tv7XNuqPjuxu-rooBFiW3mTcCw";
            HttpClient Client = new();
            var requestToFlaskEndpoint = JsonConvert.SerializeObject(prompt, jsonSerializerSettings);
            //Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", testToken);

            var response = await Client.PostAsync(url, new StringContent(requestToFlaskEndpoint,
                                                                         Encoding.UTF8,
                                                                         "application/json"));

            var responseMessage = await response.Content.ReadAsStringAsync();
            return new OkObjectResult(responseMessage);
        }
    }
}