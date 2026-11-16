using HelpDeskAPI.DAOs;
using HelpDeskAPI.DTOs;
using HelpDeskAPI.Models;
using HelpDeskAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HelpDeskAPI.Controllers
{
    // Expõe operações de cadastro e administração de usuários.
    [ApiController]
    // Mantém a rota legada com o nome explícito do controller.
    [Route("api/UsuariosController")]
    public class UsuariosController : ControllerBase
    {
        // DAO usado para consultar e persistir usuários.
        private readonly UsuarioDao _usuarioDao;
        // Serviço usado para gerar o hash da senha no cadastro.
        private readonly AuthService _authService;

        // O container injeta as dependências quando cria o controller.
        public UsuariosController(UsuarioDao usuarioDao, AuthService authService)
        {
            _usuarioDao = usuarioDao;
            _authService = authService;
        }

        // Converte a entidade para um DTO que não possui SenhaHash.
        private static UsuarioRespostaDto Resposta(Usuario usuario) => new()
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Perfil = usuario.Perfil
        };

        // Lista usuários e retorna apenas campos públicos.
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var lista = await _usuarioDao.ListaTodosAsync();
            return Ok(lista.Select(Resposta));
        }

        // Busca um usuário individual pelo identificador.
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

        // Cria um usuário Cliente a partir do DTO validado.
        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] UsuarioCadastroDto dados)
        {
            // A senha é transformada em hash antes de chegar ao banco.
            var usuario = new Usuario
            {
                Nome = dados.Nome,
                Email = dados.Email,
                SenhaHash = _authService.GerarHash(dados.Senha),
                Perfil = "Cliente"
            };

            // O banco gera o ID e pode rejeitar e-mail duplicado.
            await _usuarioDao.AdicionarAsync(usuario);
            return CreatedAtAction(nameof(Buscar), new { id = usuario.Id }, Resposta(usuario));
        }

        // Atualiza o próprio usuário ou qualquer usuário quando o solicitante é Atendente.
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Atualizar(int id, [FromBody] UsuarioAtuliazacaoDtos dados)
        {
            // Obtém o ID do usuário autenticado a partir da claim do JWT.
            var meuId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Atendente") && meuId != id.ToString())
            {
                return Forbid();
            }

            // Monta somente os dados permitidos nesta atualização.
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

        // Exclui o próprio usuário ou um usuário escolhido por um Atendente.
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

        // Pesquisa usuários pelo nome exato.
        [HttpGet("nome/{nome}")]
        public async Task<IActionResult> BuscarNomeAsync(string nome)
        {
            var usuarios = await _usuarioDao.BuscarNomeAsync(nome);
            return Ok(usuarios.Select(Resposta));
        }

        // Pesquisa usuários pelo perfil, recurso restrito a Atendentes.
        [HttpGet("perfil/{perfil}")]
        [Authorize(Roles = "Atendente")]
        public async Task<IActionResult> BuscarPorPerfilAsync(string perfil)
        {
            var usuarios = await _usuarioDao.BuscarPorStatusAsync(perfil);
            return Ok(usuarios.Select(Resposta));
        }
    }
}
