using AutoFixture;
using Ecom.Application.Queries;
using Ecom.Domain.Entities;
using Ecom.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using MockQueryable.Moq;
using Moq;
using OrderService.APi.Queries;
using System.Security.Claims;

namespace Ecom.Test.OrderService.Tests
{
    public class GetMyOrdersQueryHandlerTests
    {
        private readonly Fixture fixture = new();
        private readonly Mock<AuthDbContext> db = new();
        private readonly Mock<IHttpContextAccessor> http = new();
        private readonly GetMyOrdersQueryHandler handler;
        private readonly Guid userId;

        public GetMyOrdersQueryHandlerTests()
        {
            fixture.Behaviors.Remove(fixture.Behaviors.OfType<ThrowingRecursionBehavior>().First());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            userId = fixture.Create<Guid>();
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }, "mock"));

            http.Setup(x => x.HttpContext!.User).Returns(user);
            handler = new GetMyOrdersQueryHandler(db.Object, http.Object);
        }

        private void SetupOrders(params Order[] orders)
        {
            foreach (var o in orders) o.UserId = userId;

            var mockSet = orders.BuildMockDbSet();
            db.Setup(d => d.Orders).Returns(mockSet.Object);
        }

        [Fact]
        public async Task Returns_user_orders_with_all_details()
        {
            var orders = fixture.Build<Order>()
                .With(o => o.OrderItems, fixture.CreateMany<OrderItem>(2).ToList())
                .CreateMany(3)
                .ToArray();

            SetupOrders(orders);

            var result = await handler.Handle(new GetMyOrdersQuery(), CancellationToken.None);

            result.Should().HaveCount(3);
            result.Should().OnlyContain(o => o.Items.Count == 2);
        }

        [Fact]
        public async Task Returns_empty_list_when_no_orders()
        {
            SetupOrders();

            var result = await handler.Handle(new GetMyOrdersQuery(), CancellationToken.None);

            result.Should().BeEmpty();
        }
    }
}