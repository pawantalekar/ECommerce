using Ecom.Application.CatalogService.Application.Interfaces;
using Ecom.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Repository
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly AuthDbContext _db;
        public CatalogRepository(AuthDbContext db) => _db = db;

        public async Task AddProductAsync(Product p, CancellationToken ct)
        {
            if (p.Id == Guid.Empty)
                p.Id = Guid.NewGuid();

            if (p.Category != null)
            {
                if (p.Category.Id == Guid.Empty)
                    p.Category.Id = Guid.NewGuid();

                var existingByName = await _db.Categories
                    .FirstOrDefaultAsync(c => c.Name == p.Category.Name, ct);

                if (existingByName != null)
                {
                    p.CategoryId = existingByName.Id;
                    p.Category = existingByName;
                }
                else
                {
                    _db.Categories.Add(p.Category);
                    p.CategoryId = p.Category.Id;
                }
            }
            else
            {
                if (p.CategoryId == Guid.Empty || await _db.Categories.FindAsync(new object[] { p.CategoryId }, ct) == null)
                {
                    var category = new Category
                    {
                        Id = Guid.NewGuid(),
                        Name = "Default Category",
                        Slug = "default-category",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    _db.Categories.Add(category);
                    p.CategoryId = category.Id;
                    p.Category = category;
                }
                else
                {
                    p.Category = await _db.Categories.FindAsync(new object[] { p.CategoryId }, ct)!;
                }
            }

            if (p.Brand != null)
            {
                if (p.Brand.Id == Guid.Empty)
                    p.Brand.Id = Guid.NewGuid();

                var existingBrand = await _db.Brands.FirstOrDefaultAsync(b => b.Name == p.Brand.Name, ct);
                if (existingBrand != null)
                {
                    if (string.IsNullOrWhiteSpace(existingBrand.LogoUrl) && !string.IsNullOrWhiteSpace(p.Brand.LogoUrl))
                    {
                        existingBrand.LogoUrl = p.Brand.LogoUrl;
                        _db.Brands.Update(existingBrand);
                    }

                    p.BrandId = existingBrand.Id;
                    p.Brand = existingBrand;
                }
                else
                {
                    _db.Brands.Add(p.Brand);
                    p.BrandId = p.Brand.Id;
                }
            }
            else if (p.BrandId.HasValue)
            {
                var b = await _db.Brands.FindAsync(new object[] { p.BrandId.Value }, ct);
                if (b == null)
                {
                    var brand = new Brand
                    {
                        Id = p.BrandId.Value,
                        Name = "Default Brand",
                        Slug = "default-brand"
                    };
                    _db.Brands.Add(brand);
                    p.Brand = brand;
                }
                else
                {
                    p.Brand = b;
                }
            }

            // 
            var incomingTagNames = p.ProductTags
                .Where(pt => pt.Tag != null && !string.IsNullOrWhiteSpace(pt.Tag.Name))
                .Select(pt => pt.Tag!.Name.Trim())
                .ToList();

            var normalizedIncoming = incomingTagNames
                .Select(n => n.ToLowerInvariant())
                .Distinct()
                .ToList();

            var existingTags = await _db.Tags
                .Where(t => normalizedIncoming.Contains(t.Name.ToLower()))
                .ToListAsync(ct);

            var existingTagDict = existingTags
                .ToDictionary(t => t.Name.ToLowerInvariant(), t => t);

            foreach (var name in normalizedIncoming)
            {
                if (!existingTagDict.ContainsKey(name))
                {
                    var newTag = new Tag { Id = Guid.NewGuid(), Name = name };
                    _db.Tags.Add(newTag);
                    existingTagDict[name] = newTag;
                }
            }

            var resolvedTags = normalizedIncoming
                .Select(n => new ProductTag
                {
                    Product = p,
                    Tag = existingTagDict[n]
                })
                .ToList();

            p.ProductTags = resolvedTags;

            foreach (var img in p.ProductImages)
            {
                if (img.Id == Guid.Empty) img.Id = Guid.NewGuid();
                img.Product = p;
            }

            await _db.Products.AddAsync(p, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task<Product?> GetBySlugAsync(string slug, CancellationToken ct)
         => await _db.Products
             .Include(p => p.Category)
             .Include(p => p.ProductImages)
             .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
             .Include(p => p.Brand)
             .FirstOrDefaultAsync(p => p.Slug == slug, ct);
    
    public async Task<List<Product>> GetAllProductsAsync(CancellationToken ct)
        {
            return await _db.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
                .Include(p => p.Brand)
                .ToListAsync(ct);
        }
        public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _db.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(p => p.Id == id, ct);
        }

        public async Task UpdateProductAsync(Product product, CancellationToken ct)
        {
            _db.Products.Update(product);
            await _db.SaveChangesAsync(ct);
        }
        public IQueryable<Product> GetQueryable()
        {
            return _db.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
                .AsNoTracking();
        }

       public async Task<List<Product>> GetProductsBySellerIdAsync(Guid sellerId, CancellationToken ct)
        {
            return await _db.Products
               .Include(p => p.Category)
               .Include(p => p.ProductImages)
               .Include(p => p.ProductTags).ThenInclude(pt => pt.Tag)
               .Include(p => p.Brand)
               .Where(p => p.SellerId == sellerId)
               .ToListAsync(ct);
        }
    }

}
