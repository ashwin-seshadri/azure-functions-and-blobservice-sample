using Microsoft.AspNetCore.Authorization;

namespace AzureBlobProject.Models
{
    public class Blob
    {
        public string Title { get; set; }
        public string Comment { get; set; }
        public string Uri { get; set; }
    }
}
