namespace HelpDeskAPI.Models
{
    // Entidade persistida na tabela de usuários.
    public class Usuario
    {
        // Identificador gerado pelo banco.
        public int Id { get; set; } 
        // Nome exibido para o usuário.
        public string Nome { get; set; } = string.Empty;
        // E-mail usado no cadastro e no login; o banco aplica unicidade.
        public string Email { get; set; } = string.Empty;
        // Hash da senha; nunca deve ser retornado diretamente pela API.
        public string SenhaHash { get; set; } = string.Empty;
        // Perfil usado nas regras de autorização, como Cliente ou Atendente.
        public string Perfil {  get; set; } = string.Empty;

    }
}
