using AzureTangyFunc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureTangyFunc
{
    public class OnSalesUploadWriteToQueue
    {
        private readonly ILogger<OnSalesUploadWriteToQueue> _logger;

        public OnSalesUploadWriteToQueue(ILogger<OnSalesUploadWriteToQueue> logger)
        {
            _logger = logger;
        }

        [QueueOutput("SalesRequestInBound", Connection = "AzureWebJobsStorage")]
        [Function("OnSalesUploadWriteToQueue")]
        public async Task<SalesRequest> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req, 
             FunctionContext executionContext)
        {
            _logger.LogInformation("Sales Request received by OnSalesUploadWriteToQueue function.");

            var request = JsonConvert.DeserializeObject<SalesRequest>(await new StreamReader(req.Body).ReadToEndAsync());
            _logger.LogInformation($"payload: {JsonConvert.SerializeObject(request)}");
            return request;
        }
    }
}
