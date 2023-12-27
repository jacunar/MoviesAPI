﻿namespace MoviesAPI.Entidades; 
public class Movie {
    public int Id { get; set; }
    [Required]
    [StringLength(300)]
    public string Title { get; set; } = string.Empty;
    public bool OnCimena { get; set; }
    public DateTime Premiere { get; set; }
    public string Poster { get; set; } = string.Empty;
    public List<MoviesActors> MoviesActors { get; set; } = null!;
    public List<MoviesGenres> MoviesGenres { get; set; } = null!;
}