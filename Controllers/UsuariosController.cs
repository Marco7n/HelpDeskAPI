using HelpDeskAPI.DAOs;
using HelpDeskAPI.DTOs;
using HelpDeskAPI.Models;
using Microsoft.AspNetCore.Mvc;
using HelpDeskAPI.Models;
using HelpDeskAPI.Services;

namespace HelpDeskAPI.Controllers
{
    
        [ApiController]
        [Route("api/UsuariosController")]
        public class UsuariosController : ControllerBase
        {
            private readonly UsuarioDao _usuarioDao;
            private readonly AuthService _authService;

            public UsuariosController(UsuarioDao _usuarioDao, AuthService authService)
            {
                this._usuarioDao = _usuarioDao;
                this._authService = authService;
            }
            
            
            
            publid 

            [HttpGet]
            public async Task<IActionResult> Listar()
            {
                var lista = await _usuarioDao.ListaTodosAsync();

                var resposta = lista.Select(usuario => new UsuarioRespostaDto
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Perfil = usuario.Perfil
                });
                
                
                return Ok(resposta);


            }

            [HttpGet("{id}")]
            public async Task<IActionResult> Buscar(int id)
            {
                var busca = await _usuarioDao.BuscarPorIdAsync(id);
                if (busca == null)
                {
                    return NotFound();
                }

                return Ok(busca);
            }

            [HttpPost]
            public async Task<IActionResult> Cadastrar([FromBody]  UsuarioCadastroDto dados)
            {

                try
                {
                    var usuario = new Usuario
                    {
                        Nome = dados.Nome,
                        Email = dados.Email,
                        SenhaHash = _authService.GerarHash(dados.Senha),
                        Perfil = "usuario"
                    };

                    await _usuarioDao.AdicionarAsync(usuario);
               


                var respostaDtos = new UsuarioRespostaDto
                {
                    Id = usuario.Id,
                    Nome = usuario.Nome,
                    Email = usuario.Email,
                    Perfil = usuario.Perfil
                };

                

                return CreatedAtAction(
                    nameof(Buscar),
                    new { id = usuario.Id },
                    respostaDtos
                );
                
                }
                catch (Exception e)
                {
                    return BadRequest("Nao foi possivel cadastrar os dados");
                }

            }

            [HttpPut("{id}")]

            public async Task<IActionResult> Atualizar(int id, [FromBody] UsuarioAtuliazacaoDtos dados)
            {
                var usuario = new Usuario
                {
                    Nome = dados.Nome,
                    Email = dados.Email,
                };
                
                bool atualizado = await _usuarioDao.AtualizarAsync(id, usuario);
                
                if (!atualizado)
                {
                    return BadRequest("Nao foi possivel atualizar o usuario");
                }

                return NoContent();

            }

            [HttpDelete("{id}")]

            public async Task<IActionResult> Excluir(int id)
            {

                var excluir = await _usuarioDao.ExcluirAsync(id);
                if (!excluir)
                {
                    return NotFound();

                }

                return NoContent();


            }
            
            [HttpGet("prioridade/{prioridade}")]

            public async Task<IActionResult> BuscarNomeAsync(string prioridade)
            {

                var buscarPrioridade = await _usuarioDao.BuscarNomeAsync(prioridade);

                if (buscarPrioridade == null)
                {
                    return NotFound();
                }

                return Ok(buscarPrioridade);
            }

        }
    }


