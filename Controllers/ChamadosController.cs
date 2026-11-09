using Microsoft.AspNetCore.Mvc;
using HelpDeskAPI.DAOs;
using HelpDeskAPI.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace HelpDeskAPI.Controllers
{
    [ApiController]
    [Route("api/chamados")]
    [Authorize(Roles = "Cliente,Atendente")]
    public class ChamadosController : ControllerBase
    {
        private const int CodigoRestricaoChaveEstrangeira = 787;

        private static bool StatusValido(string status) =>
            status is "Aberto" or "Em andamento" or "Concluido";

        private static bool PrioridadeValida(string prioridade) =>
            prioridade is "Baixa" or "Media" or "Alta";

        private readonly ChamadoDao _chamadoDao;
        private int? UsuarioId => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
        public ChamadosController(ChamadoDao chamadoDao)
        {
            _chamadoDao = chamadoDao;
        }
        [HttpGet]

        public async Task<IActionResult> Listar()
        {
            if (UsuarioId is not int usuarioId) return Forbid();
            var lista = await _chamadoDao.ListaTodosAsync();
            return Ok(User.IsInRole("Atendente") ? lista : lista.Where(chamado => chamado.UsuarioId == usuarioId));


        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Buscar(int id)
        {
            if (UsuarioId is not int usuarioId) return Forbid();
            var busca = await _chamadoDao.BuscarPorIdAsync(id);
            if (busca == null || (!User.IsInRole("Atendente") && busca.UsuarioId != usuarioId))
            {
                return NotFound();
            }
            return Ok(busca);
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Cadastrar([FromBody] Chamado chamado)
        {
            if (string.IsNullOrWhiteSpace(chamado.Titulo) ||
                string.IsNullOrWhiteSpace(chamado.Descricao))
            {
                return BadRequest("Título e descrição são obrigatórios.");
            }

            if (!StatusValido(chamado.Status) || !PrioridadeValida(chamado.Prioridade))
            {
                return BadRequest("Status ou prioridade inválidos.");
            }

            if (UsuarioId is not int usuarioId) return Forbid();
            chamado.UsuarioId = usuarioId;
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
        [Authorize(Roles = "Atendente")]

        public async Task<IActionResult> Atualizar(int id, [FromBody] Chamado dadosAtualizados)
        {
            if (!StatusValido(dadosAtualizados.Status) ||
                !PrioridadeValida(dadosAtualizados.Prioridade))
            {
                return BadRequest("Status ou prioridade inválidos.");
            }

            bool atualizado = await _chamadoDao.AtualizarAsync(id, dadosAtualizados);
            if (!atualizado)
            {

                return NotFound();

            }
            return NoContent();


        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Atendente")]

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
            if (UsuarioId is not int usuarioId) return Forbid();
            var buscarStatus = await _chamadoDao.BuscarPorStatusAsync(status);
            return Ok(User.IsInRole("Atendente") ? buscarStatus : buscarStatus.Where(chamado => chamado.UsuarioId == usuarioId));



        }
        [HttpGet("prioridade/{prioridade}")]

        public async Task<IActionResult> BuscarPorPrioridadeAsync(string prioridade)
        {
            if (UsuarioId is not int usuarioId) return Forbid();
            var buscarPrioridade = await _chamadoDao.BuscarPorPrioridadeAsync(prioridade);
            return Ok(User.IsInRole("Atendente") ? buscarPrioridade : buscarPrioridade.Where(chamado => chamado.UsuarioId == usuarioId));
        }
    }
}
