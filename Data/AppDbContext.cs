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
        public DbSet<Paciente> Paciente { get; set; }
    }
}
