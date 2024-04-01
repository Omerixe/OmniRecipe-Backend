using System.Net;

namespace MyRecipesApi.Services;
public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ApiKeyValidation _apiKeyValidation;
    public ApiKeyMiddleware(RequestDelegate next, ApiKeyValidation apiKeyValidation)
    {
        _next = next;
        _apiKeyValidation = apiKeyValidation;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        Console.WriteLine($"ApiKey: {context.Request.Headers[Constants.ApiKeyHeaderName]}");
        if (string.IsNullOrWhiteSpace(context.Request.Headers[Constants.ApiKeyHeaderName]))
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return;
        }
        string? userApiKey = context.Request.Headers[Constants.ApiKeyHeaderName];
        if (!_apiKeyValidation.IsValidApiKey(userApiKey!))
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return;
        }
        await _next(context);
    }
}