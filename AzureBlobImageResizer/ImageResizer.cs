using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AzureBlobImageResizer
{
    public static class ImageResizer
    {
        [FunctionName("ImageResizer")]
        public static void Run([BlobTrigger("unprocessed/{name}", Connection = "connie")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }
    }
}
