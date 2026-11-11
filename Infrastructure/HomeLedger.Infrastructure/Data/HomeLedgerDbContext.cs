using HomeLedger.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
namespace HomeLedger.Infrastructure.Data;

public sealed class HomeLedgerDbContext(DbContextOptions<HomeLedgerDbContext> options) : DbContext(options)
{
    public DbSet<CategoryEntity> Categories => Set<CategoryEntity>();
    public DbSet<GroupingEntity> Groupings => Set<GroupingEntity>();
    public DbSet<LocationEntity> Locations => Set<LocationEntity>();
    public DbSet<ExpenseEntity> Expenses => Set<ExpenseEntity>();
    public DbSet<IncomeEntity> Incomes => Set<IncomeEntity>();
    public DbSet<PaymentTypeEntity> PaymentTypes => Set<PaymentTypeEntity>();
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

        var location = modelBuilder.Entity<LocationEntity>();
        location.ToTable("Locations");
        location.Property(x => x.Name)
            .HasMaxLength(150)
            .IsRequired();
        location.Property(x => x.Description)
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

        var expense = modelBuilder.Entity<ExpenseEntity>();
        expense.ToTable("Expenses");
        expense.HasOne(x => x.Location)
            .WithMany()
            .HasForeignKey(x => x.LocationId)
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
