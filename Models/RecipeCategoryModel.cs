using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Foodbook.Models;

public class RecipeCategoryModel{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public int RecipeCategoryId { get; set; }

    [Required]
    [StringLength(50)]
    public string CategoryName {get; set;} ="";
}