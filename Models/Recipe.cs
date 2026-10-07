using System.ComponentModel.DataAnnotations;

namespace Chef_Board_Desafio3.Models;

public class Recipe
{
    public int Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Range(1, 1440)]
    public int PreparationTimeMinutes { get; set; }

    public ICollection<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

    public ICollection<PreparationStep> PreparationSteps { get; set; } = new List<PreparationStep>();
}
