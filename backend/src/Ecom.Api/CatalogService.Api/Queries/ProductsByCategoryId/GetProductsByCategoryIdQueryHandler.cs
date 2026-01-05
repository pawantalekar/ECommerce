using AutoMapper;
using Ecom.Application.CatalogService.Application.DTO;
using Ecom.Application.CatalogService.Application.Interfaces;
using MediatR;

namespace CatalogService.Api.Queries.ProductsByCategoryId
{
    public class GetProductsByCategoryIdQueryHandler : IRequestHandler<GetProductsByCategoryIdQuery, List<ProductDTO>>
    {
        private readonly ICatalogRepository _catalogRepository;
        private readonly IMapper mapper;

        public GetProductsByCategoryIdQueryHandler(ICatalogRepository catalogRepository, IMapper mapper)
        {
            _catalogRepository = catalogRepository;
            this.mapper = mapper;
        }

        public async Task<List<ProductDTO>> Handle(GetProductsByCategoryIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _catalogRepository.GetProductsByCategoryIdAsync(request.Id, cancellationToken);
            return mapper.Map<List<ProductDTO>>(result);
        }
    }
}
