using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Foodbook.Models;

public class RatingModel{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RatingId {get; set;}

    [Required]
    [ForeignKey("Recipe")]
    public int RecipeId {get; set;}

    [Required]
    [ForeignKey("User")]
    public int UserId {get; set;}

    [Required]
    public int Rate {get; set;}

    [StringLength(200)]
    public string Comment {get; set;} = "";

   // [Required]
    [DataType(DataType.Date)]
    public DateTime RatingDate {get; set;} = DateTime.Now;

}

