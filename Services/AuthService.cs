using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HelpDeskAPI.Data;
using HelpDeskAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace HelpDeskAPI.Services
{
    public class AuthService
    {
        private readonly HelpDeskContext _contexto;
        private readonly IConfiguration _configuracao;

        public AuthService(HelpDeskContext contexto, IConfiguration configuracao)
        {
            _contexto = contexto;
            _configuracao = configuracao;
        }

        public async Task<string?> LoginAsync(string email, string senha)
        {

            var usuario = await _contexto.Usuarios
                .FirstOrDefaultAsync(usuario => usuario.Email == email);

            if (usuario == null)
            {
                return null;
            }

            if (!BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash))
            {
                return null;
            }

            return GerarToken(usuario);
        }

        private string GerarToken(Usuario usuario)
        {

            var chave = _configuracao["Jwt:Key"]
                ?? throw new InvalidOperationException("A chave Jwt:Key não foi configurada.");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Perfil)
            };

            var chaveSegura = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chave));
            var credenciais = new SigningCredentials(chaveSegura, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credenciais);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
