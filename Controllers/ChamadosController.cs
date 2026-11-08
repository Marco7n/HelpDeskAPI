using Microsoft.AspNetCore.Mvc;
using HelpDeskAPI.DAOs;
using HelpDeskAPI.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskAPI.Controllers
{
    [ApiController]
    [Route("api/chamados")]
    public class ChamadosController : ControllerBase
    {
        private const int CodigoRestricaoChaveEstrangeira = 787;

        private readonly ChamadoDao _chamadoDao;
        public ChamadosController(ChamadoDao chamadoDao)
        {
            _chamadoDao = chamadoDao;
        }
        [HttpGet]

        public async Task<IActionResult> Listar()
        {

            var lista = await _chamadoDao.ListaTodosAsync();
            return Ok(lista);


        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Buscar(int id)
        {
            var busca = await _chamadoDao.BuscarPorIdAsync(id);
            if (busca == null)
            {
                return NotFound();
            }
            return Ok(busca);
        }
        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] Chamado chamado)
        {

            try
            {
                await _chamadoDao.AdicionarAsync(chamado);
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqliteException
                { SqliteExtendedErrorCode: CodigoRestricaoChaveEstrangeira })
            {
                return BadRequest(new ProblemDetails { Detail = "UsuarioId não referencia um usuário existente." });
            }

            return CreatedAtAction(
            nameof(Buscar),
            new { id = chamado.Id },
            chamado
        );

        }
        [HttpPut("{id}")]

        public async Task<IActionResult> Atualizar(int id, [FromBody] Chamado dadosAtualizados)
        {

            bool atualizado = await _chamadoDao.AtualizarAsync(id, dadosAtualizados);
            if (!atualizado)
            {

                return NotFound();

            }
            return NoContent();


        }
        [HttpDelete("{id}")]

        public async Task<IActionResult> Excluir(int id)
        {

            try
            {
                if (!await _chamadoDao.ExcluirAsync(id))
                {
                    return NotFound();
                }
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqliteException
                { SqliteExtendedErrorCode: CodigoRestricaoChaveEstrangeira })
            {
                return Conflict(new ProblemDetails { Detail = "O chamado possui interações vinculadas." });
            }
            return NoContent();


        }
        [HttpGet("status/{status}")]

        public async Task<IActionResult> BuscarPorStatusAsync(string status)
        {

            var buscarStatus = await _chamadoDao.BuscarPorStatusAsync(status);

            return Ok(buscarStatus);



        }
        [HttpGet("prioridade/{prioridade}")]

        public async Task<IActionResult> BuscarPorPrioridadeAsync(string prioridade)
        {

            var buscarPrioridade = await _chamadoDao.BuscarPorPrioridadeAsync(prioridade);

            return Ok(buscarPrioridade);
        }
    }
}
