using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AF.Addons
{
    public static class ReadFromTable
    {
        [FunctionName("ReadFromTable")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            [Table("RepositoriesAndEmails", Connection = "AzureWebJobsStorage")] CloudTable table,
            ILogger log)
        {
            TableQuery<DynamicTableEntity> rangeQuery = new TableQuery<DynamicTableEntity>();

            var queryOutput = await table.ExecuteQuerySegmentedAsync(rangeQuery, null);

            var results = queryOutput.Results;

            List<object> repos = new List<object>();

            foreach (var entity in results)
            {
                var repo = entity.Properties["RepositoryUrl"].StringValue;
                var emailAdress = entity.Properties["Email"].StringValue;
                var addon = entity.Properties["AddonName"].StringValue;

                repos.Add(new {repoUrl = repo, email = emailAdress, addonName = addon});
            }

            return new OkObjectResult(repos);
        }
    }
}
