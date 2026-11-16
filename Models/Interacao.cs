using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HelpDeskAPI.Models
{
    public class Interacao
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "A mensagem é obrigatória.")]
        [StringLength(4000)]
        public string Mensagem { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }

        [Range(1, int.MaxValue)]
        public int ChamadoId { get; set; }

        public int UsuarioId { get; set; }

        [JsonIgnore]
        public Chamado? Chamado { get; set; }

        [JsonIgnore]
        public Usuario? Usuario { get; set; }
    }
}
