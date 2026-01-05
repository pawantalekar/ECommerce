using AutoFixture;
using CatalogService.Api.Commands.AddProduct;
using Ecom.Application.CatalogService.Application.Interfaces;
using Ecom.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Ecom.Test.CatalogService.Tests.Commands
{
    public class AddProductCommandHandlerTests
    {
        private readonly Fixture fixture = new();
        private readonly Mock<ICatalogRepository> repo = new();
        private readonly Mock<IHttpContextAccessor> httpContextAccessor = new();
        private readonly AddProductCommandHandler handler;
        private readonly Guid userId;

        public AddProductCommandHandlerTests()
        {
            fixture.Behaviors.Remove(fixture.Behaviors.OfType<ThrowingRecursionBehavior>().First());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            userId = Guid.NewGuid();
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                new[] { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) }, "mock"));

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.SetupGet(c => c.User).Returns(user);
            httpContextAccessor.SetupGet(x => x.HttpContext).Returns(mockHttpContext.Object);

            repo.Setup(r => r.AddProductAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            handler = new AddProductCommandHandler(repo.Object, httpContextAccessor.Object);
        }

        [Fact]
        public async Task Creates_new_category_when_category_name_provided()
        {
            var command = fixture.Build<AddProductCommand>()
                .With(x => x.CategoryId, (Guid?)null)
                .With(x => x.CategoryName, "Electronics")
                .With(x => x.CategorySlug, "electronics")
                .Create();

            await handler.Handle(command, CancellationToken.None);

            repo.Verify(r => r.AddProductAsync(
                It.Is<Product>(p => p.Category != null && p.Category.Name == "Electronics"),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Creates_new_brand_when_brand_name_provided()
        {
            var command = fixture.Build<AddProductCommand>()
                .With(x => x.BrandId, (Guid?)null)
                .With(x => x.BrandName, "TestBrand")
                .Create();

            await handler.Handle(command, CancellationToken.None);

            repo.Verify(r => r.AddProductAsync(
                It.Is<Product>(p => p.Brand != null && p.Brand.Name == "TestBrand"),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Uses_existing_category_id_when_provided()
        {
            var categoryId = Guid.NewGuid();
            var brandId = Guid.NewGuid();
            var command = fixture.Build<AddProductCommand>()
                .With(x => x.CategoryId, categoryId)
                .With(x => x.BrandId, brandId)
                .Create();

            await handler.Handle(command, CancellationToken.None);

            repo.Verify(r => r.AddProductAsync(
                It.Is<Product>(p => p.CategoryId == categoryId && p.BrandId == brandId && p.Category == null),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Creates_product_images_with_correct_sort_order()
        {
            var imageUrls = new List<string> { "url1.jpg", "url2.jpg", "url3.jpg" };
            var command = fixture.Build<AddProductCommand>()
                .With(x => x.ImageUrls, imageUrls)
                .With(x => x.Name, "TestProduct")
                .Create();

            await handler.Handle(command, CancellationToken.None);

            repo.Verify(r => r.AddProductAsync(
                It.Is<Product>(p => 
                    p.ProductImages.Count == 3 && 
                    p.ProductImages.Any(img => img.IsThumbnail == true && img.SortOrder == 0) &&
                    p.ProductImages.Any(img => img.IsThumbnail != true && img.SortOrder == 1)),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Creates_empty_image_list_when_urls_are_null()
        {
            var command = fixture.Build<AddProductCommand>()
                .With(x => x.ImageUrls, (List<string>?)null)
                .Create();

            await handler.Handle(command, CancellationToken.None);

            repo.Verify(r => r.AddProductAsync(
                It.Is<Product>(p => p.ProductImages.Count == 0),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Generates_slug_from_name_when_name_provided()
        {
            var command = fixture.Build<AddProductCommand>()
                .With(x => x.Name, "Test Product Name")
                .Create();

            var result = await handler.Handle(command, CancellationToken.None);

            result.Slug.Should().Be("test-product-name");
        }

        [Fact]
        public async Task Generates_guid_slug_when_name_is_null()
        {
            var command = fixture.Build<AddProductCommand>()
                .With(x => x.Name, (string?)null)
                .Create();

            var result = await handler.Handle(command, CancellationToken.None);

            Guid.TryParse(result.Slug, out _).Should().BeTrue();
        }

        [Fact]
        public async Task Creates_product_tags_from_tag_list()
        {
            var tags = new List<string> { "electronics", "sale", "new" };
            var command = fixture.Build<AddProductCommand>()
                .With(x => x.Tags, tags)
                .Create();

            await handler.Handle(command, CancellationToken.None);

            repo.Verify(r => r.AddProductAsync(
                It.Is<Product>(p => p.ProductTags.Count == 3),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Sets_seller_id_from_authenticated_user()
        {
            var command = fixture.Create<AddProductCommand>();

            await handler.Handle(command, CancellationToken.None);

            repo.Verify(r => r.AddProductAsync(
                It.Is<Product>(p => p.SellerId == userId),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Throws_when_user_not_authenticated()
        {
            httpContextAccessor.SetupGet(x => x.HttpContext).Returns((HttpContext?)null);
            var handler2 = new AddProductCommandHandler(repo.Object, httpContextAccessor.Object);
            var command = fixture.Create<AddProductCommand>();

            await handler2.Invoking(h => h.Handle(command, CancellationToken.None))
                .Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("User not authenticated");
        }
    }
}