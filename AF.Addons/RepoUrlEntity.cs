using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;

namespace AF.Addons
{
    class RepoUrlEntity : TableEntity
    {
        public string RepositoryUrl { get; set; }
        public string Email { get; set; }
        public string AddonName { get; set; }

        public RepoUrlEntity(string repoUrl, string email, string addonName)
        {
            RepositoryUrl = repoUrl;
            Email = email;
            AddonName = addonName;
            PartitionKey = "RepositoryToTest";
            RowKey = Guid.NewGuid().ToString();
        }

        public RepoUrlEntity() { }
    }
}
