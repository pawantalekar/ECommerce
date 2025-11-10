using Ecom.Application.CatalogService.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.CatalogService.Application.Interfaces
{
    public interface IProductRepository
    {
        public Task AddProduct(ProductDto product);
        
    }
}
