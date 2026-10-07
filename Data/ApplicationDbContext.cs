using Chef_Board_Desafio3.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chef_Board_Desafio3.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Recipe> Recipes => Set<Recipe>();
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<PreparationStep> PreparationSteps => Set<PreparationStep>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Recipe>(entity =>
        {
            entity.Property(recipe => recipe.Name).HasMaxLength(100).IsRequired();
            entity.Property(recipe => recipe.Description).HasMaxLength(1000);
            entity.HasData(
                new Recipe { Id = 1, Name = "Caesar Salad", Description = "Classic salad with chicken, romaine lettuce, and Caesar dressing.", PreparationTimeMinutes = 20 },
                new Recipe { Id = 2, Name = "Pasta Carbonara", Description = "Pasta with cream sauce, egg, and Parmesan cheese.", PreparationTimeMinutes = 30 },
                new Recipe { Id = 3, Name = "Tomato Soup", Description = "Light tomato soup with basil.", PreparationTimeMinutes = 40 });
        });

        builder.Entity<Ingredient>(entity =>
        {
            entity.Property(ingredient => ingredient.Name).HasMaxLength(50).IsRequired();
            entity.Property(ingredient => ingredient.UnitOfMeasure).HasMaxLength(50).IsRequired();
            entity.Property(ingredient => ingredient.Quantity).HasPrecision(10, 2);
            entity.HasOne(ingredient => ingredient.Recipe)
                .WithMany(recipe => recipe.Ingredients)
                .HasForeignKey(ingredient => ingredient.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasData(
                new Ingredient { Id = 1, RecipeId = 1, Name = "Romaine lettuce", Quantity = 1, UnitOfMeasure = "Unit" },
                new Ingredient { Id = 2, RecipeId = 1, Name = "Grilled chicken", Quantity = 200, UnitOfMeasure = "Grams" },
                new Ingredient { Id = 3, RecipeId = 1, Name = "Caesar dressing", Quantity = 50, UnitOfMeasure = "Milliliters" },
                new Ingredient { Id = 4, RecipeId = 2, Name = "Spaghetti pasta", Quantity = 250, UnitOfMeasure = "Grams" },
                new Ingredient { Id = 5, RecipeId = 2, Name = "Heavy cream", Quantity = 100, UnitOfMeasure = "Milliliters" },
                new Ingredient { Id = 6, RecipeId = 2, Name = "Egg", Quantity = 1, UnitOfMeasure = "Unit" },
                new Ingredient { Id = 7, RecipeId = 2, Name = "Parmesan cheese", Quantity = 50, UnitOfMeasure = "Grams" },
                new Ingredient { Id = 8, RecipeId = 3, Name = "Fresh tomatoes", Quantity = 500, UnitOfMeasure = "Grams" },
                new Ingredient { Id = 9, RecipeId = 3, Name = "Basil", Quantity = 5, UnitOfMeasure = "Leaves" });
        });

        builder.Entity<PreparationStep>(entity =>
        {
            entity.Property(step => step.Description).HasMaxLength(1000).IsRequired();
            entity.HasIndex(step => new { step.RecipeId, step.StepOrder }).IsUnique();
            entity.HasOne(step => step.Recipe)
                .WithMany(recipe => recipe.PreparationSteps)
                .HasForeignKey(step => step.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasData(
                new PreparationStep { Id = 1, RecipeId = 1, StepOrder = 1, Description = "Wash the romaine lettuce and cut it into pieces." },
                new PreparationStep { Id = 2, RecipeId = 1, StepOrder = 2, Description = "Grill the chicken and cut it into strips." },
                new PreparationStep { Id = 3, RecipeId = 1, StepOrder = 3, Description = "Mix the lettuce, chicken, and Caesar dressing." },
                new PreparationStep { Id = 4, RecipeId = 2, StepOrder = 1, Description = "Cook the pasta in boiling salted water." },
                new PreparationStep { Id = 5, RecipeId = 2, StepOrder = 2, Description = "Mix the egg, cream, and Parmesan cheese." },
                new PreparationStep { Id = 6, RecipeId = 2, StepOrder = 3, Description = "Add the mixture to the hot pasta." },
                new PreparationStep { Id = 7, RecipeId = 3, StepOrder = 1, Description = "Cut the tomatoes and boil them until soft." },
                new PreparationStep { Id = 8, RecipeId = 3, StepOrder = 2, Description = "Blend the tomatoes and add the basil." },
                new PreparationStep { Id = 9, RecipeId = 3, StepOrder = 3, Description = "Cook for 10 more minutes and serve hot." });
        });
    }
}
