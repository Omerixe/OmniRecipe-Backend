

namespace OmniRecipesApi.Services
{
    public interface IImageService
    {
        public Task<string?> UploadImage(IFormFile formFile);

        public Task DeleteImage(string imageUrl);
        
    }
}