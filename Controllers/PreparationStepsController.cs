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
public class PreparationStepsController(ApplicationDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PreparationStep>>> GetAll()
    {
        return Ok(await context.PreparationSteps
            .AsNoTracking()
            .OrderBy(step => step.RecipeId)
            .ThenBy(step => step.StepOrder)
            .ToListAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PreparationStep>> GetById(int id)
    {
        var step = await context.PreparationSteps.AsNoTracking().FirstOrDefaultAsync(step => step.Id == id);
        return step is null ? NotFound() : Ok(step);
    }

    [HttpPost]
    [Authorize(Roles = IdentityDataSeeder.AdministratorRole)]
    public async Task<ActionResult<PreparationStep>> Create(PreparationStepRequest request)
    {
        if (!await RecipeExistsAsync(request.RecipeId))
        {
            return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]> { [nameof(request.RecipeId)] = ["The specified recipe does not exist."] }));
        }

        if (await OrderAlreadyExistsAsync(request.RecipeId, request.StepOrder))
        {
            return Conflict("A preparation step with this order already exists for the specified recipe.");
        }

        var step = new PreparationStep { Description = request.Description.Trim(), StepOrder = request.StepOrder, RecipeId = request.RecipeId };
        context.PreparationSteps.Add(step);
        await context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = step.Id }, step);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = IdentityDataSeeder.AdministratorRole)]
    public async Task<IActionResult> Update(int id, PreparationStepRequest request)
    {
        var step = await context.PreparationSteps.FindAsync(id);
        if (step is null)
        {
            return NotFound();
        }

        if (!await RecipeExistsAsync(request.RecipeId))
        {
            return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]> { [nameof(request.RecipeId)] = ["The specified recipe does not exist."] }));
        }

        if (await OrderAlreadyExistsAsync(request.RecipeId, request.StepOrder, id))
        {
            return Conflict("A preparation step with this order already exists for the specified recipe.");
        }

        step.Description = request.Description.Trim();
        step.StepOrder = request.StepOrder;
        step.RecipeId = request.RecipeId;
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = IdentityDataSeeder.AdministratorRole)]
    public async Task<IActionResult> Delete(int id)
    {
        var step = await context.PreparationSteps.FindAsync(id);
        if (step is null)
        {
            return NotFound();
        }

        context.PreparationSteps.Remove(step);
        await context.SaveChangesAsync();
        return NoContent();
    }

    private Task<bool> RecipeExistsAsync(int recipeId) => context.Recipes.AnyAsync(recipe => recipe.Id == recipeId);

    private Task<bool> OrderAlreadyExistsAsync(int recipeId, int stepOrder, int? excludedId = null) =>
        context.PreparationSteps.AnyAsync(step =>
            step.RecipeId == recipeId &&
            step.StepOrder == stepOrder &&
            (!excludedId.HasValue || step.Id != excludedId.Value));
}
