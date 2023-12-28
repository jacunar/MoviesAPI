namespace MoviesAPI.DTOs; 
public class MoviesIndexDTO {
    public List<MovieDTO> FuturosEstrenos { get; set; } = null!;
    public List<MovieDTO> EnCines { get; set; } = null!;
}