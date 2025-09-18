using CreditSimulatorAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CreditSimulatorAPI.Data
{
    public class CreditDbContext : DbContext
    {
        public CreditDbContext(DbContextOptions<CreditDbContext> options) : base(options) { }

        public DbSet<Loan> Loans { get; set; }
        public DbSet<Payment> Payments { get; set; }
    }
}
