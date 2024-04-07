using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;

namespace OmniRecipesApi.Services
{
    public class AzureService
    {
        private BlobServiceClient _blobServiceClient;
        public AzureService(IOptions<AzureSettings> azureSettings)
        {
            _blobServiceClient = new BlobServiceClient(
                new Uri(azureSettings.Value.StorageEndpoint),
                new DefaultAzureCredential());
        }

        public async Task<String?> UploadImage(IFormFile formFile) 
        {
            if (formFile.ContentType != "image/jpeg" && formFile.ContentType != "image/png")
            {
                return null;
            }

            var containerClient = _blobServiceClient.GetBlobContainerClient("images");
            var blobClient = containerClient.GetBlobClient(Guid.NewGuid().ToString() + Path.GetExtension(formFile.FileName));
            await blobClient.UploadAsync(formFile.OpenReadStream(), true);
            blobClient.SetHttpHeaders(new BlobHttpHeaders() { ContentType = formFile.ContentType });

            return blobClient.Uri.ToString();
        }

    }

}