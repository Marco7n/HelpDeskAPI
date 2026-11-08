using Microsoft.AspNetCore.Mvc;
using HelpDeskAPI.DAOs;
using HelpDeskAPI.Models;


namespace HelpDeskAPI.Controllers
{
    [ApiController]
    [Route("api/interacoes")]

    public class InteracoesController : ControllerBase
    {
        private readonly InteracaoDao _interacaoDao;

        public InteracoesController(InteracaoDao interacaoDao)
        {

            _interacaoDao = interacaoDao;

        }
        [HttpGet]

        public async Task<IActionResult> Listar()
        {

            var lista = await _interacaoDao.ListaTodosAsync();
            return Ok(lista);

        }
        [HttpGet("{id}")]

        public async Task<IActionResult> Buscar(int id)
        {

            var buscar = await _interacaoDao.BuscarPorIdAsync(id);
            if (buscar == null)
            {
                return NotFound();
            }
            return Ok(buscar);

        }

        [HttpPost]

        public async Task<IActionResult> Cadastrar([FromBody] Interacao interacao)
        {

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

            var BuscarChamado = await _interacaoDao.BuscarPorChamadoAsync(chamadoId);
            return Ok(BuscarChamado);

        }
        [HttpGet("usuario/{usuarioId}")]

        public async Task<IActionResult> BuscarPorUsuario(int usuarioId)
        {

            var buscarUsuario = await _interacaoDao.BuscarPorUsuarioAsync(usuarioId);
            return Ok(buscarUsuario);


        }

    }
}
