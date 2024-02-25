using System.Collections.Generic;
using MyRecipesApi.Models;

namespace MyRecipesApi.Services
{
    public class RecipeService
    {
        public Recipe GetRecipe()
        {
            return new Recipe
            {
                Id = 1,
                Title = "Spaghetti Bolognese",
                Subtitle = "A classic Italian dish",
                Ingredients = 
                [
                    new Ingredient { Name = "Spaghetti", Quantity = "200", Unit = Unit.Gram },
                    new Ingredient { Name = "Minced meat", Quantity = "400", Unit = Unit.Gram },
                    new Ingredient { Name = "Onion", Quantity = "1", Unit = Unit.Piece },
                    new Ingredient { Name = "Garlic", Quantity = "2", Unit = Unit.Piece },
                    new Ingredient { Name = "Tomato sauce", Quantity = "500", Unit = Unit.Milliliter }
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
