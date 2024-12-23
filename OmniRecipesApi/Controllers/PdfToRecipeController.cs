using Microsoft.AspNetCore.Mvc;
using OmniRecipesApi.Dto;
using OmniRecipesApi.Models;
using OmniRecipesApi.Services;
using System;

namespace OmniRecipesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfToRecipeController : ControllerBase
    {

        private readonly PdfToRecipeService _pdfToRecipeService;

        public PdfToRecipeController(PdfToRecipeService pdfToRecipeService)
        {
            _pdfToRecipeService = pdfToRecipeService;
        }

        [HttpPost("convert")]
        public async Task<IActionResult> ConvertPdfToRecipe( IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            try
            {
                var names = await _pdfToRecipeService.ConvertAsync(file);
                return Ok(new { status = "success", data = names });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = ex.Message });
            }
        }


    }
}