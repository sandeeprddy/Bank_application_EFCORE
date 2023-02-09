using Microsoft.EntityFrameworkCore;
using Models;

namespace DataAcessLayer
{
    public class BankDBContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<Currency> Currencies { get; set; } 
        public DbSet<BankCurrency> BankCurrencies { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "Server=(localdb)\\MSSQLLocalDB;Database=Organisation;Trusted_Connection=True;";

            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.HasDefaultSchema("Bank");

            modelBuilder.Entity<Bank>().HasAlternateKey(bank => bank.Name);

            modelBuilder.Entity<Bank>().Property(bank => bank.Id).ValueGeneratedNever();
            modelBuilder.Entity<User>().Property(user => user.Id).ValueGeneratedNever();
            modelBuilder.Entity<Account>().Property(account => account.Id).ValueGeneratedNever();
            modelBuilder.Entity<Transaction>().Property(transaction => transaction.Id).ValueGeneratedNever();

        }
    }
}
