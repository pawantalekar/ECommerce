using Ecom.Application.ReviewService.Application.DTO;
using Ecom.Application.ReviewService.Application.Interfaces;
using Ecom.Domain.Entities;
using Ecom.Domain.Enums;
using Ecom.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ReviewService.Api.Commands.CreateReview
{
    public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, ReviewDto>
    {
        private readonly IReviewRepository _repo;
        private readonly IHttpContextAccessor _http;
        private readonly AuthDbContext _context;

        public CreateReviewCommandHandler(IReviewRepository repo, IHttpContextAccessor http, AuthDbContext context)
        {
            _repo = repo; _http = http; _context = context;
        }

        public async Task<ReviewDto> Handle(CreateReviewCommand cmd, CancellationToken ct)
        {
            var userId = Guid.Parse(_http.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var req = cmd.Request;

            if (!await _repo.HasPurchasedAndNotReviewedAsync(userId, req.ProductId))
                throw new InvalidOperationException("Cannot review this product");

            var orderId = await _context.OrderItems
                .Where(oi => oi.Order.UserId == userId && oi.ProductId == req.ProductId && oi.Order.PaymentStatus == PaymentStatusEnum.Paid)
                .Select(oi => oi.OrderId)
                .FirstAsync(ct);

            var review = new Review
            {
                Id = Guid.NewGuid(),
                ProductId = req.ProductId,
                UserId = userId,
                OrderId = orderId,
                Rating = req.Rating,
                Comment = req.Comment,
                CreatedAt = DateTime.UtcNow,
                Status = "Pending",
                ApprovedById = null,
                ApprovedAt = null
            };

            await _repo.AddAsync(review);
            await _repo.SaveChangesAsync();

            var user = await _context.Users.FindAsync(userId);
            return new ReviewDto(review.Id, review.ProductId, review.UserId, review.Rating, review.Comment, review.CreatedAt, null, user!.Name);
        }
    }
}
