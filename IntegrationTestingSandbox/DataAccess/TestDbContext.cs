using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTestingSandbox.DataAccess
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
        }

        //Entities
        public DbSet<Strings> Strings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Strings>().ToTable("strings").HasNoKey();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(
                "Host=localhost; Port=5432; Database=postgres; Username=postgres; Password=mysecretpassword");
    }

    //Entity
    public class Strings
    {
        [Column("string")] public string String { get; set; }
    }
}