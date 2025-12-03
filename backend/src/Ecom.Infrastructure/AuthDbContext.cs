using System;
using Ecom.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecom.Infrastructure
{
    public partial class AuthDbContext : DbContext
    {
        public AuthDbContext() { }

        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options) { }


        public virtual DbSet<Brand> Brands { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductImage> ProductImages { get; set; } = null!;
        public virtual DbSet<Tag> Tags { get; set; } = null!;
        public virtual DbSet<ProductTag> ProductTags { get; set; } = null!;

        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        public virtual DbSet<Cart> Carts { get; set; }

        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }

        public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<Review> Reviews { get; set; }

        public virtual DbSet<SellerRequest> SellerRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // BRAND
            modelBuilder.Entity<Brand>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.Slug).HasMaxLength(100);
                entity.Property(e => e.LogoUrl).HasMaxLength(500);
            });

            // CATEGORY
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.Slug).HasMaxLength(100);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            });

            // PRODUCT
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Slug).IsUnique();
                entity.HasIndex(e => e.Sku).IsUnique();
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
                entity.Property(e => e.Name).HasMaxLength(200);
                entity.Property(e => e.Slug).HasMaxLength(200);
                entity.Property(e => e.ShortDescription).HasMaxLength(500);
                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.Sku).HasMaxLength(100);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsFeatured).HasDefaultValue(false);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.Category)
                      .WithMany(p => p.Products)
                      .HasForeignKey(d => d.CategoryId)
                      .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Brand)
                      .WithMany()
                      .HasForeignKey(d => d.BrandId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // PRODUCT IMAGE
            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
                entity.Property(e => e.Url).HasMaxLength(500);
                entity.Property(e => e.AltText).HasMaxLength(200);
                entity.Property(e => e.SortOrder).HasDefaultValue(0);
                entity.Property(e => e.IsThumbnail).HasDefaultValue(false);

                entity.HasOne(d => d.Product)
                      .WithMany(p => p.ProductImages)
                      .HasForeignKey(d => d.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // TAG
            modelBuilder.Entity<Tag>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
                entity.Property(e => e.Name).HasMaxLength(50);
            });

            //product tag


            modelBuilder.Entity<ProductTag>(entity =>
            {
                entity.ToTable("ProductTags");
                entity.HasKey(e => new { e.ProductId, e.TagId });



                entity.Property(e => e.ProductId).HasColumnName("ProductId");
                entity.Property(e => e.TagId).HasColumnName("TagId");

                entity.HasOne(pt => pt.Product)
                      .WithMany(p => p.ProductTags)
                      .HasForeignKey(pt => pt.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(pt => pt.Tag)
                      .WithMany(t => t.ProductTags)
                      .HasForeignKey(pt => pt.TagId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("Tags");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.Name).HasColumnName("Name");
            });



            // USER & REFRESH TOKEN (UNCHANGED)
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
                entity.Property(e => e.Name).HasMaxLength(255);
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.PasswordHash).HasMaxLength(500);
                entity.Property(e => e.Role).HasMaxLength(50).HasDefaultValue("User");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
                entity.Property(e => e.Token).HasMaxLength(500);
                entity.HasOne(d => d.User)
                      .WithMany(p => p.RefreshTokens)
                      .HasForeignKey(d => d.UserId);
            });


            //for cart related
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Carts__3214EC07F810DF33");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__CartItem__3214EC07AC0C488D");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
                entity.Property(e => e.Quantity).HasDefaultValue(1);

                entity.HasOne(d => d.Cart).WithMany(p => p.Items).HasConstraintName("FK_CartItems_Cart");
            });

            //order service 

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__OrderIte__3214EC0779FF2A7E");

                entity.HasIndex(e => e.OrderId, "IX_OrderItems_OrderId");

                entity.HasIndex(e => e.ProductId, "IX_OrderItems_ProductId");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
                entity.Property(e => e.ProductName).HasMaxLength(200);
                entity.Property(e => e.SubTotal)
                    .HasComputedColumnSql("([UnitPrice]*[Quantity])", true)
                    .HasColumnType("decimal(29, 2)");
                entity.Property(e => e.ThumbnailUrl).HasMaxLength(500);
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.Order).WithMany(p => p.OrderItems).HasForeignKey(d => d.OrderId);

                entity.HasOne(d => d.Product).WithMany(p => p.OrderItem)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Orders__3214EC078C28CC25");

                entity.HasIndex(e => e.OrderNumber, "IX_Orders_OrderNumber").IsUnique();

                entity.HasIndex(e => e.UserId, "IX_Orders_UserId");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
                entity.Property(e => e.OrderNumber).HasMaxLength(30);
                entity.Property(e => e.OrderStatus)
                    .HasMaxLength(20)
                    .HasDefaultValue("Confirmed");
                entity.Property(e => e.PaymentStatus)
                    .HasMaxLength(20)
                    .HasDefaultValue("Pending");
                entity.Property(e => e.RazorpayOrderId).HasMaxLength(50);
                entity.Property(e => e.RazorpayPaymentId).HasMaxLength(50);
                entity.Property(e => e.ShippingAddressLine1).HasMaxLength(200);
                entity.Property(e => e.ShippingAddressLine2).HasMaxLength(200);
                entity.Property(e => e.ShippingCity).HasMaxLength(50);
                entity.Property(e => e.ShippingFullName).HasMaxLength(100);
                entity.Property(e => e.ShippingPhone).HasMaxLength(15);
                entity.Property(e => e.ShippingPincode).HasMaxLength(10);
                entity.Property(e => e.ShippingState).HasMaxLength(50);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18, 2)");

                entity.HasOne(d => d.User).WithMany(p => p.Orders).HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Products__3214EC07F28EE719");

                entity.HasIndex(e => e.Slug, "UQ__Products__BC7B5FB6ECF9EE69").IsUnique();

                entity.HasIndex(e => e.Sku, "UQ__Products__CA1ECF0D2E139FBC").IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsFeatured).HasDefaultValue(false);
                entity.Property(e => e.Name).HasMaxLength(200);
                entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
                entity.Property(e => e.ShortDescription).HasMaxLength(500);
                entity.Property(e => e.Sku)
                    .HasMaxLength(100)
                    .HasColumnName("SKU");
                entity.Property(e => e.Slug).HasMaxLength(200);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Users__3214EC0713F0F151");

                entity.HasIndex(e => e.Email, "IX_Users_Email");

                entity.HasIndex(e => e.Email, "UQ__Users__A9D10534988C48B4").IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysdatetime())");
                entity.Property(e => e.Email).HasMaxLength(255);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.Name).HasMaxLength(255);
                entity.Property(e => e.PasswordHash).HasMaxLength(500);
                entity.Property(e => e.Role)
                    .HasMaxLength(50)
                    .HasDefaultValue("User");
                entity.Property(e => e.Ssoprovider)
                    .HasMaxLength(50)
                    .HasColumnName("SSOProvider");
                entity.Property(e => e.SsoproviderId)
                    .HasMaxLength(255)
                    .HasColumnName("SSOProviderId");
            });

            //Review 

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Reviews__3214EC073A636C96");

                entity.HasIndex(e => e.CreatedAt, "IX_Reviews_CreatedAt").IsDescending();

                entity.HasIndex(e => e.ProductId, "IX_Reviews_ProductId");

                entity.HasIndex(e => e.Status, "IX_Reviews_Status");

                entity.HasIndex(e => e.UserId, "IX_Reviews_UserId");

                entity.HasIndex(e => new { e.ProductId, e.UserId }, "UX_Reviews_ProductId_UserId").IsUnique();

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .HasDefaultValue("Pending");
            });

            //sellerRequests
            modelBuilder.Entity<SellerRequest>(entity =>
            {
                entity.HasIndex(e => e.Status, "IX_SellerRequests_Status");

                entity.HasIndex(e => e.UserId, "IX_SellerRequests_UserId");

                entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
                entity.Property(e => e.RequestedAt).HasDefaultValueSql("(getutcdate())");
                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .HasDefaultValue("Pending");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}