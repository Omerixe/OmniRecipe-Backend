using System.Collections.Generic;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MyRecipesApi.Dto;
using MyRecipesApi.Models;

namespace MyRecipesApi.Services
{
    public class RecipeService
    {
        private readonly IMongoCollection<Recipe> _recipesCollection;
        private FirebaseService _firebaseService;
        public readonly int CurrentVersion = 1;

        public RecipeService(IOptions<RecipesDatabaseSettings> settings, FirebaseService firebaseService)
        {
            _firebaseService = firebaseService;
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
                var url = await _firebaseService.UploadImage(imageFile);
                if (url != null)
                {
                    recipe.ImageUrl = url;
                }
            }

            await _recipesCollection.InsertOneAsync(recipe);
        }
            
            
    }
}
