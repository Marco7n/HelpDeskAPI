using HelpDeskAPI.DAOs;
using HelpDeskAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("HelpDesk")
    ?? throw new InvalidOperationException("Configure ConnectionStrings:HelpDesk.");
builder.Services.AddDbContext<HelpDeskContext>(options => options.UseSqlite(connectionString));
builder.Services.AddScoped<ChamadoDao>();
builder.Services.AddScoped<InteracaoDao>();
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var contexto = scope.ServiceProvider.GetRequiredService<HelpDeskContext>();
    await contexto.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
