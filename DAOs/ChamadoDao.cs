using HelpDeskAPI.Data;
using HelpDeskAPI.Models;
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskAPI.DAOs
{

    // Camada de persistência dos chamados, isolando EF Core dos controllers.
    public class ChamadoDao
    {

        // Contexto que representa o banco atual da requisição.
        private readonly HelpDeskContext _contexto;

        // Recebe o contexto configurado com SQLite.
        public ChamadoDao(HelpDeskContext contexto)
        {
            _contexto = contexto;

        }

        // Adiciona e persiste um novo chamado.
        public async Task AdicionarAsync(Chamado chamado)
        {

            await _contexto.Chamados.AddAsync(chamado);
            await _contexto.SaveChangesAsync();

        }

        // Busca um chamado por ID para leitura ou verificação de acesso.
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
    


    // Retorna todos os chamados para posterior filtragem por perfil no controller.
    public async Task<List<Chamado>> ListaTodosAsync() { 
            var lista = await _contexto.Chamados.ToListAsync();
            return lista;
        }
        // Atualiza o status e a prioridade do chamado existente.
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

        // Remove o chamado; falha se ainda existirem interações dependentes.
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

        // Consulta chamados pelo status informado.
        public async Task<List<Chamado>> BuscarPorStatusAsync(string status)
        {
            var BuscaStatus = await _contexto.Chamados
                .Where(s => s.Status == status)
                .ToListAsync();

            return BuscaStatus;


        }

        // Consulta chamados pela prioridade informada.
        public async Task<List<Chamado>> BuscarPorPrioridadeAsync(string prioridade)
        {

            var BuscaPrioridade = await _contexto.Chamados
                   .Where(p => p.Prioridade == prioridade)
                   .ToListAsync();

            return BuscaPrioridade;


        }

    
    }
}
