namespace MoviesAPI.Entidades; 
public class Review: IId {
    public int Id { get; set; }
    public string Comentario { get; set; } = string.Empty;
    [Range(1, 5)]
    public int Puntuacion { get; set; }
    public int MovieId { get; set; }
    public Movie Movie { get; set; } = null!;
    public string UsuarioId { get; set; } = string.Empty;
    public IdentityUser Usuario { get; set; } = null!;
}