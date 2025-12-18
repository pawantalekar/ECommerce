using AutoFixture;
using Ecom.Application.ReviewService.Application.Interfaces;
using Ecom.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using ReviewService.Api.Commands.DeleteReview;
using System.Security.Claims;
using Xunit;

namespace Ecom.Test.ReviewService.Tests.Commands
{
    public class DeleteReviewCommandHandlerTests
    {
        private readonly Fixture fixture = new();
        private readonly Mock<IReviewRepository> repo = new();
        private readonly Mock<IHttpContextAccessor> http = new();
        private readonly DeleteReviewCommandHandler handler;
        private readonly Guid userId;

        public DeleteReviewCommandHandlerTests()
        {
            fixture.Behaviors.Remove(fixture.Behaviors.OfType<ThrowingRecursionBehavior>().First());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            userId = fixture.Create<Guid>();
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }, "mock"));

            http.Setup(x => x.HttpContext!.User).Returns(user);

            handler = new DeleteReviewCommandHandler(repo.Object, http.Object);
        }

        [Fact]
        public async Task Deletes_review_when_user_is_owner()
        {
            var reviewId = fixture.Create<Guid>();
            var review = fixture.Build<Review>()
                .With(r => r.Id, reviewId)
                .With(r => r.UserId, userId)
                .Create();

            repo.Setup(r => r.GetByIdAsync(reviewId))
                .ReturnsAsync(review);

            repo.Setup(r => r.DeleteAsync(review)).Returns(Task.CompletedTask);
            repo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var command = new DeleteReviewCommand(reviewId);

            await handler.Handle(command, CancellationToken.None);

            repo.Verify(r => r.DeleteAsync(review), Times.Once);
            repo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Throws_when_review_not_found()
        {
            var reviewId = fixture.Create<Guid>();

            repo.Setup(r => r.GetByIdAsync(reviewId))
                .ReturnsAsync((Review?)null);

            var command = new DeleteReviewCommand(reviewId);

            await handler.Invoking(h => h.Handle(command, CancellationToken.None))
                         .Should().ThrowAsync<InvalidOperationException>()
                         .WithMessage("Not authorized");
        }

        [Fact]
        public async Task Throws_when_user_is_not_owner()
        {
            var reviewId = fixture.Create<Guid>();
            var otherUserId = fixture.Create<Guid>();

            var review = fixture.Build<Review>()
                .With(r => r.Id, reviewId)
                .With(r => r.UserId, otherUserId) 
                .Create();

            repo.Setup(r => r.GetByIdAsync(reviewId))
                .ReturnsAsync(review);

            var command = new DeleteReviewCommand(reviewId);

            await handler.Invoking(h => h.Handle(command, CancellationToken.None))
                         .Should().ThrowAsync<InvalidOperationException>()
                         .WithMessage("Not authorized");
        }
    }
}