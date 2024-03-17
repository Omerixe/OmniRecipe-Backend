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
        public ActionResult<RecipeDto> GetRecipe(string id)
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
                Version = recipe.Version,
                ImageUrl = recipe.ImageUrl
            };

            return recipeDto;
        }

        [HttpGet]
        public async Task<ActionResult<List<RecipeDto>>> GetCategories()
        {
            var recipes = await _recipeService.GetRecipesAsync();

            var recipeDtos = recipes.Select(recipe => new RecipeDto
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
                Version = recipe.Version,
                ImageUrl = recipe.ImageUrl
            }).ToList();

            return recipeDtos;
        }

        [HttpPost]
        public async Task<IActionResult> Post(NewRecipeDto recipeDto)
        {
            var recipe = new Recipe
            {
                Title = recipeDto.Title,
                Subtitle = recipeDto.Subtitle,
                Ingredients = recipeDto.Ingredients.Select(ingredientDto => new Ingredient
                {
                    Name = ingredientDto.Name,
                    Quantity = ingredientDto.Quantity,
                    Unit = ingredientDto.Unit
                }).ToList(),
                Steps = recipeDto.Steps,
                Version = _recipeService.CurrentVersion,
                ImageUrl = ""
            };

            await _recipeService.CreateRecipe(recipe, recipeDto.Image);

            return CreatedAtAction(nameof(GetRecipe), new { id = recipe.Id }, recipe);
        }
    }
}
