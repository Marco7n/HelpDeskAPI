using HelpDeskAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace HelpDeskAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _servicoAutenticacao;

        public AuthController(AuthService servicoAutenticacao)
        {
            _servicoAutenticacao = servicoAutenticacao;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest requisicao)
        {
            var token = await _servicoAutenticacao.LoginAsync(
                requisicao.Email,
                requisicao.Senha);

            if (token == null)
            {
                return Unauthorized("E-mail ou senha inválidos.");
            }

            return Ok(new { token });
        }
    }

    public record LoginRequest(string Email, string Senha);
}
