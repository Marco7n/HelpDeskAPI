using System.Collections.ObjectModel;
namespace HelpDeskAPI.Models

{
    // TODO Marco: conferir os campos com o banco e alinhar o relacionamento com Usuario.
    public class Chamado
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Prioridade { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
        public int UsuarioId { get; set; }

        public Usuario Usuario { get; set; } = null!;
      
        public ICollection<Interacao> Interacoes { get; set; } = new List<Interacao>();
    }
}

