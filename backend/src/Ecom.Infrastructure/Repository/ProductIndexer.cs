//using Microsoft.EntityFrameworkCore;
//using Typesense;

//namespace Ecom.Infrastructure.Repository
//{
//    public class ProductIndexer
//    {
//        private readonly AuthDbContext _db;
//        private readonly TypesenseService _typesense;

//        public ProductIndexer(AuthDbContext db, TypesenseService typesense)
//        {
//            _db = db;
//            _typesense = typesense;
//        }

//        public async Task IndexAllProductsAsync()
//        {
//            var client = _typesense.Client;

//            // 1. Create collection (ignore if exists)
//            var schema = new Schema
//            {
//                Name = "products",
//                Fields = new List<Field>
//                {
//                    new() { Name = "id", Type = "string" },
//                    new() { Name = "name", Type = "string" },
//                    new() { Name = "slug", Type = "string" },
//                    new() { Name = "brandName", Type = "string", Optional = true },
//                    new() { Name = "categoryName", Type = "string", Optional = true },
//                    new() { Name = "tags", Type = "string[]", Optional = true },
//                    new() { Name = "price", Type = "float" },
//                    new() { Name = "description", Type = "string", Optional = true, Infix = true },
//                    new() { Name = "imageUrl", Type = "string", Optional = true },
//                    new() { Name = "stockQuantity", Type = "int32" }
//                },
//                DefaultSortingField = "price"
//            };

//            try { await client.CreateCollectionAsync(schema); }
//            catch (TypesenseApiException ex) when (ex.StatusCode == 409) { }

//            // 2. Delete all old documents
//            try { await client.DeleteDocumentsAsync("products", new DeleteQuery { FilterBy = "*:*" }); }
//            catch { }

//            // 3. Load products with all navigation properties
//            var products = await _db.Products
//                .Include(p => p.Brand)
//                .Include(p => p.Category)
//                .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
//                .Include(p => p.ProductImages)
//                .Select(p => new
//                {
//                    id = p.Id.ToString(),
//                    name = p.Name ?? "",
//                    slug = p.Slug ?? "",
//                    brandName = p.Brand != null ? p.Brand.Name : "",
//                    categoryName = p.Category != null ? p.Category.Name : "",
//                    tags = p.ProductTags.Select(pt => pt.Tag.Name).ToArray(),
//                    price = (float)p.Price,
//                    description = p.Description ?? "",
//                    imageUrl = p.ProductImages.OrderBy(i => i.SortOrder).FirstOrDefault()?.Url ?? "",
//                    stockQuantity = p.StockQuantity
//                })
//                .ToListAsync();

//            if (products.Count == 0)
//            {
//                Console.WriteLine("Typesense: No products to index");
//                return;
//            }

//            // 4. Import in batches with upsert (safe even if duplicates)
//            const int batchSize = 100;
//            for (int i = 0; i < products.Count; i += batchSize)
//            {
//                var batch = products.Skip(i).Take(batchSize).ToList();
//                await client.ImportDocumentsAsync("products", batch, batchSize, ImportType.Upsert);
//            }

//            Console.WriteLine($"Typesense: Successfully indexed {products.Count} products!");
//        }
//    }
//}