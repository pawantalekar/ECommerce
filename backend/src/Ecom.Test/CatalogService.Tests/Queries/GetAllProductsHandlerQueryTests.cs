using AutoFixture;
using CatalogService.Api.Queries;
using Ecom.Application.CatalogService.Application.Interfaces;
using Ecom.Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace Ecom.Test.CatalogService.Tests.Queries
{
    public class GetAllProductsHandlerQueryTests
    {
        private readonly Fixture fixture = new();
        private readonly Mock<ICatalogRepository> repo = new();
        private readonly GetAllProductsHandlerQuery handler;

        public GetAllProductsHandlerQueryTests()
        {
            fixture.Behaviors.Remove(fixture.Behaviors.OfType<ThrowingRecursionBehavior>().First());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            handler = new GetAllProductsHandlerQuery(repo.Object);
        }

        [Fact]
        public async Task Returns_all_products_with_full_details()
        {
            var dbProducts = fixture.Build<Product>()
                .With(p => p.Category, fixture.Create<Category>())
                .With(p => p.ProductImages, fixture.CreateMany<ProductImage>(2).ToList())
                .With(p => p.ProductTags, fixture.CreateMany<ProductTag>(3).ToList())
                .CreateMany(5)
                .ToList();

            repo.Setup(r => r.GetAllProductsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(dbProducts);

            var result = await handler.Handle(new GetAllProductsQuery(), CancellationToken.None);

            result.Should().HaveCount(5);
            result[0].Name.Should().Be(dbProducts[0].Name);
            result[0].CategoryName.Should().Be(dbProducts[0].Category.Name);
            result[0].ImageUrls.Should().HaveCount(2);
            result[0].Tags.Should().HaveCount(3);
        }

        [Fact]
        public async Task Returns_empty_list_when_no_products()
        {
            repo.Setup(r => r.GetAllProductsAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product>());

            var result = await handler.Handle(new GetAllProductsQuery(), CancellationToken.None);

            result.Should().BeEmpty();
        }
    }
}