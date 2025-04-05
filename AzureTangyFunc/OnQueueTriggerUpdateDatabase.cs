using System;
using Azure.Storage.Queues.Models;
using AzureTangyFunc.Data;
using AzureTangyFunc.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureTangyFunc
{
    public class OnQueueTriggerUpdateDatabase
    {
        private readonly ILogger<OnQueueTriggerUpdateDatabase> _logger;
        private readonly AzureTangyDbContext _dbContext;

        public OnQueueTriggerUpdateDatabase(ILogger<OnQueueTriggerUpdateDatabase> logger, AzureTangyDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [Function(nameof(OnQueueTriggerUpdateDatabase))]
        public void Run([QueueTrigger("SalesRequestInBound", Connection = "AzureWebJobsStorage")] QueueMessage message)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {message.MessageText}");
            var request = JsonConvert.DeserializeObject<SalesRequest>(message.MessageText);

            request.Status = "Submitted";
            _dbContext.SalesRequests.Add(request);
            _dbContext.SaveChanges();
        }
    }
}
