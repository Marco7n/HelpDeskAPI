namespace HelpDeskAPI.Models
{
    public class Interacao
    {
        public int Id { get; set; }
        public String Mensagem { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
        public String AutorId { get; set; } = string.Empty;
        public int ChamadoId { get; set; }
        public Chamado Chamado { get; set; } = null;
    }
}
