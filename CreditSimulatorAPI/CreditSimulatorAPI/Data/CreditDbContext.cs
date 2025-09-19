using CreditSimulatorAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CreditSimulatorAPI.Data
{
    public class CreditDbContext : DbContext
    {
        public CreditDbContext(DbContextOptions<CreditDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Loan> Loans { get; set; }
        public DbSet<Payment> Payments { get; set; }
    }
}
