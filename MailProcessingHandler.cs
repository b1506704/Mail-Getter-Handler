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
using System.Web;

namespace MailProcessingFunc
{
    public static class MailProcessingHandler
    {
        [FunctionName("MailProcessingHandler")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
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
            var updatedBody = HttpUtility.HtmlDecode(emailBodyContent);
            updatedBody = updatedBody.Replace("\r\n", " ");
            updatedBody = updatedBody.Replace("\n", " ");

            var prompt = new InputDto()
            {
                Prompt = updatedBody
            };

            var url = "https://flask-endpoint.azurewebsites.net/api/prompt_handler";
            //var url = "http://127.0.0.1:5000/api/prompt_handler";

            HttpClient Client = new();
            var requestToFlaskEndpoint = JsonConvert.SerializeObject(prompt, jsonSerializerSettings);

            var response = await Client.PostAsync(url, new StringContent(requestToFlaskEndpoint,
                                                                         Encoding.UTF8,
                                                                         "application/json"));

            var responseMessage = await response.Content.ReadAsStringAsync();
            return new OkObjectResult(responseMessage);
        }

    }
}