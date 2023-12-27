namespace MoviesAPI.DTOs; 
public class ActorPatchDTO {
    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;
    public DateTime Birthdate { get; set; }
}