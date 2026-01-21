using Microsoft.EntityFrameworkCore;
using PersonalFinance.Domain.Entities;
using PersonalFinance.Domain.Common;

namespace PersonalFinance.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // DbSets for all entities
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<FamilyMember> FamilyMembers { get; set; } = null!;
    public DbSet<BankAccount> BankAccounts { get; set; } = null!;
    public DbSet<FixedDeposit> FixedDeposits { get; set; } = null!;
    public DbSet<ProvidentFund> ProvidentFunds { get; set; } = null!;
    public DbSet<PFTransaction> PFTransactions { get; set; } = null!;
    public DbSet<StockHolding> StockHoldings { get; set; } = null!;
    public DbSet<StockTransaction> StockTransactions { get; set; } = null!;
    public DbSet<MutualFundHolding> MutualFundHoldings { get; set; } = null!;
    public DbSet<MutualFundTransaction> MutualFundTransactions { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configure FamilyMember entity
        modelBuilder.Entity<FamilyMember>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                .WithMany(u => u.FamilyMembers)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configure BankAccount entity
        modelBuilder.Entity<BankAccount>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CurrentBalance).HasPrecision(18, 2);
            entity.HasOne(e => e.User)
                .WithMany(u => u.BankAccounts)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configure FixedDeposit entity
        modelBuilder.Entity<FixedDeposit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PrincipalAmount).HasPrecision(18, 2);
            entity.Property(e => e.InterestRate).HasPrecision(5, 2);
            entity.Property(e => e.MaturityAmount).HasPrecision(18, 2);
            entity.HasOne(e => e.BankAccount)
                .WithMany(b => b.FixedDeposits)
                .HasForeignKey(e => e.BankAccountId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configure ProvidentFund entity
        modelBuilder.Entity<ProvidentFund>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EmployeeContribution).HasPrecision(18, 2);
            entity.Property(e => e.EmployerContribution).HasPrecision(18, 2);
            entity.Property(e => e.CurrentBalance).HasPrecision(18, 2);
            entity.Property(e => e.InterestRate).HasPrecision(5, 2);
                
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configure PFTransaction entity
        modelBuilder.Entity<PFTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EmployeeContribution).HasPrecision(18, 2);
            entity.Property(e => e.EmployerContribution).HasPrecision(18, 2);
            entity.Property(e => e.InterestCredited).HasPrecision(18, 2);
            entity.Property(e => e.ClosingBalance).HasPrecision(18, 2);
            entity.HasOne(e => e.ProvidentFund)
                .WithMany(p => p.PFTransactions)
                .HasForeignKey(e => e.ProvidentFundId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configure StockHolding entity
        modelBuilder.Entity<StockHolding>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Quantity).HasPrecision(18, 4);
            entity.Property(e => e.AverageBuyPrice).HasPrecision(18, 2);
            entity.Property(e => e.CurrentPrice).HasPrecision(18, 2);
            entity.HasOne(e => e.User)
                .WithMany(u => u.StockHoldings)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configure StockTransaction entity
        modelBuilder.Entity<StockTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Quantity).HasPrecision(18, 4);
            entity.Property(e => e.Price).HasPrecision(18, 2);
            entity.Property(e => e.Brokerage).HasPrecision(18, 2);
            entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
            entity.HasOne(e => e.StockHolding)
                .WithMany(s => s.StockTransactions)
                .HasForeignKey(e => e.StockHoldingId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configure MutualFundHolding entity
        modelBuilder.Entity<MutualFundHolding>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Units).HasPrecision(18, 4);
            entity.Property(e => e.AverageNAV).HasPrecision(18, 4);
            entity.Property(e => e.CurrentNAV).HasPrecision(18, 4);
            entity.HasOne(e => e.User)
                .WithMany(u => u.MutualFundHoldings)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configure MutualFundTransaction entity
        modelBuilder.Entity<MutualFundTransaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Units).HasPrecision(18, 4);
            entity.Property(e => e.NAV).HasPrecision(18, 4);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.HasOne(e => e.MutualFundHolding)
                .WithMany(m => m.MutualFundTransactions)
                .HasForeignKey(e => e.MutualFundHoldingId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configure Transaction entity
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Amount).HasPrecision(18, 2);
            entity.Property(e => e.Balance).HasPrecision(18, 2);
            entity.HasOne(e => e.BankAccount)
                .WithMany(b => b.Transactions)
                .HasForeignKey(e => e.BankAccountId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configure RefreshToken entity
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Token).IsRequired().HasMaxLength(500);
            entity.HasIndex(e => e.Token).IsUnique();
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configure PasswordResetToken entity
        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Token).IsRequired().HasMaxLength(500);
            entity.HasIndex(e => e.Token).IsUnique();
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasQueryFilter(e => !e.IsDeleted);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();
        
        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}
