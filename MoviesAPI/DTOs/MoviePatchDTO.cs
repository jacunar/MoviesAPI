namespace MoviesAPI.DTOs; 
public class MoviePatchDTO {
    [Required]
    [StringLength(300)]
    public string Title { get; set; } = string.Empty;
    public bool OnCimena { get; set; }
    public DateTime Premiere { get; set; }
}