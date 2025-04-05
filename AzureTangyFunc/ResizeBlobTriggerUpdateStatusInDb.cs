using System.IO;
using System.Threading.Tasks;
using AzureTangyFunc.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AzureTangyFunc
{
    public class ResizeBlobTriggerUpdateStatusInDb
    {
        private readonly ILogger<ResizeBlobTriggerUpdateStatusInDb> _logger;
        private readonly AzureTangyDbContext _dbContext;

        public ResizeBlobTriggerUpdateStatusInDb(ILogger<ResizeBlobTriggerUpdateStatusInDb> logger, AzureTangyDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [Function(nameof(ResizeBlobTriggerUpdateStatusInDb))]
        public async Task Run([BlobTrigger("functionsalesrep-sm/{name}", Connection = "AzureWebJobsStorage")] Stream stream, string name)
        {
            var fileName = Path.GetFileNameWithoutExtension(name);
            var salesRequestFromDb = _dbContext.SalesRequests.FirstOrDefault(x => x.Id == fileName);
            if(salesRequestFromDb != null)
            {
                salesRequestFromDb.Status = "Image Processed";
                _dbContext.Update(salesRequestFromDb);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
