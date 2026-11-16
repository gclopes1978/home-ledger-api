using AutoMapper;
using HomeLedger.ApiContract.DTOs;
using HomeLedger.Domain.BusinessModels;

namespace HomeLedger.Api.Mapping;

public sealed class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        CreateMap<CategoryDto, Category>()
            .ForMember(destination => destination.Grouping, options => options.Ignore());
        CreateMap<Category, CategoryDto>();

        CreateMap<GroupingDto, Grouping>().ReverseMap();
        CreateMap<SourceDto, Source>().ReverseMap();

        CreateMap<ExpenseDto, Expense>()
            .ForMember(destination => destination.Category, options => options.Ignore())
            .ForMember(destination => destination.Source, options => options.Ignore())
            .ForMember(destination => destination.PaymentType, options => options.Ignore());
        CreateMap<Expense, ExpenseDto>();

        CreateMap<IncomeDto, Income>().ReverseMap();
        CreateMap<PaymentTypeDto, PaymentType>().ReverseMap();
    }
}