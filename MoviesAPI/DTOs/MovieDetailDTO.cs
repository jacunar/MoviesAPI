namespace MoviesAPI.DTOs; 
public class MovieDetailDTO: MovieDTO {
    public List<GenreDTO> Genres { get; set; } = null!;
    public List<ActorMovieDetailDTO> Actors { get; set; } = null!;
}

public class ActorMovieDetailDTO {
    public int ActorId { get; set; }
    public string Character { get; set; } = string.Empty;
    public string ActorName { get; set; } = string.Empty;
}