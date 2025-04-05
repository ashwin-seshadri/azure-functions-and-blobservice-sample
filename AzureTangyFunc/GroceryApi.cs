using AzureTangyFunc.Data;
using AzureTangyFunc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureTangyFunc
{
    public class GroceryApi
    {
        private readonly ILogger<GroceryApi> _logger;
        private readonly AzureTangyDbContext _dbContext;

        public GroceryApi(ILogger<GroceryApi> logger, AzureTangyDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [Function("CreateGrocery")]
        public async Task<IActionResult> CreateGrocery([HttpTrigger(AuthorizationLevel.Function, "post", Route = "GroceryList")] HttpRequest req)
        {
            _logger.LogInformation("Creating Grocery List Item.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<GroceryItem_Upsert>(requestBody);

            var item = new GroceryItem
            {
                Name = data.Name,
            };

            _dbContext.GroceryItems.Add(item);
            await _dbContext.SaveChangesAsync();
            return new OkObjectResult(item);
        }

        [Function("GetGrocery")]
        public async Task<IActionResult> GetGrocery([HttpTrigger(AuthorizationLevel.Function, "get", Route = "GroceryList")] HttpRequest req)
        {
            _logger.LogInformation("Getting Grocery List Item.");

            return new OkObjectResult(_dbContext.GroceryItems.ToList());
        }

        [Function("GetGroceryById")]
        public async Task<IActionResult> GetGroceryById([HttpTrigger(AuthorizationLevel.Function, "get", Route = "GroceryList/{id}")] HttpRequest req, string id)
        {
            _logger.LogInformation("Getting Grocery List Item by Id.");

            var item = _dbContext.GroceryItems.FirstOrDefault(x => x.Id == id);

            if (item == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(item);
        }

        [Function("UpdateGrocery")]
        public async Task<IActionResult> UpdateGrocery([HttpTrigger(AuthorizationLevel.Function, "put", Route = "GroceryList/{id}")] HttpRequest req, string id)
        {
            _logger.LogInformation("Updating Grocery List Item.");

            var item = _dbContext.GroceryItems.FirstOrDefault(x => x.Id == id);

            if (item == null)
            {
                return new NotFoundResult();
            }

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updatedData = JsonConvert.DeserializeObject<GroceryItem_Upsert>(requestBody);

            if (!string.IsNullOrEmpty(updatedData.Name))
            {
                item.Name = updatedData.Name;
                _dbContext.GroceryItems.Update(item);
                await _dbContext.SaveChangesAsync();
            }

            return new OkObjectResult(item);
        }

        [Function("DeleteGrocery")]
        public async Task<IActionResult> DeleteGrocery([HttpTrigger(AuthorizationLevel.Function, "put", Route = "GroceryList/{id}")] HttpRequest req, string id)
        {
            _logger.LogInformation("Deleting Grocery List Item.");

            var item = _dbContext.GroceryItems.FirstOrDefault(x => x.Id == id);

            if (item == null)
            {
                return new NotFoundResult();
            }

            _dbContext.GroceryItems.Remove(item);
            await _dbContext.SaveChangesAsync();
            return new OkResult();
        }
    }
}
