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

        public RecipeService(IOptions<RecipesDatabaseSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);

            _recipesCollection = database.GetCollection<Recipe>(settings.Value.RecipesCollectionName);
        }

        public async Task<List<RecipeSummaryDto>> GetRecipesAsync()
        {
            var recipes = await _recipesCollection.Find(recipe => true).ToListAsync();
            return recipes.Select(recipe => new RecipeSummaryDto
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Subtitle = recipe.Subtitle,
            }).ToList();
        }

        public Recipe GetRecipe(string id) =>
            _recipesCollection.Find<Recipe>(recipe => recipe.Id == id).FirstOrDefault();
    }
}
