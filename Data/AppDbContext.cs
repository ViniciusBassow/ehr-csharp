using ehr_csharp.Models;
using Microsoft.EntityFrameworkCore;

namespace SQLApp.Data
{
    public class AppDbContext : DbContext
    {
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
        public DbSet<Paciente> Paciente { get; set; }
        public DbSet<User> User { get; set; }
    }
}
