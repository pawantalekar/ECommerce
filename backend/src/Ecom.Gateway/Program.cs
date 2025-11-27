using CartService.Api.Commands.AddToCart;
using CatalogService.Api.Commands.AddProduct;
using Ecom.Application.AuthService.Application.Interfaces;
using Ecom.Application.CartService.Application.Interfaces;
using Ecom.Application.CatalogService.Application.Interfaces;
using Ecom.Infrastructure;
using Ecom.Infrastructure.Repository;
using Ecom.Infrastructure.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Ecom.Gateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var options = new WebApplicationOptions
            {
                Args = args,
                ContentRootPath = AppContext.BaseDirectory,
                WebRootPath = "wwwroot"
            };
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularDev", p =>
                    p.WithOrigins("http://localhost:4200")
                     .AllowAnyHeader()
                     .AllowAnyMethod()
                     .AllowCredentials());
            });

            builder.Services.AddControllers();

            builder.Services.AddValidatorsFromAssemblyContaining<AddProductCommandValidator>();

            builder.Services.AddFluentValidationAutoValidation();


            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<AddProductCommandHandler>());

            builder.Services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssemblies(
                typeof(AddProductCommandHandler).Assembly,
                typeof(AddToCartCommand).Assembly
                 )
             );


            builder.Services.AddHttpClient();
            builder.Services.AddHttpContextAccessor();

            // registering the services and depe
            builder.Services.AddScoped<AuthDbContext>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            builder.Services.AddScoped<ICatalogRepository, CatalogRepository>();
            builder.Services.AddScoped<ICartRepository, CartRepository>();

            // DbContext
            builder.Services.AddDbContext<AuthDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DCS")));


            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<AddToCartCommand>());
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<AddToCartCommandHandler>());
            builder.Services.AddValidatorsFromAssemblyContaining<AddToCartCommandValidator>();

            // for the JWT Authentication part secret name use instead of key 

            var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]);
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(options =>
           {
               options.MapInboundClaims = false;
               options.TokenValidationParameters = new TokenValidationParameters
               {
                   ValidateIssuer = true,
                   ValidateAudience = true,
                   ValidateLifetime = true,
                   ValidateIssuerSigningKey = true,
                   ValidIssuer = builder.Configuration["Jwt:Issuer"],
                   ValidAudience = builder.Configuration["Jwt:Audience"],
                   IssuerSigningKey = new SymmetricSecurityKey(key),

                   NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
                   RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
               };

               // Add debugging events
               options.Events = new JwtBearerEvents
               {
                   OnAuthenticationFailed = context =>
                   {
                       Console.WriteLine($"? AUTH FAILED: {context.Exception.Message}");
                       if (context.Exception.InnerException != null)
                       {
                           Console.WriteLine($"   Inner: {context.Exception.InnerException.Message}");
                       }
                       return Task.CompletedTask;
                   },
                   OnTokenValidated = context =>
                   {
                       Console.WriteLine("? TOKEN VALIDATED");
                       var claims = context.Principal?.Claims.Select(c => $"{c.Type}: {c.Value}");
                       Console.WriteLine($"   Claims: {string.Join(", ", claims ?? [])}");
                       return Task.CompletedTask;
                   },
                   OnMessageReceived = context =>
                   {
                       var token = context.Request.Headers.Authorization.FirstOrDefault();
                       if (!string.IsNullOrEmpty(token))
                       {
                           Console.WriteLine($"?? TOKEN RECEIVED: {token[..Math.Min(50, token.Length)]}...");
                       }
                       else
                       {
                           Console.WriteLine("?? NO TOKEN IN REQUEST");
                       }
                       return Task.CompletedTask;
                   },
                   OnChallenge = context =>
                   {
                       Console.WriteLine($"?? CHALLENGE: {context.Error}, {context.ErrorDescription}");
                       return Task.CompletedTask;
                   }
               };
           });
            builder.Services.AddAuthorization();


            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            app.UseStaticFiles();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseCors("AllowAngularDev");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
