using AutoFixture;
using CartService.Api.Commands.RemoveFromCart;
using Ecom.Application.CartService.Application.Interfaces;
using Ecom.Domain.Entities;
using Ecom.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Ecom.Test.CartService.Tests.Commands
{
    public class RemoveFromCartCommandHandlerTests
    {
        private readonly Fixture fixture = new();
        private readonly Mock<ICartRepository> cartRepo = new();
        private readonly Mock<AuthDbContext> db = new();
        private readonly Mock<IHttpContextAccessor> http = new();
        private readonly RemoveFromCartCommandHandler handler;
        private readonly Guid userId;

        public RemoveFromCartCommandHandlerTests()
        {
            fixture.Behaviors.Remove(fixture.Behaviors.OfType<ThrowingRecursionBehavior>().First());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            userId = fixture.Create<Guid>();
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }, "mock"));

            http.Setup(x => x.HttpContext!.User).Returns(user);
            handler = new RemoveFromCartCommandHandler(cartRepo.Object, db.Object, http.Object);
        }

        private (Cart cart, Product product) SetupCartWithItem(Guid productId)
        {
            var product = fixture.Build<Product>()
                .With(p => p.Id, productId)
                .With(p => p.ProductImages, new[] { new ProductImage { IsThumbnail = true, Url = "thumb.jpg" } })
                .Create();

            var cart = fixture.Build<Cart>()
                .With(c => c.UserId, userId)
                .With(c => c.Items, new List<CartItem>
                {
                    new CartItem { ProductId = productId, Quantity = 2 }
                })
                .Create();

            cartRepo.Setup(r => r.GetByUserIdWithItemsAsync(userId))
                    .ReturnsAsync(cart);

           
            var productData = new[] { product }.AsQueryable();
            var mockSet = new Mock<DbSet<Product>>();
            mockSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(productData.Provider);
            mockSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(productData.Expression);
            mockSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(productData.ElementType);
            mockSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(productData.GetEnumerator());

            db.Setup(d => d.Products).Returns(mockSet.Object);

            cartRepo.Setup(r => r.UpdateAsync(cart)).Returns(Task.CompletedTask);

            return (cart, product);
        }

        [Fact]
        public async Task Removes_item_from_cart_successfully()
        {
            var productId = fixture.Create<Guid>();
            var (cart, _) = SetupCartWithItem(productId);

            var command = new RemoveFromCartCommand { ProductId = productId };

            var result = await handler.Handle(command, CancellationToken.None);

            cart.Items.Should().BeEmpty();
            result.Cart.ItemsCount.Should().Be(0);
            result.Cart.Total.Should().Be(0);
        }

        [Fact]
        public async Task Throws_when_item_not_in_cart()
        {
            var cart = fixture.Build<Cart>()
                .With(c => c.UserId, userId)
                .With(c => c.Items, new List<CartItem>())
                .Create();

            cartRepo.Setup(r => r.GetByUserIdWithItemsAsync(userId))
                    .ReturnsAsync(cart);

            var command = new RemoveFromCartCommand { ProductId = fixture.Create<Guid>() };

            await handler.Invoking(h => h.Handle(command, CancellationToken.None))
                         .Should().ThrowAsync<Exception>()
                         .WithMessage("Item not found in cart");
        }
    }
}