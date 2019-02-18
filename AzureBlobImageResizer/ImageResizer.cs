using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Microsoft.WindowsAzure.Storage;

namespace AzureBlobImageResizer
{
    public static class ImageResizer
    {
        [FunctionName("ImageResizer")]
        public static async Task Run([BlobTrigger("unprocessed/{name}", Connection = "connie")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");
            try
            {
                if (myBlob != null)
                {
                    var encoder = GetEncoder(".jpg");

                    if (encoder != null)
                    {
                        var storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("connie"));
                        var blobClient = storageAccount.CreateCloudBlobClient();
                        var container = blobClient.GetContainerReference("processed");
                        var blockBlob = container.GetBlockBlobReference(name);

                        using (var output = new MemoryStream())
                        using (Image<Rgba32> image = Image.Load(myBlob))
                        {
                            var resizeOptions = new ResizeOptions {
                                Mode = ResizeMode.Max,
                                Size = new SixLabors.Primitives.Size(3150)
                            };

                            image.Mutate(x => x.Resize(resizeOptions));
                            image.Save(output, encoder);
                            output.Position = 0;
                            await blockBlob.UploadFromStreamAsync(output);
                        }
                    }
                    else
                    {
                        log.LogInformation($"No encoder support");
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogInformation(ex.Message);
                throw;
            }
        }


        private static IImageEncoder GetEncoder(string extension)
        {
            IImageEncoder encoder = null;

            extension = extension.Replace(".", "");

            var isSupported = Regex.IsMatch(extension, "gif|png|jpe?g", RegexOptions.IgnoreCase);

            if (isSupported)
            {
                switch (extension)
                {
                    case "png":
                        encoder = new PngEncoder();
                        break;
                    case "jpg":
                        encoder = new JpegEncoder();
                        break;
                    case "jpeg":
                        encoder = new JpegEncoder();
                        break;
                    case "gif":
                        encoder = new GifEncoder();
                        break;
                    default:
                        break;
                }
            }

            return encoder;
        }
    }
}
