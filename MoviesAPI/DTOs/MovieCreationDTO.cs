using Microsoft.AspNetCore.Mvc;

namespace MoviesAPI.DTOs; 
public class MovieCreationDTO: MoviePatchDTO {
    [FileWeightValidation(4)]
    [FileTypeValidation(FileTypeGroup.Picture)]
    public IFormFile? Poster { get; set; } = null!;

    [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
    public List<int>? GenresIDs { get; set; }

    [ModelBinder(BinderType = typeof(TypeBinder<List<ActorMoviesCreationDTO>>))]
    public List<ActorMoviesCreationDTO>? Actors { get; set; }
}