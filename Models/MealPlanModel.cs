using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace Foodbook.Models;
public class MealPlanModel
{
    [Key] 
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public int Id { get; set; }
    [Required]
    [ForeignKey("User")]
    public int UserId { get; set; }
    [Required]
    [DataType(DataType.Date)]
    public DateTime WeekStartDate { get; set; } 
    
    public ICollection<MealPlanItemModel>? MealPlanItems { get; set; }
}
