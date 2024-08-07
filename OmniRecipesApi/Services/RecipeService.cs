using System.Collections.Generic;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OmniRecipesApi.Dto;
using OmniRecipesApi.Models;

namespace OmniRecipesApi.Services
{
    public class RecipeService
    {
        private readonly IMongoCollection<Recipe> _recipesCollection;
        private IImageService _imageService;
        public readonly int CurrentVersion = 1;

        public RecipeService(IOptions<RecipesDatabaseSettings> settings, IImageService imageService)
        {
            _imageService = imageService;
            var mongoClientSettings = MongoClientSettings.FromUrl(new MongoUrl(settings.Value.ConnectionString));
            mongoClientSettings.SslSettings = new SslSettings { EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12 };
            var client = new MongoClient(mongoClientSettings);
            var database = client.GetDatabase(settings.Value.DatabaseName);

            _recipesCollection = database.GetCollection<Recipe>(settings.Value.RecipesCollectionName);
        }

        public async Task<List<Recipe>> GetRecipesAsync() =>
            await _recipesCollection.Find(recipe => true).ToListAsync();


        public Recipe GetRecipe(string id) =>
            _recipesCollection.Find<Recipe>(recipe => recipe.Id == id).FirstOrDefault();

        public async Task CreateRecipe(Recipe recipe, IFormFile? imageFile = null)
        {
            if (imageFile != null)
            {
                var url = await _imageService.UploadImage(imageFile);
                if (url != null)
                {
                    recipe.ImageUrl = url;
                }
            }

            await _recipesCollection.InsertOneAsync(recipe);
        }

        public async Task DeleteRecipe(string id)
        {
            var recipe = GetRecipe(id);
            if (recipe.ImageUrl != null)
            {
                await _imageService.DeleteImage(recipe.ImageUrl);
            }
            await _recipesCollection.DeleteOneAsync(recipe => recipe.Id == id);
        }
            
            
    }
}
