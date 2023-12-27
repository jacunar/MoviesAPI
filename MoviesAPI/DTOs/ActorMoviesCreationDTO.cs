namespace MoviesAPI.DTOs; 
public class ActorMoviesCreationDTO {
    public int ActorId { get; set; }
    public string Character { get; set; } = string.Empty;
}