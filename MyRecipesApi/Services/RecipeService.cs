using System.Collections.Generic;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MyRecipesApi.Models;

namespace MyRecipesApi.Services
{
    public class RecipeService
    {
        private readonly IMongoCollection<Recipe> _recipesCollection;

        public RecipeService(IOptions<RecipesDatabaseSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);

            _recipesCollection = database.GetCollection<Recipe>(settings.Value.RecipesCollectionName);
        }

        public async Task<List<Recipe>> GetRecipesAsync() =>
            await _recipesCollection.Find(_ => true).ToListAsync();
    }
}
