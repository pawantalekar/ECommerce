using AutoMapper;
using Ecom.Application.AuthService.Application.DTO;
using Ecom.Application.CatalogService.Application.DTO;
using Ecom.Domain.Entities;

namespace Ecom.Application.MappingProfiles
{
    public  class MappingProfile : Profile
    {
        public MappingProfile() {
            
            CreateMap<Category, CategoryDto>();
            CreateMap<Brand, BrandDto>();
            CreateMap<Product, ProductDTO>()
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ProductImages != null ? src.ProductImages.OrderBy(i => i.SortOrder).Select(i => i.Url).ToList() : new List<string>()))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.ProductTags != null ? src.ProductTags.Select(pt => pt.Tag.Name).ToList() : new List<string>()));
            CreateMap<UserProfile, UpdateProfileDto>();

            CreateMap<Address,AddressDto>().ReverseMap();
        }
    }
}
