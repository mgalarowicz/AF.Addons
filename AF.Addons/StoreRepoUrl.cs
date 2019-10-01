using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AF.Addons
{
    public static class StoreRepoUrl
    {
        [FunctionName("StoreRepoUrlWithEmail")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            [Table("RepositoriesAndEmails", Connection = "AzureWebJobsStorage")] CloudTable table,
            ILogger log)
        {
            
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic postData = JsonConvert.DeserializeObject(requestBody);
            string fromRepo = postData?.fromRepo;
            string fromEmail = postData?.fromEmail;
            string fromAddon = postData?.fromAddon;

            var missingFields = new List<string>();

            if (String.IsNullOrEmpty(fromRepo))
                missingFields.Add("repo");

            if (String.IsNullOrEmpty(fromEmail))
                missingFields.Add("email");

            if (String.IsNullOrEmpty(fromAddon))
                missingFields.Add("addon");

            if (missingFields.Any())
            {
                var missingFieldsSummary = String.Join(", ", missingFields);
                return new BadRequestObjectResult($"Missing field(s): {missingFieldsSummary}");
            }

            try
            {
                await table.CreateIfNotExistsAsync();
                
                AddRowToTable(table, new RepoUrlEntity(fromRepo, fromEmail, fromAddon));

                return new OkObjectResult("Thanks! I've successfully received your request.'");
            }
            catch (Exception ex)
            {
                return new ConflictObjectResult($"There are problems storing your repo adress: {ex.GetType()}, {ex.Message}");
            }
        }

        static async void AddRowToTable(CloudTable table, RepoUrlEntity newRepoUrl)
        {
            TableOperation insert = TableOperation.Insert(newRepoUrl);

            await table.ExecuteAsync(insert);
        }
    }
}
