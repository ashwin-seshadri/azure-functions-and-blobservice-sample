using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AzureBlobProject.Services
{
    public class ContainerService : IContainerService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public ContainerService(BlobServiceClient blobServiceClient)
        {
            this._blobServiceClient = blobServiceClient;
        }
        public async Task<List<string>> GetAllContainersAndBlobs()
        {
            List<string> containerAndBlobNames = new();
            containerAndBlobNames.Add($"Account Name: {this._blobServiceClient.AccountName}");
            containerAndBlobNames.Add("------------------------------------------------------------------------------------------------");
            await foreach (var container in _blobServiceClient.GetBlobContainersAsync())
            {
                containerAndBlobNames.Add("-- " + container.Name);
                var blobContainerClient = _blobServiceClient.GetBlobContainerClient(container.Name);
                await foreach (var blob in blobContainerClient.GetBlobsAsync())
                {
                    //get meta data
                    var blobClient = blobContainerClient.GetBlobClient(blob.Name);
                    var blobProperties = await blobClient.GetPropertiesAsync();
                    var blobItem = blob.Name;
                    if (blobProperties.Value.Metadata.Keys.Count > 0)
                    {
                        blobItem = $"{blobItem} ({string.Join(',',blobProperties.Value.Metadata.Select(kvp => $"{kvp.Key}:{kvp.Value}"))})";
                    }
                    containerAndBlobNames.Add("------ " + blobItem);
                }
                containerAndBlobNames.Add("------------------------------------------------------------------------------------------------");
            }

            return containerAndBlobNames;
        }

        public async Task<List<string>> GetAllContainers()
        {
            List<string> containerNames = new();
            await foreach (var container in _blobServiceClient.GetBlobContainersAsync())
            {
                containerNames.Add(container.Name);
            }

            return containerNames;
        }

        public async Task CreateContainer(string containerName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.None);
        }

        public async Task DeleteContainer(string containerName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await blobContainerClient.DeleteIfExistsAsync();
        }
    }
}
