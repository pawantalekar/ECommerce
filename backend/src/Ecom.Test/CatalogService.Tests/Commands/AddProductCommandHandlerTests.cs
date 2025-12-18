using AutoFixture;
using CatalogService.Api.Commands.AddProduct;
using Ecom.Application.CatalogService.Application.Interfaces;
using Ecom.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Ecom.Test.CatalogService.Tests.Commands
{
    public class AddProductCommandHandlerTests
    {
        private readonly Fixture fixture = new();
        private readonly Mock<ICatalogRepository> repo = new();
        private readonly Mock<IHttpContextAccessor> httpContextAccessor = new();
        private readonly AddProductCommandHandler handler;

        public AddProductCommandHandlerTests()
        {
            fixture.Behaviors.Remove(fixture.Behaviors.OfType<ThrowingRecursionBehavior>().First());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            repo.Setup(r => r.AddProductAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            handler = new AddProductCommandHandler(repo.Object, httpContextAccessor.Object);
        }

        [Fact]
        public async Task Creates_new_category_when_id_is_null()
        {
            var command = fixture.Build<AddProductCommand>()
                .With(x => x.CategoryId, (Guid?)null)
                .Create();

            await handler.Handle(command, CancellationToken.None);

            repo.Verify(r => r.AddProductAsync(
                It.Is<Product>(p => p.Category != null),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Uses_existing_category_id_when_provided()
        {
            var command = fixture.Create<AddProductCommand>();

            await handler.Handle(command, CancellationToken.None);

            repo.Verify(r => r.AddProductAsync(
                It.Is<Product>(p => p.CategoryId == command.CategoryId && p.BrandId == command.BrandId),
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
        public async Task Generates_guid_slug_when_name_is_null()
        {
            var command = fixture.Build<AddProductCommand>()
                .With(x => x.Name, (string?)null)
                .Create();

            var result = await handler.Handle(command, CancellationToken.None);

            Guid.TryParse(result.Slug, out _).Should().BeTrue();
        }
    }
}