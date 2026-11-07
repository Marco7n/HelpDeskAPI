using System.Text.Json.Serialization;

namespace HelpDeskAPI.Models
{
    public class Interacao
    {
        public int Id { get; set; }
        public String Mensagem { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
        public int ChamadoId { get; set; }
        public int UsuarioId { get; set; }

        [JsonIgnore]
        public Chamado? Chamado { get; set; }

        [JsonIgnore]
        public Usuario? Usuario { get; set; }
    }
}
