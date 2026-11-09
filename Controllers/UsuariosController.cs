using HelpDeskAPI.DAOs;
using HelpDeskAPI.DTOs;
using HelpDeskAPI.Models;
using HelpDeskAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HelpDeskAPI.Controllers
{
    [ApiController]
    [Route("api/UsuariosController")]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuarioDao _usuarioDao;
        private readonly AuthService _authService;

        public UsuariosController(UsuarioDao usuarioDao, AuthService authService)
        {
            _usuarioDao = usuarioDao;
            _authService = authService;
        }

        private static UsuarioRespostaDto Resposta(Usuario usuario) => new()
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Perfil = usuario.Perfil
        };

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var lista = await _usuarioDao.ListaTodosAsync();
            return Ok(lista.Select(Resposta));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Buscar(int id)
        {
            var usuario = await _usuarioDao.BuscarPorIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            return Ok(Resposta(usuario));
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] UsuarioCadastroDto dados)
        {
            try
            {
                var usuario = new Usuario
                {
                    Nome = dados.Nome,
                    Email = dados.Email,
                    SenhaHash = _authService.GerarHash(dados.Senha),
                    Perfil = "Cliente"
                };

                await _usuarioDao.AdicionarAsync(usuario);
                return CreatedAtAction(nameof(Buscar), new { id = usuario.Id }, Resposta(usuario));
            }
            catch (Exception)
            {
                return BadRequest("Nao foi possivel cadastrar os dados");
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Atualizar(int id, [FromBody] UsuarioAtuliazacaoDtos dados)
        {
            var meuId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Atendente") && meuId != id.ToString())
            {
                return Forbid();
            }

            var usuario = new Usuario
            {
                Nome = dados.Nome,
                Email = dados.Email
            };

            if (!await _usuarioDao.AtualizarAsync(id, usuario))
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Excluir(int id)
        {
            var meuId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Atendente") && meuId != id.ToString())
            {
                return Forbid();
            }

            if (!await _usuarioDao.ExcluirAsync(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("prioridade/{prioridade}")]
        public async Task<IActionResult> BuscarNomeAsync(string prioridade)
        {
            var usuarios = await _usuarioDao.BuscarNomeAsync(prioridade);
            return Ok(usuarios.Select(Resposta));
        }
    }
}
