using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Foodbook.Models;

public class RecipeModel{
    [Key] 
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public int RecipeId { get; set; }

    [Required]
    [StringLength(50)]
    public string Title { get; set; } = "";

    [Required]
    [StringLength(300)]
    public string Description { get; set; } = "";

    [Required]
    [StringLength(300)]
    public string Ingridients { get; set; } = "";

    [Required]
    [StringLength(1000)]
    public string Instructions { get; set; } = "";

    [Required]
    [ForeignKey("RecipeCategory")]
    public int Category { get; set; }

    [Required]
    [ForeignKey("User")]
    public int UserId{get; set;}

    [Required]
    [StringLength(50)]
    public string userName {get; set;} = "";

   
    [DataType(DataType.Date)]
    public DateTime PublicationDate { get; set; } = DateTime.Now;

}