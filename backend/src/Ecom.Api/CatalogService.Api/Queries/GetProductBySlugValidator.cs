using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogService.Api.Queries
{
    public class GetProductBySlugValidator : AbstractValidator<GetProductBySlugQuery>
    {
        public GetProductBySlugValidator()
        {
            RuleFor(x => x.Slug).NotEmpty().MaximumLength(200);
        }
    }
}
