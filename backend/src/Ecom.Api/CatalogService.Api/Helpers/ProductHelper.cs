namespace CatalogService.Api.Helpers
{
    public class ProductHelper
    {
        public static Task<bool> ValidateProductPrice(decimal price)
        {
            bool isValid = price > 0;
            return Task.FromResult(isValid);
        }
    }
}
