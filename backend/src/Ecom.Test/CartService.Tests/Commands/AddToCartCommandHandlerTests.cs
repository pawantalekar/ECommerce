using AutoFixture;
using CartService.Api.Commands.AddToCart;
using Ecom.Application.CartService.Application.Interfaces;
using Ecom.Application.CatalogService.Application.Interfaces;
using Ecom.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Ecom.Test.CartService.Tests.Commands
{
    public class AddToCartCommandHandlerTests
    {
        private readonly Fixture fixture = new();
        private readonly Mock<ICartRepository> cartRepo = new();
        private readonly Mock<ICatalogRepository> catalogRepo = new();
        private readonly Mock<IHttpContextAccessor> http = new();
        private readonly AddToCartCommandHandler handler;

        private readonly Guid userId;

        public AddToCartCommandHandlerTests()
        {
            fixture.Behaviors.Remove(fixture.Behaviors.OfType<ThrowingRecursionBehavior>().First());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            userId = fixture.Create<Guid>();
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }, "mock"));

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(x => x.User).Returns(user);
            http.Setup(x => x.HttpContext).Returns(httpContext.Object);

            handler = new AddToCartCommandHandler(cartRepo.Object, catalogRepo.Object, http.Object);
        }

        private Cart SetupUserCart(List<CartItem>? items = null)
        {
            var cart = fixture.Build<Cart>()
                .With(c => c.UserId, userId)
                .With(c => c.Items, items ?? new List<CartItem>())
                .Create();

            cartRepo.Setup(r => r.GetByUserIdWithItemsAsync(userId))
                    .ReturnsAsync(cart);

            return cart;
        }

        private Product SetupProduct(Guid? id = null, int stock = 100)
        {
            var product = fixture.Build<Product>()
                .With(p => p.Id, id ?? fixture.Create<Guid>())
                .With(p => p.StockQuantity, stock)
                .With(p => p.ProductImages, new[] { new ProductImage { IsThumbnail = true, Url = "thumb.jpg" } })
                .Create();

            catalogRepo.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>()))
                       .ReturnsAsync(product);

            return product;
        }

        [Fact]
        public async Task Adds_new_item_when_product_not_in_cart()
        {
            var product = SetupProduct();
            SetupUserCart();

            cartRepo.Setup(r => r.UpdateAsync(It.IsAny<Cart>())).Returns(Task.CompletedTask);

            var result = await handler.Handle(new AddToCartCommand { ProductId = product.Id, Quantity = 2 }, CancellationToken.None);

            result.Cart.Items.Should().ContainSingle();
            result.Cart.Items[0].Quantity.Should().Be(2);
            result.Cart.Total.Should().Be(product.Price * 2);
        }

        [Fact]
        public async Task Increases_quantity_when_product_already_in_cart()
        {
            var product = SetupProduct();
            var existingItem = new CartItem { ProductId = product.Id, Quantity = 3 };
            SetupUserCart(new List<CartItem> { existingItem });

            await handler.Handle(new AddToCartCommand { ProductId = product.Id, Quantity = 4 }, CancellationToken.None);

            existingItem.Quantity.Should().Be(7);
        }

        [Fact]
        public async Task Throws_when_product_not_found()
        {
            SetupUserCart();
            catalogRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                       .ReturnsAsync((Product?)null);

            var command = new AddToCartCommand { ProductId = Guid.NewGuid(), Quantity = 1 };

            await handler.Invoking(h => h.Handle(command, CancellationToken.None))
                         .Should().ThrowAsync<Exception>()
                         .WithMessage("Product not found or inactive");
        }
    }
}