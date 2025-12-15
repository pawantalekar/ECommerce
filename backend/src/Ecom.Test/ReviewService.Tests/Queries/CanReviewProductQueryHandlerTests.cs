using AutoFixture;
using Ecom.Application.ReviewService.Application.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using ReviewService.Api.Queries;
using System.Security.Claims;
using Xunit;

namespace Ecom.Test.ReviewService.Tests.Queries
{
    public class CanReviewProductQueryHandlerTests
    {
        private readonly Fixture fixture = new();
        private readonly Mock<IReviewRepository> repo = new();
        private readonly Mock<IHttpContextAccessor> http = new();
        private readonly CanReviewProductQueryHandler handler;
        private readonly Guid userId;

        public CanReviewProductQueryHandlerTests()
        {
            fixture.Behaviors.Remove(fixture.Behaviors.OfType<ThrowingRecursionBehavior>().First());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            userId = fixture.Create<Guid>();
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }, "mock"));

            http.Setup(x => x.HttpContext!.User).Returns(user);

            handler = new CanReviewProductQueryHandler(repo.Object, http.Object);
        }

        [Fact]
        public async Task Returns_true_when_user_can_review_product()
        {
            var productId = fixture.Create<Guid>();

            repo.Setup(r => r.HasPurchasedAndNotReviewedAsync(userId, productId))
                .ReturnsAsync(true);

            var result = await handler.Handle(new CanReviewProductQuery(productId), CancellationToken.None);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task Returns_false_when_user_cannot_review_product()
        {
            var productId = fixture.Create<Guid>();

            repo.Setup(r => r.HasPurchasedAndNotReviewedAsync(userId, productId))
                .ReturnsAsync(false);

            var result = await handler.Handle(new CanReviewProductQuery(productId), CancellationToken.None);

            result.Should().BeFalse();
        }
    }
}