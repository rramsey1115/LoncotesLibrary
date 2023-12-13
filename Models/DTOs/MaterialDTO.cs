using System.ComponentModel.DataAnnotations;
using Loncotes.Models;

public class MaterialDTO
{
    public int Id { get; set; }
    [Required]
    public string MaterialName { get; set; }
    [Required]
    public int MaterialTypeId { get; set; }
    public MaterialTypeDTO MaterialType { get; set; }
    [Required]
    public int GenreId { get; set; }
    public GenreDTO Genre { get; set; }
    public DateTime? OutOfCirculationSince { get; set; }
}