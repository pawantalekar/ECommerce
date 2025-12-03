namespace Ecom.Application.ReviewService.Application.DTO
{
    public record CreateReviewRequestDto(Guid ProductId, int Rating, string? Comment);
}
