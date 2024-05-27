using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace Foodbook.Models;
public class MealPlanItemModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [ForeignKey("MealPlan")]
    public int MealPlanId { get; set; }

    [Required]
    [ForeignKey("Recipe")]
    public int RecipeId { get; set; }

    [Required]
    [EnumDataType(typeof(DayOfWeek))]
    public DayOfWeek DayOfWeek { get; set; } 

    [Required]
    [ForeignKey("RecipeCategory")]
    public int CategoryId { get; set; }

}

