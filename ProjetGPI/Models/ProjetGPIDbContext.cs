using Microsoft.EntityFrameworkCore;

namespace ProjetGPI.Models
{
    public class ProjetGPIDbContext : DbContext
    {
        public ProjetGPIDbContext(DbContextOptions<ProjetGPIDbContext> options)
            : base(options) { }

        public DbSet<Etudiant> Etudiants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Etudiant>(entity =>
            {
                entity.Property(e => e.Nom).IsFixedLength();
                entity.Property(e => e.Prenom).IsFixedLength();
                entity.Property(e => e.Email).IsFixedLength();
                entity.Property(e => e.Sexe).IsFixedLength();
            });
        }
    }
}
