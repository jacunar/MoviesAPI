namespace MoviesAPI.DTOs; 
public class CinemaDTO {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class CinemaCreationDTO {
    public string Name { get; set; } = string.Empty;
    [Range(-80, 80)]
    public double Latitude { get; set; }
    [Range(-120, 120)]
    public double Longitude { get; set; }
}