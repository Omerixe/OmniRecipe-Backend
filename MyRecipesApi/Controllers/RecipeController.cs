using Microsoft.AspNetCore.Mvc;
using MyRecipesApi.Dto;
using MyRecipesApi.Models;
using MyRecipesApi.Services;
using System;

namespace MyRecipesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly RecipeService _recipeService;

        public RecipeController(RecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        [HttpGet("{id}")]
        public ActionResult<RecipeDto> Get(string id)
        {
            var recipe = _recipeService.GetRecipe(id);

            if (recipe == null)
            {
                return NotFound();
            }

            var recipeDto = new RecipeDto
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Subtitle = recipe.Subtitle,
                Ingredients = recipe.Ingredients.Select(ingredient => new IngredientDto
                {
                    Name = ingredient.Name,
                    Quantity = ingredient.Quantity,
                    Unit = ingredient.Unit
                }).ToList(),
                Steps = recipe.Steps,
                Version = recipe.Version
            };

            return recipeDto;
        }
    }
}
