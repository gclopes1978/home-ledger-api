using AutoMapper;
using HomeLedger.Domain.Interfaces.Repositories;
using HomeLedger.Infrastructure.Data;
using HomeLedger.Infrastructure.Data.Implementations;
using HomeLedger.Infrastructure.Data.Interfaces.Categories;
using HomeLedger.Infrastructure.Data.Interfaces.Expenses;
using HomeLedger.Infrastructure.Data.Interfaces.Groupings;
using HomeLedger.Infrastructure.Data.Interfaces.Incomes;
using HomeLedger.Infrastructure.Data.Interfaces.Locations;
using HomeLedger.Infrastructure.Data.Interfaces.PaymentTypes;
using HomeLedger.Infrastructure.Data.Repositories;
using HomeLedger.Infrastructure.Mapping;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace HomeLedger.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddHomeLedgerInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("HomeLedger") ?? throw new InvalidOperationException("The HomeLedger SQL Server connection string is not configured.");
        services.AddDbContext<HomeLedgerDbContext>(options => options.UseSqlServer(connectionString));
        services.AddAutoMapper(_ => { }, typeof(InfrastructureMappingProfile));

        services.AddScoped<ICategoryData, CategoryData>();
        services.AddScoped<IGroupingData, GroupingData>();
        services.AddScoped<ILocationData, LocationData>();
        services.AddScoped<IExpenseData, ExpenseData>();
        services.AddScoped<IIncomeData, IncomeData>();
        services.AddScoped<IPaymentTypeData, PaymentTypeData>();

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IGroupingRepository, GroupingRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<IExpenseRepository, ExpenseRepository>();
        services.AddScoped<IIncomeRepository, IncomeRepository>();
        services.AddScoped<IPaymentTypeRepository, PaymentTypeRepository>();
        return services;
    }
}
