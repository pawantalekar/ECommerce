using AutoFixture;
using CatalogService.Api.Queries;
using Ecom.Application.CatalogService.Application.Interfaces;
using Ecom.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace Ecom.Test.CatalogService.Tests.Queries
{
    public class GetProductBySlugHandlerTests
    {
        private readonly Fixture fixture = new();
        private readonly Mock<ICatalogRepository> repo = new();
        private readonly GetProductBySlugHandler handler;

        public GetProductBySlugHandlerTests()
        {
            fixture.Behaviors.Remove(fixture.Behaviors.OfType<ThrowingRecursionBehavior>().First());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            handler = new GetProductBySlugHandler(repo.Object);
        }

        [Fact]
        public async Task Returns_product_when_slug_exists()
        {
            var product = fixture.Build<Product>()
                .With(p => p.Slug, "samsung-galaxy-s24")
                .With(p => p.Category, fixture.Create<Category>())
                .With(p => p.ProductImages, fixture.CreateMany<ProductImage>(3).ToList())
                .With(p => p.ProductTags, fixture.CreateMany<ProductTag>(2).ToList())
                .Create();

            repo.Setup(r => r.GetBySlugAsync("samsung-galaxy-s24", It.IsAny<CancellationToken>()))
                 .ReturnsAsync(product);

            var result = await handler.Handle(new GetProductBySlugQuery("samsung-galaxy-s24"), CancellationToken.None);

            result.Should().NotBeNull();
            result!.Id.Should().Be(product.Id);
            result.Name.Should().Be(product.Name);
            result.CategoryName.Should().Be(product.Category.Name);
            result.ImageUrls.Should().HaveCount(3);
            result.Tags.Should().HaveCount(2);
        }

        [Fact]
        public async Task Returns_null_when_slug_not_found()
        {
            repo.Setup(r => r.GetBySlugAsync("non-existing-slug", It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Product?)null);

            var result = await handler.Handle(new GetProductBySlugQuery("non-existing-slug"), CancellationToken.None);

            result.Should().BeNull();
        }
    }
}