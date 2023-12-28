namespace MoviesAPI.Entidades; 
public class MoviesCinemas {
    public int MovieId { get; set; }
    public int CinemaId { get; set; }
    public Movie Movie { get; set; } = null!;
    public Cinema Cinema { get; set; } = null!;
}