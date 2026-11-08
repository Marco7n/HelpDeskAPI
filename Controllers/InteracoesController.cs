using Microsoft.AspNetCore.Mvc;
using HelpDeskAPI.DAOs;
using HelpDeskAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace HelpDeskAPI.Controllers
{
    [ApiController]
    [Route("api/interacoes")]
    [Authorize(Roles = "Cliente,Atendente")]

    public class InteracoesController : ControllerBase
    {
        private readonly InteracaoDao _interacaoDao;
        private readonly ChamadoDao _chamadoDao;
        private int? UsuarioId => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

        public InteracoesController(InteracaoDao interacaoDao, ChamadoDao chamadoDao)
        {

            _interacaoDao = interacaoDao;
            _chamadoDao = chamadoDao;

        }
        private async Task<bool> PodeAcessarChamado(int chamadoId, int usuarioId)
        {
            var chamado = await _chamadoDao.BuscarPorIdAsync(chamadoId);
            return chamado != null && (User.IsInRole("Atendente") || chamado.UsuarioId == usuarioId);
        }
        [HttpGet]

        public async Task<IActionResult> Listar()
        {
            if (UsuarioId is not int usuarioId) return Forbid();
            var lista = await _interacaoDao.ListaTodosAsync();
            if (User.IsInRole("Atendente")) return Ok(lista);
            var chamados = await _chamadoDao.ListaTodosAsync();
            var meusChamados = chamados.Where(chamado => chamado.UsuarioId == usuarioId)
                .Select(chamado => chamado.Id).ToHashSet();
            return Ok(lista.Where(interacao => meusChamados.Contains(interacao.ChamadoId)));

        }
        [HttpGet("{id}")]

        public async Task<IActionResult> Buscar(int id)
        {
            if (UsuarioId is not int usuarioId) return Forbid();
            var buscar = await _interacaoDao.BuscarPorIdAsync(id);
            if (buscar == null || !await PodeAcessarChamado(buscar.ChamadoId, usuarioId))
            {
                return NotFound();
            }
            return Ok(buscar);

        }

        [HttpPost]

        public async Task<IActionResult> Cadastrar([FromBody] Interacao interacao)
        {
            if (UsuarioId is not int usuarioId) return Forbid();
            if (!await PodeAcessarChamado(interacao.ChamadoId, usuarioId)) return NotFound();
            interacao.UsuarioId = usuarioId;
            await _interacaoDao.AdicionarAsync(interacao);

            return CreatedAtAction(
             nameof(Buscar),
            new { id = interacao.Id },
            interacao
       );
        }

        [HttpPut("{id}")]

        public async Task<IActionResult> Atualizar(int id, [FromBody] Interacao dadosAtualizados)
        {
            if (UsuarioId is not int usuarioId) return Forbid();
            var interacao = await _interacaoDao.BuscarPorIdAsync(id);
            if (interacao == null || !await PodeAcessarChamado(interacao.ChamadoId, usuarioId)) return NotFound();
            if (interacao.UsuarioId != usuarioId) return Forbid();
            var atualizar = await _interacaoDao.AtualizarAsync(id, dadosAtualizados);
            if (!atualizar)
            {
                return NotFound();
            }
            return NoContent();

        }

        [HttpDelete("{id}")]

        public async Task<IActionResult> Deletar(int id)
        {
            if (UsuarioId is not int usuarioId) return Forbid();
            var interacao = await _interacaoDao.BuscarPorIdAsync(id);
            if (interacao == null || !await PodeAcessarChamado(interacao.ChamadoId, usuarioId)) return NotFound();
            if (interacao.UsuarioId != usuarioId) return Forbid();
            var deletar = await _interacaoDao.ExcluirAsync(id);
            if (!deletar)
            {
                return NotFound();
            }
            return NoContent();

        }

        [HttpGet("chamado/{chamadoId}")]

        public async Task<IActionResult> BuscarPorChamado(int chamadoId)
        {
            if (UsuarioId is not int usuarioId) return Forbid();
            if (!await PodeAcessarChamado(chamadoId, usuarioId)) return NotFound();
            var BuscarChamado = await _interacaoDao.BuscarPorChamadoAsync(chamadoId);
            return Ok(BuscarChamado);

        }
        [HttpGet("usuario/{usuarioId}")]

        public async Task<IActionResult> BuscarPorUsuario(int usuarioId)
        {
            if (UsuarioId is not int meuId) return Forbid();
            var buscarUsuario = await _interacaoDao.BuscarPorUsuarioAsync(usuarioId);
            if (User.IsInRole("Atendente")) return Ok(buscarUsuario);
            var chamados = await _chamadoDao.ListaTodosAsync();
            var meusChamados = chamados.Where(chamado => chamado.UsuarioId == meuId)
                .Select(chamado => chamado.Id).ToHashSet();
            return Ok(buscarUsuario.Where(interacao => meusChamados.Contains(interacao.ChamadoId)));


        }

    }
}
