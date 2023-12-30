namespace MoviesAPI.DTOs; 
public class UserToken {
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
}

public class UserInfo {
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}

public class UsuarioDTO {
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class EditarRolDTO {
    public string UsuarioId { get; set; } = string.Empty;
    public string NombreRol { get; set; } = string.Empty;
}