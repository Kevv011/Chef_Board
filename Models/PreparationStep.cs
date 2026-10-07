using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Chef_Board_Desafio3.Models;

public class PreparationStep
{
    public int Id { get; set; }

    [Required]
    [MinLength(10)]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int StepOrder { get; set; }

    [Range(1, int.MaxValue)]
    public int RecipeId { get; set; }

    [JsonIgnore]
    public Recipe? Recipe { get; set; }
}
