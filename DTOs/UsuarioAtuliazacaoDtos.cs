using System.ComponentModel.DataAnnotations;

namespace HelpDeskAPI.DTOs;

public class UsuarioAtuliazacaoDtos
{
    [Required]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(254)]
    public string Email { get; set; } = string.Empty;
}
