namespace HelpDeskAPI.Models
{
    // TODO Marco: alinhar AutorId com o Id de Usuario e configurar o relacionamento do autor.
    // TODO Marco: conferir o mapeamento de DataCriacao e ChamadoId com o banco existente.
    public class Interacao
    {
        public int Id { get; set; }
        public String Mensagem { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
        public int ChamadoId { get; set; }
        public Chamado Chamado { get; set; } = null!;
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

    }
}
