using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using ProjectBackend.api.Models.Domain;
using System.IO;

namespace ProjectBackend.api.Data
{

    public class ProjectDbContext : DbContext
    {
        public ProjectDbContext() { }

        public ProjectDbContext(DbContextOptions<ProjectDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=localhost;Database=ProjectDatabase;Trusted_Connection=True;TrustServerCertificate=True");
            }
        }

        public DbSet<ProductsDomain> Products { get; set; }
    }
}