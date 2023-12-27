namespace MoviesAPI.DTOs; 
public class MovieDTO {
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool OnCimena { get; set; }
    public DateTime Premiere { get; set; }
    public string Poster { get; set; } = string.Empty;
}