namespace Ecom.Application.ReviewService.Application.DTO
{
    public record PendingReviewDto(
     Guid Id,
     Guid ProductId,
     string ProductName,
     Guid UserId,
     string UserName,
     int Rating,
     string? Comment,
     DateTime CreatedAt
 );
}
