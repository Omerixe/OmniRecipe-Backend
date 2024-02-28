using Microsoft.AspNetCore.Mvc;
using MyRecipesApi.Dto;
using MyRecipesApi.Models;
using MyRecipesApi.Services;

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

        [HttpGet]
        public async Task<List<RecipeSummaryDto>> GetRecipes() =>
            await _recipeService.GetRecipesAsync();
    }
}
