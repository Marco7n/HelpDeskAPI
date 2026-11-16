using HelpDeskAPI.Data;
using HelpDeskAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskAPI.DAOs
{
    // Camada de persistência das mensagens de interação.
    public class InteracaoDao
    {
        // Contexto que executa as consultas e comandos no banco.
        private readonly HelpDeskContext _contexto;

        // Recebe o contexto por injeção de dependência.
        public InteracaoDao(HelpDeskContext contexto)
        {
            _contexto = contexto;
        }

        // Persiste uma nova interação já associada ao usuário autenticado.
        public async Task AdicionarAsync(Interacao interacao)
        {
            _contexto.Interacoes.Add(interacao);
            await _contexto.SaveChangesAsync();
        }

        // Busca uma interação individual.
        public async Task<Interacao?> BuscarPorIdAsync(int id)
        {
            return await _contexto.Interacoes.FindAsync(id);
        }

        // Lista todas as interações; o controller aplica o filtro de acesso.
        public Task<List<Interacao>> ListaTodosAsync()
        {
            return _contexto.Interacoes.ToListAsync();
        }

        // Altera somente a mensagem, preservando autor e vínculo original.
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

        // Exclui uma interação existente.
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

        // Retorna as interações pertencentes a um chamado.
        public Task<List<Interacao>> BuscarPorChamadoAsync(int chamadoId)
        {
            return _contexto.Interacoes.Where(item => item.ChamadoId == chamadoId).ToListAsync();
        }

        // Retorna as interações criadas por um usuário específico.
        public Task<List<Interacao>> BuscarPorUsuarioAsync(int usuarioId)
        {
            try
            {
                return _contexto.Interacoes.Where(item => item.UsuarioId == usuarioId).ToListAsync();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
