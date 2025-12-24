using AutoMapper;
using Ecom.Application.CatalogService.Application.DTO;
using Ecom.Domain.Entities;

namespace Ecom.Application.MappingProfiles
{
    public  class MappingProfile : Profile
    {
        public MappingProfile() {
            
            CreateMap<Category, CategoryDto>();
            CreateMap<Brand, BrandDto>();
        }
    }
}
