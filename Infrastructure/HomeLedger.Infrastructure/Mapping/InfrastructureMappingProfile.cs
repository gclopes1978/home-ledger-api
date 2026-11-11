using AutoMapper;
using HomeLedger.Domain.BusinessModels;
using HomeLedger.Infrastructure.Entities;

namespace HomeLedger.Infrastructure.Mapping;

public sealed class InfrastructureMappingProfile : Profile
{
    public InfrastructureMappingProfile()
    {
        CreateMap<Category, CategoryEntity>().ReverseMap();
        CreateMap<Grouping, GroupingEntity>().ReverseMap();
        CreateMap<Location, LocationEntity>().ReverseMap();
        CreateMap<Expense, ExpenseEntity>().ReverseMap();
        CreateMap<Income, IncomeEntity>().ReverseMap();
        CreateMap<PaymentType, PaymentTypeEntity>().ReverseMap();
    }
}
