namespace MoviesAPI.Validations;
public class FileTypeValidation : ValidationAttribute {
    public FileTypeValidation(string[] validTypes) {
        ValidTypes = validTypes;
    }

    private readonly string[] ValidTypes { get; set; }
}