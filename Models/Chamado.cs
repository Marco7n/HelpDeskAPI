using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HelpDeskAPI.Models

{
    // Entidade que representa uma solicitação de atendimento.
    public class Chamado
    {
        // Identificador do chamado.
        public int Id { get; set; }
        // Título obrigatório e limitado para evitar entradas excessivamente grandes.
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
        // Preenchido pelo SQLite com CURRENT_TIMESTAMP quando inserido.
        public DateTime DataCriacao { get; set; }

        // Chave estrangeira do usuário que abriu o chamado.
        public int UsuarioId { get; set; }

        // Navegação ignorada no JSON para evitar expor ou serializar ciclos.
        [JsonIgnore]
        public Usuario? Usuario { get; set; }

        // Interações associadas; também não são serializadas neste modelo.
        [JsonIgnore]
        public ICollection<Interacao> Interacoes { get; set; } = new List<Interacao>();
    }
}

