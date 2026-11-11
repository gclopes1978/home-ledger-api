using HomeLedger.Application.UseCases;
using HomeLedger.Domain.Interfaces.Application;
using Microsoft.Extensions.DependencyInjection;
namespace HomeLedger.Application.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddHomeLedgerApplication(this IServiceCollection services)
    {
        services.AddScoped<ICategoryUseCase, CategoryUseCase>();
        services.AddScoped<IGroupingUseCase, GroupingUseCase>();
        services.AddScoped<ILocationUseCase, LocationUseCase>();
        services.AddScoped<IExpenseUseCase, ExpenseUseCase>();
        services.AddScoped<IIncomeUseCase, IncomeUseCase>();
        services.AddScoped<IPaymentTypeUseCase, PaymentTypeUseCase>();
        return services;
    }
}
