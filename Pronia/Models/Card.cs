using System.ComponentModel.DataAnnotations;

namespace Pronia.Models;

public class Card
{   //[Key]
    public int Id { get; set; }
    /*[MaxLength(255)]
    [MinLength(3,ErrorMessage ="Length is minimum 3")]*/
    public string Title { get; set; } = null!;
    /*[Required(ErrorMessage ="Field must not be empty!")]*/
    public string Description { get; set; } = null!;
    public string ImagePath { get; set; } = null!;


}
