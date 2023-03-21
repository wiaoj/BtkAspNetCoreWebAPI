using AspNetCoreRateLimit;
using Microsoft.AspNetCore.Mvc;
using NLog;
using Services;
using Services.Contracts;
using Spectre.Console;
using WebApi.Extensions;

AnsiConsole.Write(new FigletText("BTK Book Store Application").Centered().Color(Color.Green3));

WebApplicationBuilder webApplicationBuilder = WebApplication.CreateBuilder(args);

// Add services to the container.
LogManager.LoadConfiguration(String.Concat(Directory.GetCurrentDirectory(), "/nlog.config"));
webApplicationBuilder.Services.AddControllers(config => {
    config.RespectBrowserAcceptHeader = true;
    config.ReturnHttpNotAcceptable = true;
    config.CacheProfiles.Add("5mins", new() { 
        Duration = 300 
    });
})
.AddXmlDataContractSerializerFormatters()
.AddCustomCsvFormatter()
.AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly)
.AddNewtonsoftJson(mvcNewtonsonftOpsions =>
    mvcNewtonsonftOpsions.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

webApplicationBuilder.Services.Configure<ApiBehaviorOptions>(apiBehaviorOptions => {
    apiBehaviorOptions.SuppressModelStateInvalidFilter = true;
});

webApplicationBuilder.Services.AddEndpointsApiExplorer()
                              .ConfigureSwagger()
                              .ConfigureSqlContext(webApplicationBuilder.Configuration)
                              .ConfigureRepositoryManager()
                              .ConfigureServiceManager()
                              .ConfigureLoggerService()
                              .AddAutoMapper(typeof(Program))
                              .ConfigureActionFilters()
                              .ConfigureCors()
                              .ConfigureDataShaper()
                              .AddCustomMediaTypes()
                              .AddScoped<IBookLinks, BookLinks>()
                              .ConfigureVersioning()
                              .ConfigureResponseCaching()
                              .ConfigureHttpCacheHeaders()
                              .AddMemoryCache()
                              .ConfigureRateLimitingOptions()
                              .AddHttpContextAccessor()
                              .ConfigureIdentity()
                              .ConfigureJWT(webApplicationBuilder.Configuration)

                              .RegisterRepositories()
                              .RegisterServices();

WebApplication webApplication = webApplicationBuilder.Build();

ILoggerService logger = webApplication.Services.GetRequiredService<ILoggerService>();
webApplication.ConfigureExceptionHandler(logger);

if(webApplication.Environment.IsDevelopment()) {
    webApplication.UseSwagger();
    webApplication.UseSwaggerUI(s => {
        s.SwaggerEndpoint("/swagger/v1/swagger.json", "BTK Akademi v1");
        s.SwaggerEndpoint("/swagger/v2/swagger.json", "BTK Akademi v2");
    });
}

if(webApplication.Environment.IsProduction()) {
    webApplication.UseHsts();
}


webApplication.UseHttpsRedirection();

webApplication.UseIpRateLimiting();
webApplication.UseCors("CorsPolicy");
webApplication.UseResponseCaching();
webApplication.UseHttpCacheHeaders();

webApplication.UseAuthentication();
webApplication.UseAuthorization();

webApplication.MapControllers();

await webApplication.RunAsync();