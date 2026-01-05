using AutoFixture;
using Ecom.Domain.Entities;
using Ecom.Infrastructure;
using Ecom.Test.CartService.Tests.Commands;
using Ecom.Test.ReviewService.Tests.Commands;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using MockQueryable.Moq;
using Moq;
using OrderService.APi.Queries;
using System.Security.Claims;
using Xunit;

namespace Ecom.Test.OrderService.Tests
{
    public class GetOrderDetailsQueryHandlerTests
    {
        private readonly Fixture f = new();
        private readonly Mock<AuthDbContext> db = new();
        private readonly Mock<IHttpContextAccessor> http = new();
        private readonly GetOrderDetailsQueryHandler handler;
        private readonly Guid userId;

        public GetOrderDetailsQueryHandlerTests()
        {
            f.Behaviors.Remove(f.Behaviors.OfType<ThrowingRecursionBehavior>().First());
            f.Behaviors.Add(new OmitOnRecursionBehavior());

            userId = f.Create<Guid>();
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }, "mock"));

            http.Setup(x => x.HttpContext!.User).Returns(user);
            handler = new GetOrderDetailsQueryHandler(db.Object, http.Object);
        }

        private void SetupOrders(params Order[] orders)
        {
            foreach (var o in orders) o.UserId = userId; 
            var mockSet = orders.BuildMockDbSet();
            db.Setup(d => d.Orders).Returns(mockSet.Object);
        }

        [Fact]
        public async Task Returns_order_details_when_order_belongs_to_user()
        {
            var order = f.Build<Order>()
                .With(o => o.UserId, userId)
                .With(o => o.OrderItems, f.CreateMany<OrderItem>(2).ToList())
                .With(o => o.ShippingFullName, "Pawan T")
                .Create();

            SetupOrders(order);

            var result = await handler.Handle(new GetOrderDetailsQuery(order.Id), CancellationToken.None);

            result.Id.Should().Be(order.Id);
            result.OrderNumber.Should().Be(order.OrderNumber);
            result.Items.Should().HaveCount(2);
            result.ShippingAddress.FullName.Should().Be("Pawan T");
        }

        [Fact]
        public async Task Throws_KeyNotFoundException_when_order_not_found_or_not_owned()
        {
            var otherUserId = f.Create<Guid>();
            var otherUserOrder = f.Build<Order>()
                .With(o => o.UserId, otherUserId)
                .Create();

            var mockSet = new[] { otherUserOrder }.BuildMockDbSet();
            db.Setup(d => d.Orders).Returns(mockSet.Object);

            var query = new GetOrderDetailsQuery(otherUserOrder.Id);

            await handler.Invoking(h => h.Handle(query, CancellationToken.None))
                         .Should().ThrowAsync<KeyNotFoundException>()
                         .WithMessage("Order not found");
        }

        [Fact]
        public async Task Returns_nothing_when_no_orders()
        {
            SetupOrders(); 

            var query = new GetOrderDetailsQuery(f.Create<Guid>());

            await handler.Invoking(h => h.Handle(query, CancellationToken.None))
                         .Should().ThrowAsync<KeyNotFoundException>();
        }
    }
}