using System.Security.Cryptography;
using System.Text;
using HelpDeskAPI.DAOs;
using HelpDeskAPI.Data;
using HelpDeskAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("HelpDesk")
    ?? throw new InvalidOperationException("Configure ConnectionStrings:HelpDesk para o MySQL.");
builder.Services.AddDbContext<HelpDeskContext>(options => options.UseMySQL(connectionString));
builder.Services.AddScoped<ChamadoDao>();
builder.Services.AddScoped<InteracaoDao>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UsuarioDao>();

var chaveJwt = builder.Configuration["Jwt:Key"];
if (builder.Environment.IsDevelopment() && string.IsNullOrWhiteSpace(chaveJwt))
{
    chaveJwt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
    builder.Configuration["Jwt:Key"] = chaveJwt;
}
if (string.IsNullOrWhiteSpace(chaveJwt))
{
    throw new InvalidOperationException("Configure Jwt:Key.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opcoes =>
    {
        opcoes.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveJwt)),
            ValidateLifetime = true,
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var contexto = scope.ServiceProvider.GetRequiredService<HelpDeskContext>();
    await contexto.Database.OpenConnectionAsync();
    await contexto.Database.CloseConnectionAsync();
}


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
