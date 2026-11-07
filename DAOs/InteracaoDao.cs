using HelpDeskAPI.Data;
using HelpDeskAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskAPI.DAOs
{
    public class InteracaoDao
    {
        private readonly HelpDeskContext _contexto;

        public InteracaoDao(HelpDeskContext contexto)
        {
            _contexto = contexto;
        }

        public async Task AdicionarAsync(Interacao interacao)
        {
            _contexto.Interacoes.Add(interacao);
            await _contexto.SaveChangesAsync();
        }

        public async Task<Interacao?> BuscarPorIdAsync(int id)
        {
            return await _contexto.Interacoes.FindAsync(id);
        }

        public Task<List<Interacao>> ListaTodosAsync()
        {
            return _contexto.Interacoes.ToListAsync();
        }

        public async Task<bool> AtualizarAsync(int id, Interacao dadosAtualizados)
        {
            var interacao = await _contexto.Interacoes.FindAsync(id);
            if (interacao == null)
            {
                return false;
            }

            interacao.Mensagem = dadosAtualizados.Mensagem;
            await _contexto.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExcluirAsync(int id)
        {
            var interacao = await _contexto.Interacoes.FindAsync(id);
            if (interacao == null)
            {
                return false;
            }

            _contexto.Interacoes.Remove(interacao);
            await _contexto.SaveChangesAsync();
            return true;
        }

        public Task<List<Interacao>> BuscarPorChamadoAsync(int chamadoId)
        {
            return _contexto.Interacoes.Where(item => item.ChamadoId == chamadoId).ToListAsync();
        }

        public Task<List<Interacao>> BuscarPorUsuarioAsync(int usuarioId)
        {
            return _contexto.Interacoes.Where(item => item.UsuarioId == usuarioId).ToListAsync();
        }
    }
}
