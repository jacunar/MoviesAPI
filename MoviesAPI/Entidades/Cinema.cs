using NetTopologySuite.Geometries;

namespace MoviesAPI.Entidades; 
public class Cinema: IId {
    public int Id { get; set; }
    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;
    public List<MoviesCinemas> MoviesCinemas { get; set; } = null!;
    public Point? Location { get; set; }
}