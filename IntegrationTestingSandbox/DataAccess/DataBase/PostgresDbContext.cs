using Microsoft.EntityFrameworkCore;

namespace IntegrationTestingSandbox.DataAccess.DataBase
{
    public class PostgresDbContext : DbContext
    {
        public PostgresDbContext(DbContextOptions<PostgresDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        //Entities
        public DbSet<Strings> Strings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Strings>()
                .ToTable("strings")
                .HasKey(p => p.Id)
                .HasName("PK_Strings");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(
                "Host=localhost; Port=5432; Database=postgres; Username=postgres; Password=mysecretpassword");
    }
}