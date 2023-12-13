using System.ComponentModel.DataAnnotations;

namespace Loncotes.Models;

public class Genre
{
    public int Id { get; set; }
    [Required]
    public string? Name { get; set; }
}