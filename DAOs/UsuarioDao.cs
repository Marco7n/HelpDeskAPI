using HelpDeskAPI.Data;
using HelpDeskAPI.Models;
using Microsoft.EntityFrameworkCore;
using HelpDeskAPI.DTOs;

namespace HelpDeskAPI.DAOs
{
    // TODO Marco: receber HelpDeskContext no construtor e implementar as operacoes de InteracaoDao.
    // Implementar uma operacao por vez, persistindo e consultando pelo contexto.
    // Camada de persistência responsável pelas operações de Usuario.
    public class UsuarioDao 
    {
         // Contexto EF Core usado para consultar e salvar usuários.
         private readonly HelpDeskContext _contexto;
        
        // Recebe o contexto por injeção de dependência.
        public UsuarioDao (HelpDeskContext contexto )
        {
            _contexto = contexto;
           
        }
        
        // Insere o usuário e dispara a validação de unicidade do e-mail no banco.
        public async Task AdicionarAsync(Usuario usuario)
        {
            
            await _contexto.Usuarios.AddAsync(usuario);
            await _contexto.SaveChangesAsync();
         
        }
        // Busca um usuário pela chave primária; retorna null se não existir.
        public async Task<Usuario?> BuscarPorIdAsync(int id) {
            
           var encontrar = await _contexto.Usuarios.FindAsync(id);
            return encontrar;
        }
        // Lista todas as entidades; o controller converte-as para DTOs públicos.
        public async Task<List<Usuario>> ListaTodosAsync() { 
            var lista = await _contexto.Usuarios.ToListAsync();
            return lista;
        }
        // Atualiza somente nome e e-mail do usuário encontrado.
        public async Task<bool> AtualizarAsync(int id, Usuario dadosAtualizados)
        {
            var usuario = await _contexto.Usuarios.FindAsync(id);
            
            if (usuario == null)
            {
                return false;
            }

            usuario.Nome = dadosAtualizados.Nome;
                usuario.Email = dadosAtualizados.Email;
            
            await _contexto.SaveChangesAsync();
            return true;


        }
        // Remove o usuário; chaves relacionadas podem gerar conflito controlado.
        public async Task<bool> ExcluirAsync(int id)
        {

            var Encontrar = await _contexto.Usuarios.FindAsync(id);
            if (Encontrar == null)
            {
                return false;
            }

            _contexto.Usuarios.Remove(Encontrar);
            await _contexto.SaveChangesAsync();
            return true;
        }
        // Mantém o nome legado do método, mas filtra pelo Perfil da entidade Usuario.
        public async Task<List<Usuario>> BuscarPorStatusAsync(string perfil)
        {
            var buscaPerfil = await _contexto.Usuarios
                .Where(usuario => usuario.Perfil == perfil)
                .ToListAsync();

            return buscaPerfil;
        }

        // Pesquisa usuários cujo nome seja exatamente o valor informado.
        public async Task<List<Usuario>> BuscarNomeAsync(string nome)
        {

            var BuscaNome = await _contexto.Usuarios
                   .Where(p => p.Nome == nome)
                   .ToListAsync();

            return BuscaNome;


        }
    }
}
