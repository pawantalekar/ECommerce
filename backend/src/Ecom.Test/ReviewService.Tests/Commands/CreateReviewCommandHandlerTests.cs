using AutoFixture;
using Ecom.Application.ReviewService.Application.DTO;
using Ecom.Application.ReviewService.Application.Interfaces;
using Ecom.Domain.Entities;
using Ecom.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using ReviewService.Api.Commands.CreateReview;
using System.Security.Claims;
using Xunit;

namespace Ecom.Test.ReviewService.Tests.Commands
{
    public class CreateReviewCommandHandlerTests
    {
        private readonly Fixture fixture = new();
        private readonly Mock<IReviewRepository> repo = new();
        private readonly Mock<AuthDbContext> db = new();
        private readonly Mock<IHttpContextAccessor> http = new();
        private readonly CreateReviewCommandHandler handler;
        private readonly Guid userId;

        public CreateReviewCommandHandlerTests()
        {
            fixture.Behaviors.Remove(fixture.Behaviors.OfType<ThrowingRecursionBehavior>().First());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            userId = fixture.Create<Guid>();
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }, "mock"));

            http.Setup(x => x.HttpContext!.User).Returns(user);
            handler = new CreateReviewCommandHandler(repo.Object, http.Object, db.Object);
        }

        private void SetupUserHasPurchasedAndNotReviewed(Guid productId, bool canReview = true)
        {
            repo.Setup(r => r.HasPurchasedAndNotReviewedAsync(userId, productId))
                .ReturnsAsync(canReview);
        }

        private void SetupPaidOrderWithProduct(Guid productId, Guid orderId)
        {
            var orderItem = new OrderItem
            {
                ProductId = productId,
                OrderId = orderId,
                Order = new Order { UserId = userId, PaymentStatus = "Paid", Id = orderId }
            };

            var orderItems = new[] { orderItem }.BuildMockDbSet();
            db.Setup(d => d.OrderItems).Returns(orderItems.Object);
        }

        private void SetupUserName(string name = "Pawan T")
        {
            var user = fixture.Build<User>().With(u => u.Id, userId).With(u => u.Name, name).Create();
            db.Setup(d => d.Users.FindAsync(userId))
                .ReturnsAsync(user);
        }

        [Fact]
        public async Task Creates_review_successfully_when_user_has_purchased()
        {
            var productId = fixture.Create<Guid>();
            var orderId = fixture.Create<Guid>();

            SetupUserHasPurchasedAndNotReviewed(productId, true);
            SetupPaidOrderWithProduct(productId, orderId);
            SetupUserName();

            repo.Setup(r => r.AddAsync(It.IsAny<Review>())).Returns(Task.CompletedTask);
            repo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

            var command = new CreateReviewCommand(
                new CreateReviewRequestDto(productId, 5, "Great product!"));

            var result = await handler.Handle(command, CancellationToken.None);

            result.Rating.Should().Be(5);
            result.Comment.Should().Be("Great product!");
            result.UserName.Should().Be("Pawan T");
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Throws_when_user_cannot_review_product()
        {
            var productId = fixture.Create<Guid>();

            SetupUserHasPurchasedAndNotReviewed(productId, false);

            var command = new CreateReviewCommand(
                new CreateReviewRequestDto(productId, 4, "Nice"));

            await handler.Invoking(h => h.Handle(command, CancellationToken.None))
                         .Should().ThrowAsync<InvalidOperationException>()
                         .WithMessage("Cannot review this product");
        }
    }
}