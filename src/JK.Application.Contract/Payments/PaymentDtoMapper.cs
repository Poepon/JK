using Abp.Reflection.Extensions;
using AutoMapper;
using JK.Payments.Bacis;
using JK.Payments.Dto;
using JK.Payments.Integration;
using JK.Payments.Integration.Dto;
using JK.Payments.Integration.Dto.ApiParameters;
using JK.Payments.Orders;
using JK.Payments.Orders.Dto;
using JK.Payments.TenantConfigs.Dto;
using JK.Payments.Tenants;

namespace JK.Payments
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            this.CreateMap<Bank, BankDto>().ReverseMap();
            this.CreateMap<Channel, ChannelDto>().ReverseMap();

            this.CreateMap<ResultCode, ResultCodeDto>().ReverseMap();
            this.CreateMap<ApiParameter, ApiParameterDto>().ReverseMap();
            this.CreateMap<Company, CompanyDto>().ReverseMap();
            this.CreateMap<PaymentOrder, PaymentOrderDto>().ReverseMap();
            this.CreateMap<CompanyAccount, CompanyAccountDto>().ReverseMap();
            this.CreateMap<ApiConfiguration, ApiConfigurationDto>().ReverseMap();
            


        }
    }
}
