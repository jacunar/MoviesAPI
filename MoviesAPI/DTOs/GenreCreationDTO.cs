using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs; 
public class GenreCreationDTO {
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
}