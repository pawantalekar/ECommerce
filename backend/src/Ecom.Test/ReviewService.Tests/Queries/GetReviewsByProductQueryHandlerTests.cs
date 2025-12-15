using AutoFixture;
using Ecom.Application.ReviewService.Application.Interfaces;
using Ecom.Domain.Entities;
using Ecom.Infrastructure;
using FluentAssertions;
using Moq;
using ReviewService.Api.Queries;

namespace Ecom.Test.ReviewService.Tests.Queries
{
    public class GetReviewsByProductQueryHandlerTests
    {
        private readonly Fixture fixture = new();
        private readonly Mock<IReviewRepository> repo = new();
        private readonly Mock<AuthDbContext> db = new();
        private readonly GetReviewsByProductQueryHandler handler;

        public GetReviewsByProductQueryHandlerTests()
        {
            fixture.Behaviors.Remove(fixture.Behaviors.OfType<ThrowingRecursionBehavior>().First());
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            handler = new GetReviewsByProductQueryHandler(repo.Object, db.Object);
        }

        [Fact]
        public async Task Returns_only_approved_reviews_with_user_name()
        {
            var productId = fixture.Create<Guid>();
            var user = fixture.Build<User>().With(u => u.Name, "Rahul K").Create();

            var reviews = new List<Review>
            {
                fixture.Build<Review>()
                 .With(r => r.ProductId, productId)
                 .With(r => r.Status, "Approved")
                 .With(r => r.UserId, user.Id)
                 .With(r => r.User, user)
                 .Create(),
                fixture.Build<Review>()
                 .With(r => r.ProductId, productId)
                 .With(r => r.Status, "Pending")
                 .With(r => r.User, (User?)null)
                 .Create()
            };

            repo.Setup(r => r.GetByProductIdAsync(productId, true))
                .ReturnsAsync(reviews.Where(r => r.Status == "Approved").ToList());

            var result = await handler.Handle(new GetReviewsByProductQuery(productId), CancellationToken.None);

            result.Should().HaveCount(1);
            result[0].UserName.Should().Be("Rahul K");
        }

        [Fact]
        public async Task Returns_anonymous_when_user_is_null()
        {
            var productId = fixture.Create<Guid>();
            var review = fixture.Build<Review>()
                .With(r => r.ProductId, productId)
                .With(r => r.Status, "Approved")
                .With(r => r.User, (User?)null)
                .Create();

            repo.Setup(r => r.GetByProductIdAsync(productId, true))
                .ReturnsAsync(new List<Review> { review });

            var result = await handler.Handle(new GetReviewsByProductQuery(productId), CancellationToken.None);

            result.Should().ContainSingle();
            result[0].UserName.Should().Be("Anonymous");
        }

        [Fact]
        public async Task Returns_empty_list_when_no_approved_reviews()
        {
            var productId = fixture.Create<Guid>();

            repo.Setup(r => r.GetByProductIdAsync(productId, true))
                .ReturnsAsync(new List<Review>());

            var result = await handler.Handle(new GetReviewsByProductQuery(productId), CancellationToken.None);

            result.Should().BeEmpty();
        }
    }
}