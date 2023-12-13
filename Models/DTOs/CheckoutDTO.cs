using System.ComponentModel.DataAnnotations;
using Loncotes.Models;


public class CheckoutDTO 
{
    public int Id { get; set; }
    [Required]
    public int MaterialId { get; set; }
    public Material Material { get; set; }
    [Required]
    public int PatronId { get; set; }
    public Patron Patron { get; set; }
    public DateTime CheckoutDate { get; set; }
    public DateTime? ReturnDate { get; set; }
}