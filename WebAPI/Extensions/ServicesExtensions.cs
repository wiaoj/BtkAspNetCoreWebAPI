using AspNetCoreRateLimit;
using Entities.DataTransferObjects;
using Entities.Models;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Presentation.ActionFilters;
using Presentation.Controllers;
using Repositories.Contracts;
using Repositories.EFCore;
using Services;
using Services.Contracts;
using System.Text;

namespace WebApi.Extensions;
public static class ServicesExtensions {
    public static IServiceCollection ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) {
        services.AddDbContext<RepositoryContext>(options => options.UseSqlServer(configuration.GetConnectionString("sqlConnection")));
        return services;
    }

    public static IServiceCollection ConfigureRepositoryManager(this IServiceCollection services) {
        services.AddScoped<IRepositoryManager, RepositoryManager>();
        return services;
    }

    public static IServiceCollection ConfigureServiceManager(this IServiceCollection services) {
        services.AddScoped<IServiceManager, ServiceManager>();
        return services;
    }

    public static IServiceCollection ConfigureLoggerService(this IServiceCollection services) {
        services.AddSingleton<ILoggerService, LoggerManager>();
        return services;
    }

    public static IServiceCollection ConfigureActionFilters(this IServiceCollection services) {
        services.AddScoped<ValidationFilterAttribute>();
        services.AddSingleton<LogFilterAttribute>();
        services.AddScoped<ValidateMediaTypeAttribute>();
        return services;
    }

    public static IServiceCollection ConfigureCors(this IServiceCollection services) {
        services.AddCors(options => {
            options.AddPolicy("CorsPolicy", builder =>
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .WithExposedHeaders("X-Pagination")
            );
        });
        return services;
    }

    public static IServiceCollection ConfigureDataShaper(this IServiceCollection services) {
        services.AddScoped<IDataShaper<BookDto>, DataShaper<BookDto>>();
        return services;
    }

    public static IServiceCollection AddCustomMediaTypes(this IServiceCollection services) {
        services.Configure<MvcOptions>(config => {
            SystemTextJsonOutputFormatter? systemTextJsonOutputFormatter = config
            .OutputFormatters
            .OfType<SystemTextJsonOutputFormatter>()?.FirstOrDefault();

            if(systemTextJsonOutputFormatter is not null) {
                systemTextJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.btkakademi.hateoas+json");

                systemTextJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.btkakademi.apiroot+json");
            }

            XmlDataContractSerializerOutputFormatter? xmlOutputFormatter = config
            .OutputFormatters
            .OfType<XmlDataContractSerializerOutputFormatter>()?.FirstOrDefault();

            if(xmlOutputFormatter is not null) {
                xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.btkakademi.hateoas+xml");

                xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.btkakademi.apiroot+xml");
            }
        });
        return services;
    }

    public static IServiceCollection ConfigureVersioning(this IServiceCollection services) {
        services.AddApiVersioning(apiVersioningOptions => {
            apiVersioningOptions.ReportApiVersions = true;
            apiVersioningOptions.AssumeDefaultVersionWhenUnspecified = true;
            apiVersioningOptions.DefaultApiVersion = new ApiVersion(1, 0);
            apiVersioningOptions.ApiVersionReader = new HeaderApiVersionReader("api-version");

            apiVersioningOptions.Conventions.Controller<BooksController>().HasApiVersion(new ApiVersion(1, 0));

            apiVersioningOptions.Conventions.Controller<BooksV2Controller>().HasDeprecatedApiVersion(new ApiVersion(2, 0));
        });
        return services;
    }

    public static IServiceCollection ConfigureResponseCaching(this IServiceCollection services) {
        services.AddResponseCaching();
        return services;
    }

    public static IServiceCollection ConfigureHttpCacheHeaders(this IServiceCollection services) {
        services.AddHttpCacheHeaders(expirationModelOptions => {
            expirationModelOptions.MaxAge = 90;
            expirationModelOptions.CacheLocation = CacheLocation.Public;
        },
        validationOpt => {
            validationOpt.MustRevalidate = false;
        });
        return services;
    }

    public static IServiceCollection ConfigureRateLimitingOptions(this IServiceCollection services) {
        List<RateLimitRule> rateLimitRules = new() {
            new() {
                Endpoint = "*",
                Limit = 60,
                Period = "1m"
            }
        };

        services.Configure<IpRateLimitOptions>(ipRateLimitOptions => {
            ipRateLimitOptions.GeneralRules = rateLimitRules;
        });

        services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
        return services;
    }

    public static IServiceCollection ConfigureIdentity(this IServiceCollection services) {
        IdentityBuilder builder = services.AddIdentity<User, IdentityRole>(opts => {
            opts.Password.RequireDigit = true;
            opts.Password.RequireLowercase = false;
            opts.Password.RequireUppercase = false;
            opts.Password.RequireNonAlphanumeric = false;
            opts.Password.RequiredLength = 6;

            opts.User.RequireUniqueEmail = true;
        }).AddEntityFrameworkStores<RepositoryContext>()
          .AddDefaultTokenProviders();
        return services;
    }

    public static IServiceCollection ConfigureJWT(this IServiceCollection services, IConfiguration configuration) {
        IConfigurationSection jwtSettings = configuration.GetSection("JwtSettings");
        String? secretKey = jwtSettings["secretKey"];

        services.AddAuthentication(authenticationOptions => {
            authenticationOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            authenticationOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(jwtBearerOptions =>
            jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["validIssuer"],
                ValidAudience = jwtSettings["validAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            }
        );
        return services;
    }

    public static IServiceCollection ConfigureSwagger(this IServiceCollection services) {
        services.AddSwaggerGen(swaggerGenOptions => {
            swaggerGenOptions.SwaggerDoc("v1",
                new() {
                    Title = "BTK Akademi",
                    Version = "v1",
                    Description = "BTK Akademi ASP.NET Core Web API",
                    TermsOfService = new Uri("https://www.btkakademi.gov.tr/"),
                    Contact = new OpenApiContact {
                        Name = "wiaoj",
                        Email = "x@x.x",
                        Url = new Uri("https://www.x.com")
                    }
                });

            swaggerGenOptions.SwaggerDoc("v2", new() { Title = "BTK Akademi", Version = "v2" });

            swaggerGenOptions.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme() {
                In = ParameterLocation.Header,
                Description = "Place to add JWT with Bearer",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            swaggerGenOptions.AddSecurityRequirement(new() {
                {
                    new OpenApiSecurityScheme {
                        Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id="Bearer"
                        },
                        Name = "Bearer"
                    },
                    new List<String>()
                }
            });
        });
        return services;
    }

    public static IServiceCollection RegisterRepositories(this IServiceCollection services) {
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        return services;
    }

    public static IServiceCollection RegisterServices(this IServiceCollection services) {
        services.AddScoped<IBookService, BookManager>();
        services.AddScoped<ICategoryService, CategoryManager>();
        services.AddScoped<IAuthenticationService, AuthenticationManager>();
        return services;
    }
}