namespace OmniRecipesApi.Services;

public class ApiKeyValidation
{
    private readonly IConfiguration _configuration;
    public ApiKeyValidation(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public bool IsValidApiKey(string userApiKey)
    {
        Console.WriteLine($"ApiKeyValidation.IsValidApiKey: userApiKey={userApiKey}");
        if (string.IsNullOrWhiteSpace(userApiKey))
            return false;
        string? apiKey = _configuration.GetValue<string>(Constants.ApiKeyName);
        if (apiKey == null || apiKey != userApiKey)
            return false;
        return true;
    }
}