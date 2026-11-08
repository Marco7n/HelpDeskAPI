using HelpDeskAPI.Data;
using HelpDeskAPI.Models;
using Microsoft.EntityFrameworkCore;
using HelpDeskAPI.DTOs;

namespace HelpDeskAPI.DAOs
{
    // TODO Marco: receber HelpDeskContext no construtor e implementar as operacoes de InteracaoDao.
    // Implementar uma operacao por vez, persistindo e consultando pelo contexto.
    public class UsuarioDao 
    {
        
         private readonly HelpDeskContext _contexto;
        
        public UsuarioDao (HelpDeskContext contexto )
        {
            _contexto = contexto;
           
        }
        
        public async Task AdicionarAsync(Usuario usuario)
        {
            
            await _contexto.Usuarios.AddAsync(usuario);
            await _contexto.SaveChangesAsync();
         
        }
        public async Task<Usuario?> BuscarPorIdAsync(int id) {
            
           var encontrar = await _contexto.Usuarios.FindAsync(id);
            return encontrar;
        }
        public async Task<List<Usuario>> ListaTodosAsync() { 
            var lista = await _contexto.Usuarios.ToListAsync();
            return lista;
        }
        public async Task<bool> AtualizarAsync(int id, Usuario dadosAtualizados)
        {
            var find = await _contexto.Usuarios.FindAsync(id);
            if (find == null)
            {
                return false;
            }

          
            await _contexto.SaveChangesAsync();
            return true;


        }
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
        public async Task<List<Usuario>> BuscarPorStatusAsync(string nome)
        {
            var BuscaStatus = await _contexto.Usuarios
                .Where(s => s.Nome == nome)
                .ToListAsync();

            return BuscaStatus;


        }

        public async Task<List<Usuario>> BuscarNomeAsync(string nome)
        {

            var BuscaNome = await _contexto.Usuarios
                   .Where(p => p.Nome == nome)
                   .ToListAsync();

            return BuscaNome;


        }
    }
}
