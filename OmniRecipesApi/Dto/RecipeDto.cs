using System;
using System.Collections.Generic;

namespace OmniRecipesApi.Dto
{
    public class RecipeDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public ICollection<IngredientDto> Ingredients { get; set; }

        public ICollection<string> Steps { get; set; }
        public int Version { get; set; }

        public string ImageUrl { get; set; }
    }
}
