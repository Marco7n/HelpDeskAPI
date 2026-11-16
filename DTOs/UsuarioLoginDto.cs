namespace HelpDeskAPI.DTOs;

public class UsuarioLoginDto
{
    // E-mail usado para localizar a conta.
    public string Email { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}