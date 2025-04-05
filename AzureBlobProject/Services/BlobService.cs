using System.Reflection.Metadata;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using AzureBlobProject.Models;
using Blob = AzureBlobProject.Models.Blob;

namespace AzureBlobProject.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        public BlobService(BlobServiceClient blobServiceClient)
        {
            this._blobServiceClient = blobServiceClient;
        }
        public async Task<List<string>> GetAllBlobs(string containerName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobs = blobContainerClient.GetBlobsAsync();
            var blobNames = new List<string>();
            await foreach (var blob in blobs)
            {
                blobNames.Add(blob.Name);
            }

            return blobNames;
        }

        public async Task<List<Blob>> GetAllBlobsWithUri(string containerName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobs = blobContainerClient.GetBlobsAsync();

            var sasContainerSignature = string.Empty;

            var blobList = new List<Blob>();

            if (blobContainerClient.CanGenerateSasUri)
            {
                var sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = blobContainerClient.Name,
                    Resource = "c",
                    ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
                };

                sasBuilder.SetPermissions(BlobAccountSasPermissions.Read);
                sasContainerSignature = blobContainerClient.GenerateSasUri(sasBuilder).AbsoluteUri.Split('?')[1].ToString();
            }

            await foreach (var blob in blobs)
            {
                var blobClient = blobContainerClient.GetBlobClient(blob.Name);
                var newBlob = new Blob
                {
                    Uri = string.IsNullOrWhiteSpace(sasContainerSignature) ? blobClient.Uri.AbsoluteUri : $"{blobClient.Uri.AbsoluteUri}?{sasContainerSignature}",
                };

                //if (blobClient.CanGenerateSasUri)
                //{
                //    var sasBuilder = new BlobSasBuilder
                //    {
                //        BlobContainerName = blobClient.BlobContainerName,
                //        BlobName = blob.Name,
                //        Resource = "b",
                //        ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
                //    };
                    
                //    sasBuilder.SetPermissions(BlobAccountSasPermissions.Read);
                //    newBlob.Uri = blobClient.GenerateSasUri(sasBuilder).AbsoluteUri;
                //}

                var blobProperties = await blobClient.GetPropertiesAsync();
                if (blobProperties.Value.Metadata.TryGetValue("title", out var title))
                {
                    newBlob.Title = title;
                }
                if (blobProperties.Value.Metadata.TryGetValue("comment", out var comment))
                {
                    newBlob.Comment = comment;
                }
                blobList.Add(newBlob);
            }

            return blobList;
        }

        public async Task<string> GetBlob(string name, string containerName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(name);

            return blobClient.Uri.AbsoluteUri;
        }

        public async Task<bool> UploadBlob(string name, IFormFile file, Blob blob, string containerName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(name);

            var httpHeaders = new BlobHttpHeaders
            {
                ContentType = file.ContentType
            };

            var metaData = new Dictionary<string, string>
            {
                { "title", blob.Title },
                { "comment", blob.Comment }
            };

            var result = await blobClient.UploadAsync(file.OpenReadStream(), httpHeaders, metaData);

            // To edit metadata we need to use the SetMetaDataAsync method
            //metaData.Remove("comment");
            //await blobClient.SetMetadataAsync(metaData);

            if (result != null)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteBlob(string name, string containerName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = blobContainerClient.GetBlobClient(name);

            return await blobClient.DeleteIfExistsAsync();
        }
    }
}
