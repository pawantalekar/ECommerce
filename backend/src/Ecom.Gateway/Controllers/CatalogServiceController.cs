using CatalogService.Api.Commands.AddProduct;
using CatalogService.Api.Commands.UpdateProduct;
using CatalogService.Api.Queries;
using Ecom.Infrastructure.Repository;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Typesense;

namespace Ecom.Gateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogServiceController : ControllerBase
    {
        private readonly IMediator _mediator;

        private readonly IWebHostEnvironment _env;
        private readonly TypesenseService _typesenseService;


        public CatalogServiceController( IMediator mediator, IWebHostEnvironment env, TypesenseService _typesenseService)
        {
            _mediator = mediator;
            _env = env;
            this._typesenseService = _typesenseService;
        }

        [HttpPost("products")]
        public async Task<IActionResult> AddProduct([FromBody] AddProductCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return CreatedAtAction(
                    nameof(GetProductBySlug),
                    new { slug = result.Slug },
                    result
                );
            }
            catch (ArgumentException ex) when (ex.Message?.StartsWith("Category") == true)
            {
                
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("products/{slug}")]
        public async Task<IActionResult> GetProductBySlug(string slug)
        {
            var result = await _mediator.Send(new GetProductBySlugQuery(slug));
            return result is not null ? Ok(result) : NotFound();
        }
        [HttpGet("products")]
        public async Task<IActionResult> GetAllProducts()
        {
            var result = await _mediator.Send(new GetAllProductsQuery());
            return Ok(result);
        }

        [HttpPut("products/{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductCommand command)
        {
            if (id != command.Id)
                return BadRequest(new { error = "Id mismatch" });

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        //for the images specailly to select and save from the local storage maybe for temporarily if not works
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "No file uploaded" });

            if (file.Length > 5 * 1024 * 1024) 
                return BadRequest(new { error = "File too large. Max 5 MB." });

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType))
                return BadRequest(new { error = "Only JPG, PNG, WebP allowed" });

            var uploadFolder = Path.Combine(_env.WebRootPath, "uploads", "products");
            Directory.CreateDirectory(uploadFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadFolder, fileName);

            await using var stream = System.IO.File.Create(filePath);
            await file.CopyToAsync(stream);

            var url = $"{Request.Scheme}://{Request.Host}/uploads/products/{fileName}";
            return Ok(new { url });
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProducts([FromQuery] string q, [FromQuery] int page = 1)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
                return Ok(new { hits = new List<object>(), found = 0 });

            var result = await _typesenseService.Client.Search<object>("products", new SearchParameters
            {
                Text = q,                                           
                QueryBy = "name,brandName,categoryName,tags,description",
                PerPage = 20,
                Page = page,
                SortBy = "stockQuantity:desc"
            });

            return Ok(new
            {
                hits = result.Hits.Select(h => h.Document),
                found = result.Found,
                page,
                totalPages = (result.Found + 19) / 20
            });
        }


    }
}
