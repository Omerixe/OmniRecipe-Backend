using Microsoft.AspNetCore.Mvc;
using MyRecipesApi.Dto;
using MyRecipesApi.Models;
using MyRecipesApi.Services;

namespace MyRecipesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeSummaryController : ControllerBase
    {
        private readonly RecipeService _recipeService;

        public RecipeSummaryController(RecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        [HttpGet]
        public async Task<List<RecipeSummaryDto>> GetRecipes()
        {
            var recipes = await _recipeService.GetRecipesAsync();

            var recipeDtos = recipes.Select(recipe => new RecipeSummaryDto
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Subtitle = recipe.Subtitle
            }).ToList();

            return recipeDtos;
        }
    }
}
