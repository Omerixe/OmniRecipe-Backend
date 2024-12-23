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
        public PdfToRecipeService()
        {
            _tempDirectory = Path.Combine(Path.GetTempPath(), "OmniRecipes");
            Directory.CreateDirectory(_tempDirectory);
        }

        /*public async Task<NewRecipeDto> ConvertAsync(IFormFile file) 
        {
            var images = await ConvertPdfToImagesAsync(file);
        
        }*/


        public async Task<List<String>> ConvertPdfToImagesAsync(IFormFile file)
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
            var pageImageGroups = new List<List<SkiaSharp.SKBitmap>> ();
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
    }
}
