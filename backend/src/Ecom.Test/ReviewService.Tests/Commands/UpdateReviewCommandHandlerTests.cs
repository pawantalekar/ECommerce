using AutoFixture;
using Ecom.Application.ReviewService.Application.DTO;
using Ecom.Application.ReviewService.Application.Interfaces;
using Ecom.Domain.Entities;
using Ecom.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using ReviewService.Api.Commands.UpdateReview;
using System.Security.Claims;
using Xunit;

namespace Ecom.Test.ReviewService.Tests.Commands
{
    public class UpdateReviewCommandHandlerTests
    {
        private readonly Fixture fixture = new();
        private readonly Mock<IReviewRepository> repo = new();
        private readonly Mock<AuthDbContext> db = new();
        private readonly Mock<IHttpContextAccessor> http = new();
        private readonly UpdateReviewCommandHandler handler;
        private readonly Guid userId;

        public UpdateReviewCommandHandlerTests()
        {
            fixture.Behaviors.Remove(fixture.Behaviors.OfType<ThrowingRecursionBehavior>().First());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            userId = fixture.Create<Guid>();
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }, "mock"));

            http.Setup(x => x.HttpContext!.User).Returns(user);

            handler = new UpdateReviewCommandHandler(repo.Object, http.Object, db.Object);
        }

        [Fact]
        public async Task Updates_review_successfully_when_user_is_owner()
        {
            var reviewId = fixture.Create<Guid>();
            var productId = fixture.Create<Guid>();

            var existingReview = fixture.Build<Review>()
                .With(r => r.Id, reviewId)
                .With(r => r.UserId, userId)
                .With(r => r.ProductId, productId)
                .With(r => r.Status, "Approved")
                .Create();

            repo.Setup(r => r.GetByIdAsync(reviewId))
                .ReturnsAsync(existingReview);

            repo.Setup(r => r.UpdateAsync(existingReview)).Returns(Task.CompletedTask);
            repo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var user = fixture.Build<User>().With(u => u.Id, userId).With(u => u.Name, "Pawan T").Create();
            db.Setup(d => d.Users.FindAsync(userId)).ReturnsAsync(user);

            var command = new UpdateReviewCommand(
                reviewId,
                new UpdateReviewRequestDto(4, "Updated comment"));

            var result = await handler.Handle(command, CancellationToken.None);

            existingReview.Rating.Should().Be(4);
            existingReview.Comment.Should().Be("Updated comment");
            existingReview.Status.Should().Be("Pending");
            existingReview.UpdatedAt.Should().NotBeNull();

            result.Rating.Should().Be(4);
            result.Comment.Should().Be("Updated comment");
            result.UserName.Should().Be("Pawan T");
        }

        [Fact]
        public async Task Throws_when_review_not_found_or_not_owned()
        {
            var reviewId = fixture.Create<Guid>();

            repo.Setup(r => r.GetByIdAsync(reviewId))
                .ReturnsAsync((Review?)null);

            var command = new UpdateReviewCommand(
                reviewId,
                new UpdateReviewRequestDto(3, "Trying to hack"));

            await handler.Invoking(h => h.Handle(command, CancellationToken.None))
                         .Should().ThrowAsync<InvalidOperationException>()
                         .WithMessage("Not authorized");
        }

        [Fact]
        public async Task Throws_when_user_tries_to_update_other_review()
        {
            var reviewId = fixture.Create<Guid>();
            var otherUserId = fixture.Create<Guid>();

            var review = fixture.Build<Review>()
                .With(r => r.Id, reviewId)
                .With(r => r.UserId, otherUserId) 
                .Create();

            repo.Setup(r => r.GetByIdAsync(reviewId))
                .ReturnsAsync(review);

            var command = new UpdateReviewCommand(
                reviewId,
                new UpdateReviewRequestDto(5, "Sneaky update"));

            await handler.Invoking(h => h.Handle(command, CancellationToken.None))
                         .Should().ThrowAsync<InvalidOperationException>()
                         .WithMessage("Not authorized");
        }
    }
}