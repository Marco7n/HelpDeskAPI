using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HelpDeskAPI.Models

{
    public class Chamado
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "O título é obrigatório.")]
        [StringLength(200)]
        public string Titulo { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [StringLength(4000)]
        public string Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O status é obrigatório.")]
        [RegularExpression("Aberto|Em andamento|Concluido", ErrorMessage = "Status inválido.")]
        public string Status { get; set; } = string.Empty;

        [Required(ErrorMessage = "A prioridade é obrigatória.")]
        [RegularExpression("Baixa|Media|Alta", ErrorMessage = "Prioridade inválida.")]
        public string Prioridade { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }

        public int UsuarioId { get; set; }

        [JsonIgnore]
        public Usuario? Usuario { get; set; }

        [JsonIgnore]
        public ICollection<Interacao> Interacoes { get; set; } = new List<Interacao>();
    }
}

