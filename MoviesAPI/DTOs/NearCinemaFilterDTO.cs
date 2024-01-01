namespace MoviesAPI.DTOs; 
public class NearCinemaFilterDTO {
    [Range(-100, 100)]
    public double Latitude { get; set; }
    [Range(-20, 20)]
    public double Longitude { get; set; }
    private int distanceInKms { get; set; } = 10;
    private int maxKmsDistance = 50;
    public int DistanceInKms {
        get { return distanceInKms; }

        set { distanceInKms = (value > maxKmsDistance) ? maxKmsDistance : value; }
    }
}