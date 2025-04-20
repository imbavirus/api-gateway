using ApiGateway.Api.Middleware.Implementation;
using ApiGateway.Api.Services.PhoneCall;
using ApiGateway.Api.Managers.Data.Implementation;
using ApiGateway.Api.Services.PhoneCall.Implementation;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
using DotNetEnv;
using ApiGateway.Api.Hubs;
using ApiGateway.Api.Managers.Lacrm.Implementation;
using ApiGateway.Api.Managers.PhoneCall.Implementation;
using ApiGateway.Api.Managers.PhoneCall;
using ApiGateway.Api.Managers.Lacrm;

// Load environment variables from .env file
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
}).AddRazorRuntimeCompilation();

// Allows for Views to be in /Web folder
builder.Services.Configure<RazorViewEngineOptions>(options =>
{
    // {0} - Action Name
    // {1} - Controller Name
    // {2} - Area Name

    // Clear existing locations 
    options.ViewLocationFormats.Clear();

    // Add your custom locations prefixed with /Web
    options.ViewLocationFormats.Add("/Web/Views/{1}/{0}" + RazorViewEngine.ViewExtension); // /Web/Views/Controller/Action.cshtml
    options.ViewLocationFormats.Add("/Web/Views/Shared/{0}" + RazorViewEngine.ViewExtension); // /Web/Views/Shared/Action.cshtml
});

// Add API controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Allow trailing commas in JSON input since the object is sent as such
        options.JsonSerializerOptions.AllowTrailingCommas = true;
    });

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "API Gateway",
        Description = "An API Gateway for LACRM",
    });
});


// Add compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

// Add services to the container.

// Register HttpClient for LacrmHttpManager
builder.Services.AddHttpClient<ILacrmHttpManager, LacrmHttpManager>();

// Add managers.
builder.Services.AddSingleton<ILacrmManager, LacrmManager>();
builder.Services.AddSingleton<ILacrmHttpManager, LacrmHttpManager>();
builder.Services.AddSingleton<IPhoneCallManager, PhoneCallManager>();

// Add Datastore
builder.Services.AddSingleton<InMemoryDataStoreManager>();

// Add SignalR for table data handling
builder.Services.AddSignalR();

// Add Services
builder.Services.AddScoped<IPhoneCallService, PhoneCallService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
} 
else 
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseExceptionHandler(options => options.UseMiddleware<ExceptionMiddleware>());

app.UseHttpsRedirection();
app.UseResponseCompression(); 

app.UseStaticFiles(); // For serving static files from wwwroot

app.UseRouting();

app.UseAuthorization();

// Map both MVC and API routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers(); // For API endpoints
app.MapHub<DataHub>("/dataHub");

app.Run();
