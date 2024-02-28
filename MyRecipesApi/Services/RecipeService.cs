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
        public Recipe GetRecipe()
        {
            return new Recipe
            {
                Id = "1",
                Title = "Spaghetti Bolognese",
                Subtitle = "A classic Italian dish",
                Ingredients = 
                [
                    new Ingredient { Name = "Spaghetti", Quantity = 200, Unit = "g" },
                    new Ingredient { Name = "Minced meat", Quantity = 400, Unit = "g" },
                    new Ingredient { Name = "Onion", Quantity = 1, Unit = "pc." },
                    new Ingredient { Name = "Garlic", Quantity = 2, Unit = "pc." },
                    new Ingredient { Name = "Tomato sauce", Quantity = 500, Unit = "ml" }
                ],
                Steps =
                [
                    "Boil the spaghetti",
                    "Fry the minced meat",
                    "Add the onion and garlic to the meat",
                    "Add the tomato sauce to the meat",
                    "Serve the meat sauce on top of the spaghetti"
                ],
                Version = 1
            };
        }
    }
}
