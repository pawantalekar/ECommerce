namespace Ecom.Application.AuthService.Application.DTO
{
    public record SellerRequestDto(
     Guid Id,
     Guid UserId,
     string UserName,
     string UserEmail,
     DateTime RequestedAt,
     string Status
 );
}
