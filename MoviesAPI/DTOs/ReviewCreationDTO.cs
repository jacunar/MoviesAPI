namespace MoviesAPI.DTOs; 
public class ReviewCreationDTO {
    public string Comentario { get; set; } = string.Empty;
    [Range(1, 5)]
    public int Puntuacion { get; set; }
}