using ehr_csharp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SQLApp.Data
{
    public class AppDbContext : IdentityDbContext<Usuario>

    {
        internal object Medicos;

        public IConfiguration _config { get; set; }
        public AppDbContext(IConfiguration config)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_config.GetConnectionString("DefaultConnection"));            
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    //modelBuilder.Entity<User>().Property(x => x.)

        //    modelBuilder.HasDefaultSchema("identity");
        //}
        public DbSet<Anexo> Anexo { get; set; }
        public DbSet<Antecedente> Antecedente { get; set; }
        public DbSet<Consulta> Consulta { get; set; }
        public DbSet<Diagnostico> Diagnostico { get; set; }
        public DbSet<Especialidade> Especialidade { get; set; }
        public DbSet<Exame> Exame { get; set; }
        public DbSet<Hemograma> Hemograma { get; set; }
        public DbSet<Medicamento> Medicamento { get; set; }
        public DbSet<Medico> Medico { get; set; }
        public DbSet<Paciente> Paciente { get; set; }
        public DbSet<Prescricao> Prescricao { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Log> Log { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurando o relacionamento entre Hemograma e Consulta
            modelBuilder.Entity<Hemograma>()
                .HasOne(h => h.Consulta)
                .WithMany() // Presume-se que Consulta não tem uma coleção de Hemogramas
                .HasForeignKey(h => h.IdConsulta)
                .OnDelete(DeleteBehavior.Restrict); // Define Restrict para evitar cascata
        }

    }
}
