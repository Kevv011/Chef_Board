using Chef_Board_Desafio3.Data;
using Chef_Board_Desafio3.DTOs;
using Chef_Board_Desafio3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chef_Board_Desafio3.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class IngredientsController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Ingredient>>> GetAll()
    {
        return Ok(await context.Ingredients.AsNoTracking().OrderBy(ingredient => ingredient.Name).ToListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Ingredient>> GetById(int id)
    {
        var ingredient = await context.Ingredients.AsNoTracking().FirstOrDefaultAsync(ingredient => ingredient.Id == id);
        return ingredient is null ? NotFound() : Ok(ingredient);
    }

    [HttpPost]
    [Authorize(Roles = IdentityDataSeeder.AdministratorRole)]
    public async Task<ActionResult<Ingredient>> Create(IngredientRequest request)
    {
        if (!await RecipeExistsAsync(request.RecipeId))
        {
            return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]> { [nameof(request.RecipeId)] = ["The specified recipe does not exist."] }));
        }

        var ingredient = new Ingredient
        {
            Name = request.Name.Trim(),
            Quantity = request.Quantity,
            UnitOfMeasure = request.UnitOfMeasure.Trim(),
            RecipeId = request.RecipeId
        };
        context.Ingredients.Add(ingredient);
        await context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = ingredient.Id }, ingredient);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = IdentityDataSeeder.AdministratorRole)]
    public async Task<IActionResult> Update(int id, IngredientRequest request)
    {
        var ingredient = await context.Ingredients.FindAsync(id);
        if (ingredient is null)
        {
            return NotFound();
        }

        if (!await RecipeExistsAsync(request.RecipeId))
        {
            return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]> { [nameof(request.RecipeId)] = ["The specified recipe does not exist."] }));
        }

        ingredient.Name = request.Name.Trim();
        ingredient.Quantity = request.Quantity;
        ingredient.UnitOfMeasure = request.UnitOfMeasure.Trim();
        ingredient.RecipeId = request.RecipeId;
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = IdentityDataSeeder.AdministratorRole)]
    public async Task<IActionResult> Delete(int id)
    {
        var ingredient = await context.Ingredients.FindAsync(id);
        if (ingredient is null)
        {
            return NotFound();
        }

        context.Ingredients.Remove(ingredient);
        await context.SaveChangesAsync();
        return NoContent();
    }

    private Task<bool> RecipeExistsAsync(int recipeId) => context.Recipes.AnyAsync(recipe => recipe.Id == recipeId);
}
