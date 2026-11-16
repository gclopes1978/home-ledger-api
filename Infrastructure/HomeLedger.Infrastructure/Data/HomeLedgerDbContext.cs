using HomeLedger.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
namespace HomeLedger.Infrastructure.Data;

public sealed class HomeLedgerDbContext(DbContextOptions<HomeLedgerDbContext> options) : DbContext(options)
{
    public DbSet<CategoryEntity> Categories => Set<CategoryEntity>();
    public DbSet<GroupingEntity> Groupings => Set<GroupingEntity>();
    public DbSet<SourceEntity> Sources => Set<SourceEntity>();
    public DbSet<ExpenseEntity> Expenses => Set<ExpenseEntity>();
    public DbSet<IncomeEntity> Incomes => Set<IncomeEntity>();
    public DbSet<PaymentTypeEntity> PaymentTypes => Set<PaymentTypeEntity>();
    public DbSet<IdempotencyRecordEntity> IdempotencyRecords => Set<IdempotencyRecordEntity>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var category = modelBuilder.Entity<CategoryEntity>();
        category.ToTable("Categories");
        category.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        var grouping = modelBuilder.Entity<GroupingEntity>();
        grouping.ToTable("Groupings");
        grouping.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();

        var source = modelBuilder.Entity<SourceEntity>();
        source.ToTable("Sources");
        source.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();
        source.Property(x => x.Description)
            .HasMaxLength(500);

        var income = modelBuilder.Entity<IncomeEntity>();
        income.ToTable("Incomes");
        income.Property(x => x.Description)
            .HasMaxLength(500)
            .IsRequired();
        income.Property(x => x.Amount)
            .HasPrecision(18, 2);

        var paymentType = modelBuilder.Entity<PaymentTypeEntity>();
        paymentType.ToTable("PaymentTypes");
        paymentType.Property(x => x.Description)
            .HasMaxLength(100)
            .IsRequired();

        category.HasOne(x => x.Grouping)
            .WithMany(x => x.Categories)
            .HasForeignKey(x => x.GroupingId)
            .OnDelete(DeleteBehavior.Restrict);

        var idempotencyRecord = modelBuilder.Entity<IdempotencyRecordEntity>();
        idempotencyRecord.ToTable("IdempotencyRecords");
        idempotencyRecord.Property(x => x.IdempotencyKey)
            .HasMaxLength(128)
            .IsRequired();
        idempotencyRecord.Property(x => x.RequestHash)
            .HasMaxLength(64)
            .IsRequired();
        idempotencyRecord.Property(x => x.ResponseContentType)
            .HasMaxLength(200);
        idempotencyRecord.Property(x => x.ResponseLocation)
            .HasMaxLength(2048);
        idempotencyRecord.HasIndex(x => x.IdempotencyKey)
            .IsUnique();
        var expense = modelBuilder.Entity<ExpenseEntity>();
        expense.ToTable("Expenses");
        expense.HasOne(x => x.Source)
            .WithMany()
            .HasForeignKey(x => x.SourceId)
            .OnDelete(DeleteBehavior.Restrict);
        expense.HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        expense.HasOne(x => x.PaymentType)
            .WithMany(x => x.Expenses)
            .HasForeignKey(x => x.PaymentTypeId)
            .OnDelete(DeleteBehavior.Restrict);
        expense.Property(x => x.PurchaseAmount)
            .HasPrecision(18, 2);
        expense.Property(x => x.DiscountAmount)
            .HasPrecision(18, 2);
        expense.Property(x => x.Total)
            .HasPrecision(18, 2);
    }
}
