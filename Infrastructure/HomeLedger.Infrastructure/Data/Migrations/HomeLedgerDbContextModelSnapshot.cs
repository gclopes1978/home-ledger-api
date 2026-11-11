using HomeLedger.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace HomeLedger.Infrastructure.Data.Migrations;

[DbContext(typeof(HomeLedgerDbContext))]
public sealed class HomeLedgerDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "10.0.14")
            .HasAnnotation("Relational:MaxIdentifierLength", 128);

        modelBuilder.Entity("HomeLedger.Infrastructure.Entities.CategoryEntity", entity =>
        {
            entity.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
            entity.Property<int>("GroupingId").HasColumnType("int");
            entity.Property<string>("Name").IsRequired().HasMaxLength(150).HasColumnType("nvarchar(150)");
            entity.HasKey("Id");
            entity.HasIndex("GroupingId");
            entity.ToTable("Categories");
        });

        modelBuilder.Entity("HomeLedger.Infrastructure.Entities.ExpenseEntity", entity =>
        {
            entity.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
            entity.Property<int>("CategoryId").HasColumnType("int");
            entity.Property<DateOnly>("Date").HasColumnType("date");
            entity.Property<decimal>("DiscountAmount").HasPrecision(18, 2).HasColumnType("decimal(18,2)");
            entity.Property<int>("LocationId").HasColumnType("int");
            entity.Property<int>("PaymentTypeId").HasColumnType("int");
            entity.Property<decimal>("PurchaseAmount").HasPrecision(18, 2).HasColumnType("decimal(18,2)");
            entity.Property<decimal>("Total").HasPrecision(18, 2).HasColumnType("decimal(18,2)");
            entity.HasKey("Id");
            entity.HasIndex("CategoryId");
            entity.HasIndex("LocationId");
            entity.HasIndex("PaymentTypeId");
            entity.ToTable("Expenses");
        });

        modelBuilder.Entity("HomeLedger.Infrastructure.Entities.PaymentTypeEntity", entity =>
        {
            entity.Navigation("Expenses");
        });
        modelBuilder.Entity("HomeLedger.Infrastructure.Entities.GroupingEntity", entity =>
        {
            entity.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
            entity.Property<string>("Name").IsRequired().HasMaxLength(150).HasColumnType("nvarchar(150)");
            entity.HasKey("Id");
            entity.ToTable("Groupings");
        });

        modelBuilder.Entity("HomeLedger.Infrastructure.Entities.IncomeEntity", entity =>
        {
            entity.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
            entity.Property<decimal>("Amount").HasPrecision(18, 2).HasColumnType("decimal(18,2)");
            entity.Property<DateOnly>("Date").HasColumnType("date");
            entity.Property<string>("Description").IsRequired().HasMaxLength(500).HasColumnType("nvarchar(500)");
            entity.HasKey("Id");
            entity.ToTable("Incomes");
        });

        modelBuilder.Entity("HomeLedger.Infrastructure.Entities.LocationEntity", entity =>
        {
            entity.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
            entity.Property<string>("Description").HasMaxLength(500).HasColumnType("nvarchar(500)");
            entity.Property<string>("Name").IsRequired().HasMaxLength(150).HasColumnType("nvarchar(150)");
            entity.HasKey("Id");
            entity.ToTable("Locations");
        });

        modelBuilder.Entity("HomeLedger.Infrastructure.Entities.PaymentTypeEntity", entity =>
        {
            entity.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType("int");
            entity.Property<string>("Description").IsRequired().HasMaxLength(100).HasColumnType("nvarchar(100)");
            entity.HasKey("Id");
            entity.ToTable("PaymentTypes");
        });

        modelBuilder.Entity("HomeLedger.Infrastructure.Entities.CategoryEntity", entity =>
        {
            entity.HasOne("HomeLedger.Infrastructure.Entities.GroupingEntity", "Grouping")
                .WithMany("Categories")
                .HasForeignKey("GroupingId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            entity.Navigation("Grouping");
        });

        modelBuilder.Entity("HomeLedger.Infrastructure.Entities.ExpenseEntity", entity =>
        {
            entity.HasOne("HomeLedger.Infrastructure.Entities.CategoryEntity", "Category")
                .WithMany()
                .HasForeignKey("CategoryId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            entity.HasOne("HomeLedger.Infrastructure.Entities.LocationEntity", "Location")
                .WithMany()
                .HasForeignKey("LocationId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            entity.Navigation("Category");
            entity.Navigation("Location");
            entity.HasOne("HomeLedger.Infrastructure.Entities.PaymentTypeEntity", "PaymentType")
                .WithMany("Expenses")
                .HasForeignKey("PaymentTypeId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
            entity.Navigation("PaymentType");
        });

        modelBuilder.Entity("HomeLedger.Infrastructure.Entities.PaymentTypeEntity", entity =>
        {
            entity.Navigation("Expenses");
        });
        modelBuilder.Entity("HomeLedger.Infrastructure.Entities.GroupingEntity", entity =>
        {
            entity.Navigation("Categories");
        });
#pragma warning restore 612, 618
    }
}