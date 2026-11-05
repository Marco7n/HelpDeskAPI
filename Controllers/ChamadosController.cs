using Microsoft.AspNetCore.Mvc;
using HelpDeskAPI.DAOs;
namespace HelpDeskAPI.Controllers
using HelpDeskAPI.Models;


{
    [ApiController]
    [Route("[controller]")]
    public class ChamadosController : ControllerBase
    {
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

            await _chamadoDao.AdicionarAsync(chamado);

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

            var excluir = await _chamadoDao.ExcluirAsync(id);
            if (!excluir)
            {
                return NotFound();

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
