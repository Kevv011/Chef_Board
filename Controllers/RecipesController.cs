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
public class RecipesController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Recipe>>> GetAll()
    {
        return Ok(await context.Recipes
            .AsNoTracking()
            .Include(recipe => recipe.Ingredients)
            .Include(recipe => recipe.PreparationSteps)
            .OrderBy(recipe => recipe.Name)
            .ToListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Recipe>> GetById(int id)
    {
        var recipe = await context.Recipes
            .AsNoTracking()
            .Include(recipe => recipe.Ingredients)
            .Include(recipe => recipe.PreparationSteps)
            .FirstOrDefaultAsync(recipe => recipe.Id == id);

        return recipe is null ? NotFound() : Ok(recipe);
    }

    [HttpPost]
    [Authorize(Roles = IdentityDataSeeder.AdministratorRole)]
    public async Task<ActionResult<Recipe>> Create(RecipeRequest request)
    {
        var recipe = new Recipe
        {
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            PreparationTimeMinutes = request.PreparationTimeMinutes
        };

        context.Recipes.Add(recipe);
        await context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = recipe.Id }, recipe);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = IdentityDataSeeder.AdministratorRole)]
    public async Task<IActionResult> Update(int id, RecipeRequest request)
    {
        var recipe = await context.Recipes.FindAsync(id);
        if (recipe is null)
        {
            return NotFound();
        }

        recipe.Name = request.Name.Trim();
        recipe.Description = request.Description?.Trim();
        recipe.PreparationTimeMinutes = request.PreparationTimeMinutes;
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = IdentityDataSeeder.AdministratorRole)]
    public async Task<IActionResult> Delete(int id)
    {
        var recipe = await context.Recipes.FindAsync(id);
        if (recipe is null)
        {
            return NotFound();
        }

        context.Recipes.Remove(recipe);
        await context.SaveChangesAsync();
        return NoContent();
    }
}
