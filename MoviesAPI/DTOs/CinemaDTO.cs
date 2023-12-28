namespace MoviesAPI.DTOs; 
public class CinemaDTO {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CinemaCreationDTO {
    public string Name { get; set; } = string.Empty;
}