namespace OmniRecipesApi.Services {
    public class LocalImageService: IImageService
{
    private readonly IWebHostEnvironment _environment;

    public LocalImageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string?> UploadImage(IFormFile formFile)
    {
        if (formFile == null || formFile.Length == 0)
        {
            return null;
        }

        string uploadsFolder = Path.Combine(_environment.ContentRootPath, "UploadedImages");
        string uniqueFileName = Guid.NewGuid().ToString() + "_" + formFile.FileName;
        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await formFile.CopyToAsync(fileStream);
        }

        // Return the URI where the file can be accessed
        return $"/Images/{uniqueFileName}";
    }
}

}

