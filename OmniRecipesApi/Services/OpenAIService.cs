using Azure.Identity;
using Azure.Core;
using System.Text.Json;
using System.Text;

using System.Net;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.IO;
using Microsoft.Extensions.Options;

namespace OmniRecipesApi.Services
{
    public class OpenAIService
    {


        private readonly string endpoint;
        private readonly string modelDeployment;
        private readonly string apiVersion = "2024-08-01-preview";

        private readonly DefaultAzureCredential credential;

        private readonly string bearerToken;
        public OpenAIService(IOptions<OpenAISettings> openAISettings)
        {
            endpoint = openAISettings.Value.Endpoint;
            modelDeployment = openAISettings.Value.DeploymentName;
            credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
            {
                ExcludeEnvironmentCredential = true,
                ExcludeManagedIdentityCredential = true,
                ExcludeSharedTokenCacheCredential = true,
                ExcludeInteractiveBrowserCredential = true,
                ExcludeAzurePowerShellCredential = true,
                ExcludeVisualStudioCodeCredential = false,
                ExcludeAzureCliCredential = false
            });
            bearerToken = credential.GetToken(new TokenRequestContext(["https://cognitiveservices.azure.com/.default"])).Token;
        }

        public async Task<String?> GetCompletionAsync(List<String> pdfImageFiles)
        {
            var prompt = $@"
            The following image contains a recipe. Please extract the recipe details and return them in the following JSON format:

            {{
            ""Title"": ""Recipe Title"",
            ""Subtitle"": ""Optional subtitle"",
            ""Ingredients"": [
                {{ ""Name"": ""Flour"", ""Quantity"": 500, ""Unit"": ""grams"" }},
                {{ ""Name"": ""Sugar"", ""Quantity"": 200, ""Unit"": ""grams"" }}
            ],
            ""Steps"": [
                ""Mix all ingredients."",
                ""Bake at 180°C for 30 minutes.""
            ],
            ""Image"": null
            }}";


            var userPromptParts = new List<JsonObject>
            {
                new JsonObject
                {
                    { "type", "text" },
                    { "text", prompt }
                }
            };

            foreach (var pdfImageFile in pdfImageFiles)
            {
                var imageBytes = await File.ReadAllBytesAsync(pdfImageFile);
                var base64Image = Convert.ToBase64String(imageBytes);
                userPromptParts.Add(new JsonObject
                {
                    { "type", "image_url" },
                    { "image_url", new JsonObject { { "url", $"data:image/jpeg;base64,{base64Image}" } } }
                });
            }

            JsonObject jsonPayload = new JsonObject
            {
                {
                    "messages", new JsonArray
                    {
                        new JsonObject
                        {
                            { "role", "system" },
                            { "content", "You are an AI assistant that extracts data from documents and returns them as structured JSON objects. Do not return as a code block." }
                        },
                        new JsonObject
                        {
                            { "role", "user" },
                            { "content", new JsonArray(userPromptParts.ToArray())}
                        }
                    }
                },
                { "model", modelDeployment },
                { "max_tokens", 4096 },
                { "temperature", 0.1 },
                { "top_p", 0.1 },
            };

            string payload = JsonSerializer.Serialize(jsonPayload, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            using (HttpClient httpClient = new HttpClient { Timeout = TimeSpan.FromMinutes(10) })
            {
                httpClient.BaseAddress = new Uri(endpoint);
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");
                httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var stringContent = new StringContent(payload, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync($"openai/deployments/{modelDeployment}/chat/completions?api-version={apiVersion}", stringContent);

                if (response.IsSuccessStatusCode)
                {
                    using var responseStream = await response.Content.ReadAsStreamAsync();
                    // Parse the JSON response using JsonDocument
                    using (var jsonDoc = await JsonDocument.ParseAsync(responseStream))
                    {
                        // Access the message content dynamically
                        JsonElement jsonElement = jsonDoc.RootElement;
                        string messageContent = jsonElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

                        var result = JsonSerializer.Deserialize<JsonElement>(messageContent);
                        Console.WriteLine(result);
                        return result.ToString();
                    }
                }
                else
                {
                    Console.WriteLine(response);
                    return null;
                }
            }

        }

    }
}