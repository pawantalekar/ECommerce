using AutoFixture;
using CartService.Api.Commands.UpdateCart;
using Ecom.Application.CartService.Application.Interfaces;
using Ecom.Domain.Entities;
using Ecom.Infrastructure;
using Ecom.Test.ReviewService.Tests.Commands;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using MockQueryable.Moq;
using Moq;
using System.Security.Claims;
using Xunit;


namespace Ecom.Test.CartService.Tests.Commands
{
    public class UpdateCartItemCommandHandlerTests
    {
        private readonly Fixture fixture = new();
        private readonly Mock<ICartRepository> cartRepo = new();
        private readonly Mock<AuthDbContext> db = new();
        private readonly Mock<IHttpContextAccessor> http = new();
        private readonly UpdateCartItemCommandHandler handler;
        private readonly Guid userId;

        public UpdateCartItemCommandHandlerTests()
        {
            fixture.Behaviors.Remove(fixture.Behaviors.OfType<ThrowingRecursionBehavior>().First());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            userId = fixture.Create<Guid>();
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }, "mock"));

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.SetupGet(c => c.User).Returns(user);
            http.SetupGet(x => x.HttpContext).Returns(mockHttpContext.Object);

            handler = new UpdateCartItemCommandHandler(cartRepo.Object, db.Object, http.Object);
        }

        private (Cart cart, Product product) SetupCartWithItem(Guid productId, int quantity = 1, int stock = 100)
        {
            var product = fixture.Build<Product>()
                .With(p => p.Id, productId)
                .With(p => p.IsActive, true)
                .With(p => p.StockQuantity, stock)
                .With(p => p.ProductImages, new[] { new ProductImage { IsThumbnail = true, Url = "thumb.jpg" } })
                .Create();

            var cart = fixture.Build<Cart>()
                .With(c => c.UserId, userId)
                .With(c => c.Items, new List<CartItem>
                {
                    new CartItem { ProductId = productId, Quantity = quantity }
                })
                .Create();

            cartRepo.Setup(r => r.GetByUserIdWithItemsAsync(userId))
                    .ReturnsAsync(cart);

            var products = new List<Product> { product };
            var productsDbSet = products.BuildMockDbSet();
            db.Setup(d => d.Products).Returns(productsDbSet.Object);

            cartRepo.Setup(r => r.UpdateAsync(It.IsAny<Cart>())).Returns(Task.CompletedTask);

            return (cart, product);
        }

        [Fact]
        public async Task Updates_quantity_successfully()
        {
            var productId = fixture.Create<Guid>();
            var (cart, product) = SetupCartWithItem(productId, quantity: 2);

            var command = new UpdateCartItemCommand { ProductId = productId, Quantity = 5 };

            var result = await handler.Handle(command, CancellationToken.None);

            cart.Items.First().Quantity.Should().Be(5);
            result.Cart.Total.Should().Be(product.Price * 5);
            result.Cart.Items.Should().ContainSingle(i => i.Quantity == 5);
        }

        [Fact]
        public async Task Throws_when_item_not_in_cart()
        {
            var cart = fixture.Build<Cart>()
                .With(c => c.UserId, userId)
                .With(c => c.Items, new List<CartItem>())
                .Create();

            cartRepo.Setup(r => r.GetByUserIdWithItemsAsync(userId)).ReturnsAsync(cart);

            var command = new UpdateCartItemCommand { ProductId = fixture.Create<Guid>(), Quantity = 1 };

            await handler.Invoking(h => h.Handle(command, CancellationToken.None))
                         .Should().ThrowAsync<Exception>()
                         .WithMessage("Item not found in cart");
        }

        [Fact]
        public async Task Throws_when_product_not_found_or_inactive()
        {
            var productId = fixture.Create<Guid>();
            var cart = fixture.Build<Cart>()
                .With(c => c.UserId, userId)
                .With(c => c.Items, new List<CartItem> { new() { ProductId = productId } })
                .Create();

            cartRepo.Setup(r => r.GetByUserIdWithItemsAsync(userId)).ReturnsAsync(cart);
            db.Setup(d => d.Products).Returns(new List<Product>().BuildMockDbSet().Object);

            var command = new UpdateCartItemCommand { ProductId = productId, Quantity = 1 };

            await handler.Invoking(h => h.Handle(command, CancellationToken.None))
                         .Should().ThrowAsync<Exception>()
                         .WithMessage("Product not found or inactive");
        }

        [Fact]
        public async Task Throws_when_insufficient_stock()
        {
            var productId = fixture.Create<Guid>();
            var (cart, _) = SetupCartWithItem(productId, quantity: 1, stock: 3);

            var command = new UpdateCartItemCommand { ProductId = productId, Quantity = 10 };

            await handler.Invoking(h => h.Handle(command, CancellationToken.None))
                         .Should().ThrowAsync<Exception>()
                         .WithMessage("Insufficient stock");
        }
    }
}