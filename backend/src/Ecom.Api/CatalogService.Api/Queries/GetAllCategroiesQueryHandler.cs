using AutoMapper;
using Ecom.Application.CatalogService.Application.DTO;
using Ecom.Application.CatalogService.Application.Interfaces;
using MediatR;

namespace CatalogService.Api.Queries
{
    public class GetAllCategroiesQueryHandler : IRequestHandler<GetAllCategroiesQuery, List<CategoryDto>>
    {
        private readonly ICatalogRepository catalogRepository;
        private readonly IMapper mapper;

        public GetAllCategroiesQueryHandler(ICatalogRepository catalogRepository, IMapper mapper)
        {
            this.catalogRepository = catalogRepository;
            this.mapper = mapper;
        }

        public async Task<List<CategoryDto>> Handle(GetAllCategroiesQuery request, CancellationToken cancellationToken)
        {
            var categories = await catalogRepository.GetAllCategoriesAsync(cancellationToken);
            return mapper.Map<List<CategoryDto>>(categories);
        }
    }
}
