namespace MoviesAPI.Validations; 
public class FileWeightValidation: ValidationAttribute {
    private readonly int megabytesMaxPictureWeight;

    public FileWeightValidation(int MegabytesMaxPictureWeight) {
        megabytesMaxPictureWeight = MegabytesMaxPictureWeight;
    }
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) {
        if (value == null)
            return ValidationResult.Success;

        IFormFile formFile = value as IFormFile;
        if (formFile == null)
            return ValidationResult.Success;

        if (formFile.Length > megabytesMaxPictureWeight * 1024 * 1024)
            return new ValidationResult($"El peso del archivo no debe ser mayor a {megabytesMaxPictureWeight}mb");

        return ValidationResult.Success;
    }
}