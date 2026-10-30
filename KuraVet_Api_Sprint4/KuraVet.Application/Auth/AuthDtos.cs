using System.ComponentModel.DataAnnotations;

namespace KuraVet.Application.Auth;

public sealed class LoginRequest
{
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    public string Senha { get; set; } = string.Empty;
}

/// Autocadastro: cria um usuário com perfil Tutor.
public class RegistroRequest
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "E-mail inválido.")]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [MinLength(8, ErrorMessage = "A senha deve ter pelo menos 8 caracteres.")]
    [MaxLength(72, ErrorMessage = "A senha deve ter no máximo 72 caracteres.")]
    public string Senha { get; set; } = string.Empty;
}

/// Cadastro feito por um Admin, permitindo escolher o perfil.
public sealed class CriarUsuarioRequest : RegistroRequest
{
    /// Admin, Veterinario ou Tutor.
    [Required(ErrorMessage = "O perfil é obrigatório.")]
    public string Perfil { get; set; } = string.Empty;
}

public sealed class UsuarioResponse
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Perfil { get; set; } = string.Empty;
}

public sealed class TokenResponse
{
    public string AccessToken { get; set; } = string.Empty;

    public string TokenType { get; set; } = "Bearer";

    public DateTime ExpiraEm { get; set; }

    public UsuarioResponse Usuario { get; set; } = new();
}
