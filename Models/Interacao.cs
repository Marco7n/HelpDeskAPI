using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HelpDeskAPI.Models
{
    // Entidade que registra uma mensagem vinculada a um chamado.
    public class Interacao
    {
        // Identificador da interação.
        public int Id { get; set; }
        // Texto obrigatório da mensagem, com limite de tamanho.
        [Required(ErrorMessage = "A mensagem é obrigatória.")]
        [StringLength(4000)]
        public string Mensagem { get; set; } = string.Empty;
        // Data preenchida pelo banco no momento da inclusão.
        public DateTime DataCriacao { get; set; }

        // Chamado ao qual a mensagem pertence.
        [Range(1, int.MaxValue)]
        public int ChamadoId { get; set; }

        // Usuário responsável por criar a interação.
        public int UsuarioId { get; set; }

        // Navegação omitida das respostas JSON para evitar ciclos.
        [JsonIgnore]
        public Chamado? Chamado { get; set; }

        [JsonIgnore]
        public Usuario? Usuario { get; set; }
    }
}
