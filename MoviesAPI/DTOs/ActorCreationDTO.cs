using MoviesAPI.Validations;

namespace MoviesAPI.DTOs;
public class ActorCreationDTO: ActorPatchDTO {
    [FileWeightValidation(4)]
    [FileTypeValidation(FileTypeGroup.Picture)]
    public IFormFile? Picture { get; set; }
}