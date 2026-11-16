// Importa tipos para codificar a chave JWT em bytes.
using System.Text;
// Importa as classes de acesso a dados registradas na injeção de dependência.
using HelpDeskAPI.DAOs;
// Importa o DbContext da aplicação.
using HelpDeskAPI.Data;
// Importa o serviço responsável pela autenticação.
using HelpDeskAPI.Services;
// Importa o esquema de autenticação Bearer usado pelos tokens JWT.
using Microsoft.AspNetCore.Authentication.JwtBearer;
// Permite ler a exceção capturada pelo middleware global.
using Microsoft.AspNetCore.Diagnostics;
// Fornece ProblemDetails para respostas padronizadas de erro.
using Microsoft.AspNetCore.Mvc;
// Fornece os tipos específicos de erro do SQLite.
using Microsoft.Data.Sqlite;
// Fornece o Entity Framework Core e o DbUpdateException.
using Microsoft.EntityFrameworkCore;
// Fornece os tipos necessários para validar e assinar JWTs.
using Microsoft.IdentityModel.Tokens;

// Cria o construtor padrão da aplicação ASP.NET Core.
var builder = WebApplication.CreateBuilder(args);

// Lê a conexão do banco; a aplicação não inicia se ela não estiver configurada.
var connectionString = builder.Configuration.GetConnectionString("HelpDesk")
    ?? throw new InvalidOperationException("Configure ConnectionStrings:HelpDesk.");
// Registra o contexto EF Core usando SQLite e a conexão configurada.
builder.Services.AddDbContext<HelpDeskContext>(options => options.UseSqlite(connectionString));
// Registra os DAOs como serviços por requisição HTTP.
builder.Services.AddScoped<ChamadoDao>();
builder.Services.AddScoped<InteracaoDao>();
// Registra o serviço de autenticação como serviço por requisição.
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UsuarioDao>();

// Obtém a chave usada para validar e emitir tokens JWT.
var chaveJwt = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Configure Jwt:Key.");

// Configura autenticação baseada no esquema Bearer.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opcoes =>
    {
        // Define como o ASP.NET Core deve validar cada token recebido.
        opcoes.TokenValidationParameters = new TokenValidationParameters
        {
            // Exige que a assinatura do token seja válida.
            ValidateIssuerSigningKey = true,
            // Usa a chave simétrica configurada para validar a assinatura.
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveJwt)),
            // Rejeita tokens expirados.
            ValidateLifetime = true,
            // Emite/aceita tokens sem exigir issuer específico neste projeto.
            ValidateIssuer = false,
            // Emite/aceita tokens sem exigir audience específico neste projeto.
            ValidateAudience = false
        };
    });

// Habilita descoberta e execução dos controllers MVC/API.
builder.Services.AddControllers();

// Habilita a geração do documento OpenAPI em desenvolvimento.
builder.Services.AddOpenApi();

// Constrói a aplicação com os serviços configurados.
var app = builder.Build();

// Cria um escopo temporário para obter o contexto durante a inicialização.
await using (var scope = app.Services.CreateAsyncScope())
{
    // Obtém o contexto registrado e aplica migrations pendentes no banco.
    var contexto = scope.ServiceProvider.GetRequiredService<HelpDeskContext>();
    await contexto.Database.MigrateAsync();
}


// Expõe o documento OpenAPI somente no ambiente de desenvolvimento.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Redireciona chamadas HTTP para HTTPS quando houver configuração correspondente.
app.UseHttpsRedirection();

// Converte exceções não tratadas em respostas HTTP padronizadas.
app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
{
    // Recupera a exceção gerada pelo endpoint ou por um serviço chamado por ele.
    var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
    // Por segurança, erros desconhecidos começam como HTTP 500.
    var statusCode = StatusCodes.Status500InternalServerError;
    var detail = "Ocorreu um erro interno ao processar a solicitação.";

    // Trata conflitos gerados pelo Entity Framework ao salvar no SQLite.
    if (exception is DbUpdateException { InnerException: SqliteException sqliteException })
    {
        // 2067 é violação de índice único; 787 é violação de chave estrangeira no SQLite.
        statusCode = sqliteException.SqliteExtendedErrorCode switch
        {
            2067 => StatusCodes.Status409Conflict,
            787 => StatusCodes.Status409Conflict,
            _ => statusCode
        };

        if (statusCode == StatusCodes.Status409Conflict)
        {
            // Retorna mensagens controladas sem expor SQL ou detalhes internos.
            detail = sqliteException.SqliteExtendedErrorCode == 2067
                ? "Já existe um registro com os dados informados."
                : "A operação entra em conflito com registros relacionados.";
        }
    }

    // Define o status e o formato da resposta de erro.
    context.Response.StatusCode = statusCode;
    context.Response.ContentType = "application/problem+json";
    // Serializa o erro no formato ProblemDetails usado pela API.
    await context.Response.WriteAsJsonAsync(new ProblemDetails
    {
        Status = statusCode,
        Title = statusCode == StatusCodes.Status409Conflict
            ? "Conflito de dados"
            : "Erro interno",
        Detail = detail
    });
}));

// Executa a autenticação e preenche User com as claims do token.
app.UseAuthentication();

// Executa as regras [Authorize] e [Authorize(Roles = ...)].
app.UseAuthorization();

// Mapeia as rotas declaradas nos controllers.
app.MapControllers();

// Inicia o servidor web e mantém a aplicação em execução.
app.Run();
