using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using OmniRecipesApi.Models;
using OmniRecipesApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<RecipeService>();
builder.Services.AddSingleton<AzureService>();
builder.Services.AddTransient<ApiKeyValidation>();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddTransient<IImageService, LocalImageService>();
}
else
{
    builder.Services.AddTransient<IImageService, AzureService>();
}

builder.Services.Configure<RecipesDatabaseSettings>(builder.Configuration.GetSection("RecipesDatabase"));
builder.Services.Configure<AzureSettings>(builder.Configuration.GetSection("Azure"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>{
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme()
    {
        Name = Constants.ApiKeyHeaderName,
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Description = "Authorization by x-api-key inside request's header",
        Scheme = "ApiKeyScheme"
    });

    var key = new OpenApiSecurityScheme()
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "ApiKey"
        },
        In = ParameterLocation.Header
    };
    var requirement = new OpenApiSecurityRequirement
    {
    { key, new List<string>() }
    };
    c.AddSecurityRequirement(requirement);

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Enable serving static files from the wwwroot folder
    app.UseStaticFiles();

    // Determine the path for the uploaded images
    var imageFolderPath = Path.Combine(builder.Environment.ContentRootPath, "UploadedImages");

    // Check if the directory exists, and if not, create it
    if (!Directory.Exists(imageFolderPath))
    {
        Directory.CreateDirectory(imageFolderPath);
    }

    // Adding a static file route for images
    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(imageFolderPath),
        RequestPath = "/Images"
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseMiddleware<ApiKeyMiddleware>();

app.MapControllers();

app.Run();
