using System.Text;
using HelpDeskAPI.DAOs;
using HelpDeskAPI.Data;
using HelpDeskAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("HelpDesk")
    ?? throw new InvalidOperationException("Configure ConnectionStrings:HelpDesk.");
builder.Services.AddDbContext<HelpDeskContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped<ChamadoDao>();
builder.Services.AddScoped<InteracaoDao>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UsuarioDao>();

var chaveJwt = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Configure Jwt:Key.");

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
    await contexto.Database.MigrateAsync();
}


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
{
    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    var statusCode = StatusCodes.Status500InternalServerError;
    var detail = "Ocorreu um erro interno ao processar a solicitação.";

    if (exception is DbUpdateException { InnerException: SqliteException sqliteException })
    {
        statusCode = sqliteException.SqliteExtendedErrorCode switch
        {
            2067 => StatusCodes.Status409Conflict,
            787 => StatusCodes.Status409Conflict,
            _ => statusCode
        };

        if (statusCode == StatusCodes.Status409Conflict)
        {
            detail = sqliteException.SqliteExtendedErrorCode == 2067
                ? "Já existe um registro com os dados informados."
                : "A operação entra em conflito com registros relacionados.";
        }
    }

    context.Response.StatusCode = statusCode;
    context.Response.ContentType = "application/problem+json";
    await context.Response.WriteAsJsonAsync(new ProblemDetails
    {
        Status = statusCode,
        Title = statusCode == StatusCodes.Status409Conflict
            ? "Conflito de dados"
            : "Erro interno",
        Detail = detail
    });
}));

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
