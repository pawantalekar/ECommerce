using AutoFixture;
using Ecom.Application.ReviewService.Application.DTO;
using Ecom.Application.ReviewService.Application.Interfaces;
using Ecom.Domain.Entities;
using Ecom.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using ReviewService.Api.Queries;
using System.Security.Claims;
using Xunit;

namespace Ecom.Test.ReviewService.Tests.Queries
{
    public class GetMyReviewForProductQueryHandlerTests
    {
        private readonly Fixture fixture = new();
        private readonly Mock<IReviewRepository> repo = new();
        private readonly Mock<AuthDbContext> db = new();
        private readonly Mock<IHttpContextAccessor> http = new();
        private readonly GetMyReviewForProductQueryHandler handler;
        private readonly Guid userId;

        public GetMyReviewForProductQueryHandlerTests()
        {
            fixture.Behaviors.Remove(fixture.Behaviors.OfType<ThrowingRecursionBehavior>().First());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            userId = fixture.Create<Guid>();
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }, "mock"));

            http.Setup(x => x.HttpContext!.User).Returns(user);

            handler = new GetMyReviewForProductQueryHandler(repo.Object, http.Object, db.Object);
        }

        [Fact]
        public async Task Returns_current_user_review_when_exists()
        {
            var productId = fixture.Create<Guid>();
            var review = fixture.Build<Review>()
                .With(r => r.UserId, userId)
                .With(r => r.ProductId, productId)
                .With(r => r.Rating, 5)
                .With(r => r.Comment, "Amazing!")
                .Create();

            var user = fixture.Build<User>().With(u => u.Id, userId).With(u => u.Name, "Pawan T").Create();
            db.Setup(d => d.Users.Find(userId)).Returns(user);

            repo.Setup(r => r.GetUserReviewForProductAsync(userId, productId))
                .ReturnsAsync(review);

            var result = await handler.Handle(new GetMyReviewForProductQuery(productId), CancellationToken.None);

            result.Should().NotBeNull();
            result!.Rating.Should().Be(5);
            result.Comment.Should().Be("Amazing!");
            result.UserName.Should().Be("Pawan T");
        }

        [Fact]
        public async Task Returns_null_when_no_review_found()
        {
            var productId = fixture.Create<Guid>();

            repo.Setup(r => r.GetUserReviewForProductAsync(userId, productId))
                .ReturnsAsync((Review?)null);

            var result = await handler.Handle(new GetMyReviewForProductQuery(productId), CancellationToken.None);

            result.Should().BeNull();
        }
    }
}