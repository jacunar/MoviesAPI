namespace MoviesAPI.Validations;
#nullable disable

public enum FileTypeGroup {
    Picture
}

public class FileTypeValidation : ValidationAttribute {
    private readonly string[] ValidTypes = new string[] { };
    public FileTypeValidation(string[] validTypes) {
        ValidTypes = validTypes;
    }

    public FileTypeValidation(FileTypeGroup fileTypeGroup) {
        if (fileTypeGroup == FileTypeGroup.Picture)
            ValidTypes = new string[] { "image/jpeg", "image/png", "image/gif", "image/jpg" };
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
        if (value == null)
            return ValidationResult.Success;

        IFormFile formFile = value as IFormFile;
        if (formFile == null)
            return ValidationResult.Success;

        if (!ValidTypes.Contains(formFile.ContentType))
            return new ValidationResult($"El tipo de archivo debe ser uno de los siguientes: {string.Join(", ", ValidTypes)}");

        return ValidationResult.Success;
    }
}