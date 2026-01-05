namespace CatalogService.Api.Helpers
{
    public class ProductHelper
    {
        public static bool ValidateProductPrice(decimal price)
        {
            bool isValid = price > 0;
            return isValid;
        }
    }
}
