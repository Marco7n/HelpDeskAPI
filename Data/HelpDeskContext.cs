using HelpDeskAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskAPI.Data
{
    public class HelpDeskContext : DbContext
    {
        public HelpDeskContext(DbContextOptions<HelpDeskContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Chamado> Chamados => Set<Chamado>();
        public DbSet<Interacao> Interacoes => Set<Interacao>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>().HasIndex(usuario => usuario.Email).IsUnique();

            var chamado = modelBuilder.Entity<Chamado>();
            chamado.Property(item => item.DataCriacao).HasDefaultValueSql("CURRENT_TIMESTAMP");
            chamado.HasOne(item => item.Usuario)
                .WithMany()
                .HasForeignKey(item => item.UsuarioId)
                .OnDelete(DeleteBehavior.NoAction);

            var interacao = modelBuilder.Entity<Interacao>();
            interacao.Property(item => item.DataCriacao).HasDefaultValueSql("CURRENT_TIMESTAMP");
            interacao.HasOne(item => item.Chamado)
                .WithMany(item => item.Interacoes)
                .HasForeignKey(item => item.ChamadoId)
                .OnDelete(DeleteBehavior.NoAction);
            interacao.HasOne(item => item.Usuario)
                .WithMany()
                .HasForeignKey(item => item.UsuarioId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
