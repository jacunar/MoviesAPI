namespace MoviesAPI.DTOs; 
public class ReviewDTO {
    public int Id { get; set; }
    public string Comentario { get; set; } = string.Empty;
    public int Puntuacion { get; set; }
    public int MovieId { get; set; }
    public string UsuarioId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
}