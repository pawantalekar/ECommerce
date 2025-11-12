using CatalogService.Api.Commands.AddProduct;
using Ecom.Application.AuthService.Application.Interfaces;
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
            

            builder.Services.AddHttpClient();

            // registering the services and depe
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            builder.Services.AddScoped<ICatalogRepository, CatalogRepository>();

            // DbContext
            builder.Services.AddDbContext<AuthDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DCS")));

            // for the JWT Authentication part secret name use instead of key 

            var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]);
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            builder.Services.AddSwaggerGen();
          
            var app = builder.Build();
            app.UseStaticFiles();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors("AllowAngularDev");
            app.UseHttpsRedirection();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
