using MoviesAPI.Validations;

namespace MoviesAPI.DTOs;
public class ActorCreationDTO {
    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;
    public DateTime Birthdate { get; set; }
    [FileWeightValidation(4)]
    [FileTypeValidation(FileTypeGroup.Picture)]
    public IFormFile Picture { get; set; } = null!;
}