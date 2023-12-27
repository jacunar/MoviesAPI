using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Entidades; 
public class Actor {
    public int Id { get; set; }
    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;
    public DateTime Birthdate { get; set; }
    public string Picture { get; set; } = string.Empty;
    public List<MoviesActors> MoviesActors { get; set; } = null!;
}