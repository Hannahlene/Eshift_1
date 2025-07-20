using EShift.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EShift.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Assistant> Assistants { get; set; }
        public DbSet<Container> Containers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Lorry> Lorries { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Load> Loads { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<LoadProduct> LoadProducts { get; set; }
        public DbSet<TransportUnit> TransportUnits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Relationship Configuration ---

            // Customer (1) → Jobs (Many)
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Jobs)
                .WithOne(j => j.Customer)
                .HasForeignKey(j => j.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Customer (1) → Products (Many)
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Products)
                .WithOne(p => p.Customer)
                .HasForeignKey(p => p.CustomerId)
                .OnDelete(DeleteBehavior.NoAction); // Prevent cascade loop

            // Job (1) → Loads (Many)
            modelBuilder.Entity<Job>()
                .HasMany(j => j.Loads)
                .WithOne(l => l.Job)
                .HasForeignKey(l => l.JobId)
                .OnDelete(DeleteBehavior.Cascade);

            // Load (1) → LoadProducts (Many)
            modelBuilder.Entity<Load>()
                .HasMany(l => l.LoadProducts)
                .WithOne(lp => lp.Load)
                .HasForeignKey(lp => lp.LoadId)
                .OnDelete(DeleteBehavior.Cascade);

            // Product (1) → LoadProducts (Many)
            modelBuilder.Entity<Product>()
                .HasMany(p => p.LoadProducts)
                .WithOne(lp => lp.Product)
                .HasForeignKey(lp => lp.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent accidental deletion

            // TransportUnit (1) → Loads (Many)
            modelBuilder.Entity<TransportUnit>()
                .HasMany(tu => tu.Loads)
                .WithOne(l => l.TransportUnit)
                .HasForeignKey(l => l.TransportUnitId)
                .OnDelete(DeleteBehavior.Restrict);

            // Lorry (1) → TransportUnits (Many)
            modelBuilder.Entity<Lorry>()
                .HasMany(l => l.TransportUnits)
                .WithOne(tu => tu.Lorry)
                .HasForeignKey(tu => tu.LorryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Driver (1) → TransportUnits (Many)
            modelBuilder.Entity<Driver>()
                .HasMany(d => d.TransportUnits)
                .WithOne(tu => tu.Driver)
                .HasForeignKey(tu => tu.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            // Assistant (1) → TransportUnits (Many) (nullable FK)
            modelBuilder.Entity<Assistant>()
                .HasMany(a => a.TransportUnits)
                .WithOne(tu => tu.Assistant)
                .HasForeignKey(tu => tu.AssistantId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // --- Precision Fix for Decimal Warnings ---
            modelBuilder.Entity<Product>()
                .Property(p => p.WeightKg)
                .HasPrecision(10, 2); // Adjust as needed

            modelBuilder.Entity<Load>()
                .Property(l => l.WeightKg)
                .HasPrecision(10, 2);
        }
    }
}
