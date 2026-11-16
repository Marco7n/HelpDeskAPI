using Microsoft.AspNetCore.Mvc;
using HelpDeskAPI.DAOs;
using HelpDeskAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace HelpDeskAPI.Controllers
{
    // Controller de mensagens associadas aos chamados.
    [ApiController]
    [Route("api/interacoes")]
    [Authorize(Roles = "Cliente,Atendente")]

    public class InteracoesController : ControllerBase
    {
        // DAO das interações.
        private readonly InteracaoDao _interacaoDao;
        // DAO usado para validar o acesso ao chamado relacionado.
        private readonly ChamadoDao _chamadoDao;
        // Recupera o ID do usuário autenticado; retorna null para token sem claim válida.
        private int? UsuarioId => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

        // Injeta os DAOs necessários aos endpoints.
        public InteracoesController(InteracaoDao interacaoDao, ChamadoDao chamadoDao)
        {

            _interacaoDao = interacaoDao;
            _chamadoDao = chamadoDao;

        }
        // Atendentes acessam qualquer chamado; Clientes somente os próprios.
        private async Task<bool> PodeAcessarChamado(int chamadoId, int usuarioId)
        {
            var chamado = await _chamadoDao.BuscarPorIdAsync(chamadoId);
            return chamado != null && (User.IsInRole("Atendente") || chamado.UsuarioId == usuarioId);
        }
        // Lista tudo para Atendente e somente interações dos chamados do Cliente.
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
        // Busca uma interação depois de validar o acesso ao chamado pai.
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

        // Cria interação para o chamado acessível ao usuário autenticado.
        [HttpPost]
        [Authorize]

        public async Task<IActionResult> Cadastrar([FromBody] Interacao? interacao)
        {
            // Rejeita corpo ausente ou mensagem sem conteúdo.
            if (interacao is null || string.IsNullOrWhiteSpace(interacao.Mensagem))
            {
                return BadRequest("A mensagem é obrigatória.");
            }

            if (UsuarioId is not int usuarioId) return Forbid();
            if (!await PodeAcessarChamado(interacao.ChamadoId, usuarioId)) return NotFound();
            // O autor vem do token, não do corpo recebido.
            interacao.UsuarioId = usuarioId;
            await _interacaoDao.AdicionarAsync(interacao);

            return CreatedAtAction(
             nameof(Buscar),
            new { id = interacao.Id },
            interacao
       );
        }

        // Permite que Atendentes corrijam a mensagem de qualquer interação acessível.
        [HttpPut("{id}")]
        [Authorize(Roles = "Atendente")]

        public async Task<IActionResult> Atualizar(int id, [FromBody] Interacao? dadosAtualizados)
        {
            if (dadosAtualizados is null || string.IsNullOrWhiteSpace(dadosAtualizados.Mensagem))
            {
                return BadRequest("A mensagem é obrigatória.");
            }

            if (UsuarioId is not int usuarioId) return Forbid();
            var interacao = await _interacaoDao.BuscarPorIdAsync(id);
            if (interacao == null || !await PodeAcessarChamado(interacao.ChamadoId, usuarioId)) return NotFound();
            var atualizar = await _interacaoDao.AtualizarAsync(id, dadosAtualizados);
            if (!atualizar)
            {
                return NotFound();
            }
            return NoContent();

        }

        // Cliente só remove sua própria interação; Atendente também passa pela regra atual.
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

        // Lista as interações de um chamado autorizado.
        [HttpGet("chamado/{chamadoId}")]

        public async Task<IActionResult> BuscarPorChamado(int chamadoId)
        {
            if (UsuarioId is not int usuarioId) return Forbid();
            if (!await PodeAcessarChamado(chamadoId, usuarioId)) return NotFound();
            var BuscarChamado = await _interacaoDao.BuscarPorChamadoAsync(chamadoId);
            return Ok(BuscarChamado);

        }
        // Lista interações de um usuário, limitando Clientes aos seus próprios chamados.
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
