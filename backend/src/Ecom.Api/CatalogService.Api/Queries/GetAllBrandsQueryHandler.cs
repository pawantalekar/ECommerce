using AutoMapper;
using Ecom.Application.CatalogService.Application.DTO;
using Ecom.Application.CatalogService.Application.Interfaces;
using MediatR;

namespace CatalogService.Api.Queries
{
    public class GetAllBrandsQueryHandler : IRequestHandler<GetAllBrandsQuery, List<BrandDto>>
    {
        private readonly ICatalogRepository catalogrepository;
        private readonly IMapper mapper;

        public GetAllBrandsQueryHandler(ICatalogRepository catalogrepository, IMapper mapper)
        {
            this.catalogrepository = catalogrepository;
            this.mapper = mapper;
        }

        public async Task<List<BrandDto>> Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
        {
            var brands = await catalogrepository.GetAllBrandsAsync(cancellationToken);

            return mapper.Map<List<BrandDto>>(brands);
        }
    }
}
