using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace MailProcessingFunc
{
    public class FileProcessingHandler
    {
        [FunctionName("FileProcessingHandler")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            var files = req.Form.Files;

            if (files.Count > 0)
            {
                //var url = "https://flask-endpoint.azurewebsites.net/api/upload-file";
                var url = "http://127.0.0.1:5000/api/upload-file";
                var streamContent = new StreamContent(files[0].OpenReadStream());

                var multiContent = new MultipartFormDataContent
                {
                    { streamContent, "file", files[0].FileName }
                };

                HttpClient Client = new();
                var response = await Client.PostAsync(url, multiContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseMessage = await response.Content.ReadAsStringAsync();
                    return new OkObjectResult(responseMessage);
                }
                else
                    return new StatusCodeResult((int)response.StatusCode);
            }
            else
            {
                return new BadRequestResult();
            }
        }
    }
}

