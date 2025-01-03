using System.Collections.Generic;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OmniRecipesApi.Dto;
using OmniRecipesApi.Models;
using SkiaSharp;

namespace OmniRecipesApi.Services
{
    public class PdfToRecipeService
    {

        private readonly string _tempDirectory;
        private readonly OpenAIService _openAIService;
        public PdfToRecipeService(OpenAIService openAIService)
        {
            _tempDirectory = Path.Combine(Path.GetTempPath(), "OmniRecipes");
            Directory.CreateDirectory(_tempDirectory);
            _openAIService = openAIService;
        }


        public async Task<List<String>> ConvertAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("No file uploaded.");
            }

            var imagePaths = await ConvertPdfToImagesAsync(file);
            await ProcessImagesAndCleanUpAsync(imagePaths);
            
            return imagePaths;
        }

        private async Task ProcessImagesAndCleanUpAsync(List<String> imagePaths)
        {
            try
            {
                // Example: Send images to the AI model
                foreach (var imagePath in imagePaths)
                {
                    await SendImageToModelAsync(imagePath);
                }
            }
            finally
            {
                // Ensure cleanup happens regardless of success or failure
                foreach (var imagePath in imagePaths)
                {
                    if (File.Exists(imagePath))
                    {
                        File.Delete(imagePath);
                    }
                }
            }
        }


        private async Task<List<String>> ConvertPdfToImagesAsync(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            var pdf = memoryStream.ToArray();
#pragma warning disable CA1416 // Validate platform compatibility
            var pageImages = PDFtoImage.Conversion.ToImages(pdf);
#pragma warning restore CA1416 // Validate platform compatibility

            var totalPageCount = pageImages.Count();

            double maxImageCount = 25;
            int maxSize = (int)Math.Ceiling(totalPageCount / maxImageCount);
            var pageImageGroups = new List<List<SkiaSharp.SKBitmap>>();
            for (int i = 0; i < totalPageCount; i += maxSize)
            {
                var pageImageGroup = pageImages.Skip(i).Take(maxSize).ToList();
                pageImageGroups.Add(pageImageGroup);
            }

            var pdfImageFiles = new List<String>();
            string pdfName = Guid.NewGuid().ToString() + "_" + file.FileName;

            var count = 0;
            foreach (var pageImageGroup in pageImageGroups)
            {
                var pdfImageName = $"{pdfName}.Part_{count}.jpg";

                int totalHeight = pageImageGroup.Sum(image => image.Height);
                int width = pageImageGroup.Max(image => image.Width);
                var stitchedImage = new SKBitmap(width, totalHeight);
                var canvas = new SKCanvas(stitchedImage);
                int currentHeight = 0;
                foreach (var pageImage in pageImageGroup)
                {
                    canvas.DrawBitmap(pageImage, 0, currentHeight);
                    currentHeight += pageImage.Height;
                }
                using (var stitchedFileStream = new FileStream(pdfImageName, FileMode.Create, FileAccess.Write))
                {
                    stitchedImage.Encode(stitchedFileStream, SKEncodedImageFormat.Jpeg, 100);
                }
                pdfImageFiles.Add(pdfImageName);
                count++;

                Console.WriteLine($"Saved image to {pdfImageName}");
            }
            return pdfImageFiles;
        }

        private async Task SendImageToModelAsync(string imagePath)
        {
            await _openAIService.GetCompletionAsync(new List<string> { imagePath });
        }
    }
}
