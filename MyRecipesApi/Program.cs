using Microsoft.OpenApi.Models;
using MyRecipesApi.Models;
using MyRecipesApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<RecipeService>();
builder.Services.AddSingleton<AzureService>();
builder.Services.AddTransient<ApiKeyValidation>();

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
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseMiddleware<ApiKeyMiddleware>();

app.MapControllers();

app.Run();
