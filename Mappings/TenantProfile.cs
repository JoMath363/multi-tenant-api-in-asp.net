using AutoMapper;
using Multi_Tenant_API.Dtos;
using Multi_Tenant_API.Models;

namespace Multi_Tenant_API.Mappings;

public class TenantProfile : Profile
{
  public TenantProfile()
  {
    CreateMap<TenantDto, TenantModel>();
    CreateMap<TenantModel, TenantDto>();
  }
}