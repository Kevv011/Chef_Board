using System.ComponentModel.DataAnnotations;

namespace Chef_Board_Desafio3.DTOs;

public class RecipeRequest
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Range(1, 1440)]
    public int PreparationTimeMinutes { get; set; }
}
