namespace MoviesAPI.Entidades; 
public class MoviesActors {
    public int ActorId { get; set; }
    public int MovieId { get; set; }
    public string Character { get; set; } = string.Empty;
    public int Orden { get; set; }
    public Actor Actor { get; set; } = null!;
    public Movie Movie { get; set; } = null!;
}