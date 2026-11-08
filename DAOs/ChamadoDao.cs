using HelpDeskAPI.Data;
using HelpDeskAPI.Models;
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskAPI.DAOs
{

    public class ChamadoDao
    {

        private readonly HelpDeskContext _contexto;

        public ChamadoDao(HelpDeskContext contexto)
        {
            _contexto = contexto;

        }

        public async Task AdicionarAsync(Chamado chamado)
        {

            await _contexto.Chamados.AddAsync(chamado);
            await _contexto.SaveChangesAsync();

        }

        public async Task<Chamado?> BuscarPorIdAsync(int id)
        {

            try
            {
                var encontrar = await _contexto.Chamados.FindAsync(id);

                if (encontrar == null)
                {
                    return null;
                }

                return encontrar;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException().Message);
                throw;
            }
            
        }
    


    public async Task<List<Chamado>> ListaTodosAsync() { 
            var lista = await _contexto.Chamados.ToListAsync();
            return lista;
        }
        public async Task<bool> AtualizarAsync(int id, Chamado dadosAtualizados)
        {
            var find = await _contexto.Chamados.FindAsync(id);
            if (find == null)
            {
                return false;
            }

            find.Status = dadosAtualizados.Status;
            find.Prioridade = dadosAtualizados.Prioridade;
            await _contexto.SaveChangesAsync();
            return true;


        }

        public async Task<bool> ExcluirAsync(int id)
        {

            try
            {
                var Encontrar = await _contexto.Chamados.FindAsync(id);
                if (Encontrar == null)
                {
                    return false;
                }

                _contexto.Chamados.Remove(Encontrar);
                await _contexto.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetBaseException().Message);
                throw;
            }
        }

        public async Task<List<Chamado>> BuscarPorStatusAsync(string status)
        {
            var BuscaStatus = await _contexto.Chamados
                .Where(s => s.Status == status)
                .ToListAsync();

            return BuscaStatus;


        }

        public async Task<List<Chamado>> BuscarPorPrioridadeAsync(string prioridade)
        {

            var BuscaPrioridade = await _contexto.Chamados
                   .Where(p => p.Prioridade == prioridade)
                   .ToListAsync();

            return BuscaPrioridade;


        }

    
    }
}
