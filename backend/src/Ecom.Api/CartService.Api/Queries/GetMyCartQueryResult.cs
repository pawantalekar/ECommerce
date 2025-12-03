using Ecom.Application.CartService.Application.DTO;

namespace CartService.Api.Queries
{
    public class GetMyCartQueryResult
    {
        public CartDto Cart { get; set; } = null!;
    }
}
