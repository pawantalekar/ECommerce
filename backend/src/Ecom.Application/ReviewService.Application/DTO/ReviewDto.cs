namespace Ecom.Application.ReviewService.Application.DTO
{
    public record ReviewDto(Guid Id, Guid ProductId, Guid UserId, int Rating, string? Comment, DateTime CreatedAt, DateTime? UpdatedAt, string UserName);
}
