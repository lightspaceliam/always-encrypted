using Microsoft.EntityFrameworkCore;

namespace AlwaysEncrypted.Entities
{
    public class AlwaysEncryptedDbContext : DbContext
    {
        public AlwaysEncryptedDbContext() { }
        public AlwaysEncryptedDbContext(DbContextOptions<AlwaysEncryptedDbContext> options) : base(options) { }

        public DbSet<Patient> Patients { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=localhost;Database=Clinic;Integrated Security=True;MultipleActiveResultSets=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>()
                .HasIndex(e => e.Ssn);
            modelBuilder.Entity<Patient>()
                .HasIndex(e => e.FirstName);
            modelBuilder.Entity<Patient>()
                .HasIndex(e => e.LastName);

            modelBuilder.Entity<Patient>()
                .HasIndex(e => e.Ssn)
                .IsUnique();
        }
    }
}
