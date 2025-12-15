using AutoFixture;
using CatalogService.Api.Commands.UpdateProduct;
using Ecom.Application.CatalogService.Application.Interfaces;
using Ecom.Domain.Entities;
using FluentAssertions;
using Moq;

namespace Ecom.Test.CatalogService.Tests.Commands
{
    public class UpdateProductCommandHandlerTests
    {
        private readonly Fixture fixture = new();
        private readonly Mock<ICatalogRepository> repo = new();
        private readonly UpdateProductCommandHandler handler;

        public UpdateProductCommandHandlerTests()
        {
            fixture.Behaviors.Remove(fixture.Behaviors.OfType<ThrowingRecursionBehavior>().First());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            handler = new UpdateProductCommandHandler(repo.Object);
        }

        [Fact]
        public async Task Updates_existing_product_successfully()
        {
            var existingProduct = fixture.Build<Product>()
                .With(p => p.Id, Guid.NewGuid())
                .With(p => p.ProductImages, new List<ProductImage>())
                .With(p => p.ProductTags, new List<ProductTag>())
                .Create();

            var command = fixture.Build<UpdateProductCommand>()
                .With(c => c.Id, existingProduct.Id)
                .Create();

            repo.Setup(r => r.GetByIdAsync(existingProduct.Id, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(existingProduct);
            repo.Setup(r => r.UpdateProductAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);

            var result = await handler.Handle(command, CancellationToken.None);

            result.Id.Should().Be(existingProduct.Id);
            repo.Verify(r => r.UpdateProductAsync(existingProduct, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Throws_when_product_not_found()
        {
            var command = fixture.Create<UpdateProductCommand>();

            repo.Setup(r => r.GetByIdAsync(command.Id, It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Product?)null);

            await handler.Invoking(h => h.Handle(command, CancellationToken.None))
                          .Should().ThrowAsync<ArgumentException>()
                          .WithMessage("Product not found");
        }

        [Fact]
        public async Task Replaces_images_when_new_urls_provided()
        {
            var product = fixture.Build<Product>()
                .With(p => p.ProductImages, fixture.CreateMany<ProductImage>(5).ToList())
                .Create();

            var command = fixture.Build<UpdateProductCommand>()
                .With(c => c.Id, product.Id)
                .With(c => c.ImageUrls, fixture.CreateMany<string>(2).ToList())
                .Create();

            repo.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>())).ReturnsAsync(product);
            repo.Setup(r => r.UpdateProductAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            await handler.Handle(command, CancellationToken.None);

            product.ProductImages.Should().HaveCount(2);
            product.ProductImages.First().IsThumbnail.Should().BeTrue();
        }

        [Fact]
        public async Task Clears_images_when_urls_are_null()
        {
            var product = fixture.Build<Product>()
                .With(p => p.ProductImages, fixture.CreateMany<ProductImage>(10).ToList())
                .Create();

            var command = fixture.Build<UpdateProductCommand>()
                .With(c => c.Id, product.Id)
                .With(c => c.ImageUrls, (List<string>?)null)
                .Create();

            repo.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>())).ReturnsAsync(product);
            repo.Setup(r => r.UpdateProductAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            await handler.Handle(command, CancellationToken.None);

            product.ProductImages.Should().BeEmpty();
        }

        [Fact]
        public async Task Creates_new_category_when_id_null_and_name_provided()
        {
            var product = fixture.Create<Product>();
            var command = fixture.Build<UpdateProductCommand>()
                .With(c => c.Id, product.Id)
                .With(c => c.CategoryId, (Guid?)null)
                .With(c => c.CategoryName, fixture.Create<string>())
                .Create();

            repo.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>())).ReturnsAsync(product);
            repo.Setup(r => r.UpdateProductAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            await handler.Handle(command, CancellationToken.None);

            product.Category.Should().NotBeNull();
            product.Category!.Name.Should().Be(command.CategoryName);
        }

        [Fact]
        public async Task Uses_existing_category_when_id_provided()
        {
            var product = fixture.Build<Product>()
                .With(p => p.Category, null as Category)
                .Create();
            var categoryId = fixture.Create<Guid>();
            var command = fixture.Build<UpdateProductCommand>()
                .With(c => c.Id, product.Id)
                .With(c => c.CategoryId, categoryId)
                .Create();

            repo.Setup(r => r.GetByIdAsync(product.Id, It.IsAny<CancellationToken>())).ReturnsAsync(product);
            repo.Setup(r => r.UpdateProductAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            await handler.Handle(command, CancellationToken.None);

            product.CategoryId.Should().Be(categoryId);
            product.Category.Should().BeNull();
        }
    }
}