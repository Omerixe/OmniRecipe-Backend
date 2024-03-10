using System;
using System.Collections.Generic;

namespace MyRecipesApi.Dto
{
    public class NewRecipeDto
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public ICollection<IngredientDto> Ingredients { get; set; }

        public ICollection<string> Steps { get; set; }

        public string ImageUrl { get; set; }
    }
}
