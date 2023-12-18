using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs; 
public class GenreDTO {
    public int Id { get; set; }
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
}