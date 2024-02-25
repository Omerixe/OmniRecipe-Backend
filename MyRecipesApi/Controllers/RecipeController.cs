using Microsoft.AspNetCore.Mvc;
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

        // GET: api/Recipe/5
        [HttpGet("{id}")]
        public ActionResult<Recipe> GetRecipe(int id)
        {
            var recipe = _recipeService.GetRecipe();

            if (recipe == null)
            {
                return NotFound();
            }

            return recipe;
        }
    }
}
