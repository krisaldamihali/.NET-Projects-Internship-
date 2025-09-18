using GmailRegistrationDemo.Models;
using Microsoft.EntityFrameworkCore;
namespace GmailRegistrationDemo.Data
{
    public class GmailDBContext : DbContext
    {
        public GmailDBContext(DbContextOptions<GmailDBContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Gender to be stored as string
            modelBuilder.Entity<User>()
                .Property(u => u.Gender)
                .HasConversion<string>()
                .IsRequired();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<RegistrationSession> RegistrationSessions { get; set; }
    }
}