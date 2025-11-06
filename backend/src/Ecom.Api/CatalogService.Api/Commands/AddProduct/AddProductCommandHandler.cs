//using Ecom.Application.CatalogService.Application.Interfaces;
//using MediatR;

//namespace CatalogService.Api.Commands.AddProduct
//{
//    public class AddProductCommandHandler : IRequestHandler<AddProductCommand, AddProductCommandResult>
//    {

//        private readonly IProductRepository produtRepository;

//       public AddProductCommandHandler(IProductRepository produtRepository)
//        {
//            this.produtRepository = produtRepository;
//        }

//        public async Task<AddProductCommandResult> Handle(AddProductCommand request, CancellationToken cancellationToken)
//        {
//            await produtRepository.AddProduct(request.Product);

//            return new AddProductCommandResult
//            {
//                Product = request.Product,
//                message = "Product  added successfully!"
//            };
//        }
//    }
//}
