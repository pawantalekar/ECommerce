using AutoFixture;
using CartService.Api.Commands.UpdateCart;
using Ecom.Application.CartService.Application.Interfaces;
using Ecom.Domain.Entities;
using Ecom.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System.Linq.Expressions;
using System.Security.Claims;
using Xunit;

namespace Ecom.Test.CartService.Tests.Commands
{
    internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;
        public TestAsyncQueryProvider(IQueryProvider inner) { _inner = inner; }
        public IQueryable CreateQuery(Expression expression) => new TestAsyncEnumerable<TEntity>(expression);
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new TestAsyncEnumerable<TElement>(expression);
        public object Execute(Expression expression) => _inner.Execute(expression);
        public TResult Execute<TResult>(Expression expression) => _inner.Execute<TResult>(expression);
        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var result = Execute(expression);
            var taskType = typeof(TResult);
            if (taskType.IsGenericType && taskType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var resultType = taskType.GetGenericArguments()[0];
                return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))!
                    .MakeGenericMethod(resultType)
                    .Invoke(null, new[] { result })!;
            }
            return (TResult)(object)Task.FromResult(result);
        }
    }

    internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
        public TestAsyncEnumerable(Expression expression) : base(expression) { }
        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
    }

    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;
        public TestAsyncEnumerator(IEnumerator<T> inner) { _inner = inner; }
        public T Current => _inner.Current;
        public ValueTask DisposeAsync() { _inner.Dispose(); return ValueTask.CompletedTask; }
        public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(_inner.MoveNext());
    }

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

            // Manually mock DbSet<Product> with async support
            var products = new List<Product> { product }.AsQueryable();
            var mockSet = new Mock<DbSet<Product>>();
            mockSet.As<IAsyncEnumerable<Product>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<Product>(products.GetEnumerator()));
            mockSet.As<IQueryable<Product>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Product>(products.Provider));
            mockSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.Expression);
            mockSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.ElementType);
            mockSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.GetEnumerator());
            db.Setup(d => d.Products).Returns(mockSet.Object);

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
            // Mock empty DbSet<Product> with async support
            var emptyProducts = new List<Product>().AsQueryable();
            var mockSet = new Mock<DbSet<Product>>();
            mockSet.As<IAsyncEnumerable<Product>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<Product>(emptyProducts.GetEnumerator()));
            mockSet.As<IQueryable<Product>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<Product>(emptyProducts.Provider));
            mockSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(emptyProducts.Expression);
            mockSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(emptyProducts.ElementType);
            mockSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(emptyProducts.GetEnumerator());
            db.Setup(d => d.Products).Returns(mockSet.Object);

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