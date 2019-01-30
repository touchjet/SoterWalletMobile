using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SoterWalletMobile.Models;
using Xamarin.Forms;

namespace SoterWalletMobile.Data
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Coin> Coins { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<WalletDevice> WalletDevices { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Coin>().HasOne(c => c.WalletDevice).WithMany(d => d.Coins).HasForeignKey(c => c.WalletDeviceId);
            modelBuilder.Entity<Address>().HasOne(a => a.Coin).WithMany(c => c.Addresses).HasForeignKey(a => a.CoinId);
            modelBuilder.Entity<Transaction>().HasOne(t => t.Address).WithMany(a => a.Transactions).HasForeignKey(t => t.AddressId);

            modelBuilder.Entity<WalletDevice>().HasIndex(d => d.Name).IsUnique();
            modelBuilder.Entity<Coin>().HasIndex(c => c.CoinName).IsUnique();
            modelBuilder.Entity<Coin>().HasIndex(c => c.CoinShortcut).IsUnique();

            modelBuilder.Entity<Transaction>().HasIndex(t => t.Hash).IsUnique();
        }

        private const string databaseName = "soterwallet.db";
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string databasePath = "";
            try
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.iOS:
                        databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "..", "Library", databaseName);
                        break;
                    case Device.Android:
                        databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), databaseName);
                        break;
                    default:
                        databasePath = Path.Combine(Directory.GetCurrentDirectory(), databaseName);
                        break;
                }
            }
            catch
            {
                Log.Error("Fall back to default path: %s", databasePath);
                databasePath = Path.Combine(Directory.GetCurrentDirectory(), databaseName);
            }
            // Specify that we will use sqlite and the path of the database here
            optionsBuilder.UseSqlite($"Filename={databasePath}");
        }
    }
}
