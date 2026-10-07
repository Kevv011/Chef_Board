using System.ComponentModel.DataAnnotations;

namespace Chef_Board_Desafio3.DTOs;

public class PreparationStepRequest
{
    [Required]
    [MinLength(10)]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int StepOrder { get; set; }

    [Range(1, int.MaxValue)]
    public int RecipeId { get; set; }
}
