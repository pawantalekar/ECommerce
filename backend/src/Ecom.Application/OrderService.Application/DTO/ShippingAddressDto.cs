using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Application.OrderService.Application.DTO
{
    public record ShippingAddressDto(
         string FullName,
         string Phone,
         string AddressLine1,
         string? AddressLine2,
         string City,
         string State,
         string Pincode
     );
}
