using ECommerceRex.Models;
using ECommerceRex.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ECommerceRex.Data;

public class ApplicationDbContext : DbContext
{
    private readonly IHmacService _hmacService;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHmacService hmacService)
        : base(options)
    {
        _hmacService = hmacService;
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<BankAccount> BankAccounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Attendance> Attendances { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Enforce decimal(18,2) for all monetary columns
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
                {
                    property.SetColumnType("decimal(18,2)");
                }
            }
        }

        // Unique constraints, indexes etc.
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Detect changes for tamper-evident ledger
        var entries = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            // Compute or verify hash
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.RowHash = ComputeHash(entry.Entity);
            }
            else if (entry.State == EntityState.Modified)
            {
                // Verify existing hash (tamper detection)
                var originalHash = entry.Entity.RowHash;
                var computedHash = ComputeHash(entry.Entity);
                if (originalHash != computedHash)
                {
                    throw new InvalidOperationException($"Tamper detected on entity {entry.Entity.GetType().Name} with Id {entry.Entity.Id}");
                }
                // Update hash after modifications
                entry.Entity.UpdatedAt = DateTime.UtcNow;
                entry.Entity.RowHash = ComputeHash(entry.Entity);
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    private string ComputeHash(BaseEntity entity)
    {
        // Serialize all properties except Id, RowHash, CreatedAt, UpdatedAt (we want to include values)
        var props = entity.GetType().GetProperties()
            .Where(p => p.Name != nameof(BaseEntity.Id) &&
                        p.Name != nameof(BaseEntity.RowHash) &&
                        p.Name != nameof(BaseEntity.CreatedAt) &&
                        p.Name != nameof(BaseEntity.UpdatedAt))
            .ToDictionary(p => p.Name, p => p.GetValue(entity)?.ToString() ?? "null");

        var json = JsonSerializer.Serialize(props);
        return _hmacService.ComputeHash(json);
    }
}
