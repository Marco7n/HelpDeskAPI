using System.ComponentModel.DataAnnotations;

namespace HelpDeskAPI.DTOs;

public class UsuarioAtuliazacaoDtos
{
    // Nome que substituirá o nome atual do usuário.
    [Required]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
