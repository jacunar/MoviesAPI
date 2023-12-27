namespace MoviesAPI.Entidades; 
public class MoviesGenres {
    public int GenreId { get; set; }
    public int MovieId { get; set; }
    public Genre Genre { get; set; } = null!;
    public Movie Movie { get; set; } = null!;
}