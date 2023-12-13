using System.ComponentModel.DataAnnotations;

public class GenreDTO
{
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
}