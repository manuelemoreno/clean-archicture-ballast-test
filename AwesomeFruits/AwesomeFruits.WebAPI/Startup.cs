using System;
using System.Collections.Generic;
using System.Text;
using AwesomeFruits.Application.Mapping.Profiles;
using AwesomeFruits.Infrastructure.Data.Contexts;
using AwesomeFruits.WebAPI.Extensions;
using AwesomeFruits.WebAPI.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace AwesomeFruits.WebAPI;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "AwesomeFruits.WebAPI", Version = "v1" });
        });

        services.AddServices();

        services.AddAutoMapper(typeof(FruitProfile).Assembly);
        services.AddAutoMapper(typeof(SaveFruitDtoProfile).Assembly);
        services.AddAutoMapper(typeof(UpdateFruitDtoProfile).Assembly);

        services.AddSingleton(sp =>
        {
            var mongoSettings = Configuration.GetSection("MongoDbSettings");

            var connectionString = mongoSettings["ConnectionString"];
            var databaseName = mongoSettings["DatabaseName"];
            return new MongoDbContext(connectionString, databaseName);
        });

        var jwtSettings = Configuration.GetSection("Jwt");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["IssuerKey"])),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["IssuerName"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // Optional: reduce or eliminate clock skew
                };
            });

        services.AddSwaggerGen(c =>
        {
            // Define the Bearer Authentication scheme
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Add: \"Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,

                    },
                    new List<string>()
                }
            });
        });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AwesomeFruits.WebAPI v1"));
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware<NotFoundMiddleware>();
        app.UseMiddleware<ValidationErrorsMiddleware>();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}