using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AzureBlobImageResizer
{
    public static class ImageResizer
    {
        [FunctionName("ImageResizer")]
        public static void Run([BlobTrigger("unprocessed/{name}", Connection = "DefaultEndpointsProtocol=https;AccountName=receiptsstorage;AccountKey=3WAk2ptGfORv1XK1G1OUHOcFQPl7EOXdUcdxzprFOz5Qd1Wh3IuhwkrPtb9YjU0iDd1T8aegH0ylWFn+ai3Gzw==;EndpointSuffix=core.windows.net")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
        }
    }
}
