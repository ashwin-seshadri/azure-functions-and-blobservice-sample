using System.IO;
using System.Security.AccessControl;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace AzureTangyFunc
{
    public class ResizeImageOnBlobUpload
    {
        private readonly ILogger<ResizeImageOnBlobUpload> _logger;

        public ResizeImageOnBlobUpload(ILogger<ResizeImageOnBlobUpload> logger)
        {
            _logger = logger;
        }

        [BlobOutput("functionsalesrep-sm/{name}", Connection = "AzureWebJobsStorage")]
        [Function(nameof(ResizeImageOnBlobUpload))]
        public async Task<byte[]> Run([BlobTrigger("functionsalesrep/{name}", Connection = "AzureWebJobsStorage")] Byte[] myBlob, 
            string name)
        {
            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Length: {myBlob.Length}");

            using var input = Image.Load(myBlob);
            input.Mutate(x => x.Resize(300, 200));

            using var outputStream = new MemoryStream();
            input.SaveAsJpeg(outputStream);
            return outputStream.ToArray();
        }
    }
}
