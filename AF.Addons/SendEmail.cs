using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SendGrid.Helpers.Mail;
using System.Text;

namespace AF.Addons
{
    public static class SendEmail
    {
        [FunctionName("SendEmail")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [SendGrid(ApiKey = "AzureWebJobsSendGridApiKey")] IAsyncCollector<SendGridMessage> messages,
            ILogger log)
        {
            using (StreamReader reader = new StreamReader(req.Body, Encoding.UTF8))
            {
                try
                {
                    var body = await reader.ReadToEndAsync();

                    dynamic postData = JsonConvert.DeserializeObject(body);

                    string fromAttachment = postData?.fromAttachment;
                    string fromEmail = postData?.fromEmail;
                    string fromAddon = postData?.fromAddon;

                    var message = new SendGridMessage();

                    message.AddAttachment($"{fromAddon}.zip", fromAttachment, "application/zip", "attachment", "Zip File");

                    message.AddTo(fromEmail);

                    message.AddContent("text/plain", "To download results, visit soneta-partners portal");

                    message.SetFrom("marek.galarowicz@enova.pl");

                    message.SetSubject($"Build and test results for {fromAddon}");

                    await messages.AddAsync(message);

                    return new OkObjectResult("The E-mail has been sent.");
                }
                catch (Exception e)
                {
                    return new ConflictObjectResult($"There are problems with sending an email: {e.GetType()}, {e.Message}");
                }
                

            }
        }
    }
}
