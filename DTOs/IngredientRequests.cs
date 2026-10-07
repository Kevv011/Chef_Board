using System.ComponentModel.DataAnnotations;

namespace Chef_Board_Desafio3.DTOs;

public class IngredientRequest
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "999999.99")]
    public decimal Quantity { get; set; }

    [Required]
    [StringLength(50)]
    public string UnitOfMeasure { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int RecipeId { get; set; }
}
