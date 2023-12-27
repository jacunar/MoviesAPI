using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Entidades; 
public class Genre {
    public int Id { get; set; }
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
    public List<MoviesGenres> MoviesGenres { get; set; } = null!;
}