using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;

namespace PromoCodeFactory.DataAccess.Data
{
    public class DataContext : DbContext
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Preference> Preferences { get; set; }
        public DbSet<PromoCode> PromoCodes { get; set; }
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .Property(e => e.Id)
                .HasConversion(
                    v => v.ToString(),
                    v => new Guid(v))
                .HasColumnType("TEXT");

            modelBuilder.Entity<Preference>()
                .Property(e => e.Id)
                .HasConversion(
                    v => v.ToString(),
                    v => new Guid(v))
                .HasColumnType("TEXT");

            modelBuilder.Entity<PromoCode>()
                .Property(e => e.Id)
                .HasConversion(
                    v => v.ToString(),
                    v => new Guid(v))
                .HasColumnType("TEXT");

            modelBuilder.Entity<PromoCode>()
                .HasOne(p => p.Preference)
                .WithMany();

            modelBuilder.Entity<PromoCode>()
                .HasOne(p => p.PartnerManager)
                .WithMany();

            modelBuilder.Entity<PromoCode>()
                .HasOne(c => c.Customer)
                .WithMany(c => c.PromoCodes);

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Role)
                .WithMany();

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Preferences)
                .WithMany(p => p.Customers)
                .UsingEntity("CustomerPreference");

            base.OnModelCreating(modelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
