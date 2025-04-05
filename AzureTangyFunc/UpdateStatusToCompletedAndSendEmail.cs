using System;
using AzureTangyFunc.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureTangyFunc
{
    public class UpdateStatusToCompletedAndSendEmail
    {
        private readonly ILogger _logger;
        private readonly AzureTangyDbContext _dbContext;

        public UpdateStatusToCompletedAndSendEmail(ILoggerFactory loggerFactory, AzureTangyDbContext dbContext)
        {
            _logger = loggerFactory.CreateLogger<UpdateStatusToCompletedAndSendEmail>();
            _dbContext = dbContext;
        }

        [Function("UpdateStatusToCompletedAndSendEmail")]
        public void Run([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
            
            var salesRequestsFromDb = _dbContext.SalesRequests.Where(x => x.Status == "Image Processed");
            foreach (var salesRequest in salesRequestsFromDb)
            {
                salesRequest.Status = "Completed";
            }
            _dbContext.UpdateRange(salesRequestsFromDb);
            _dbContext.SaveChanges();
        }
    }
}
