namespace HelpDeskAPI.DTOs;

public class UsuarioRespostaDto
{
    // Identificador público do usuário.
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Perfil { get; set; } = string.Empty;
}