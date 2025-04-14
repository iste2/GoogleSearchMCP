using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;

namespace GoogleSearchMCP;

[McpServerToolType]
public static class GoogleSearchTool
{
    [McpServerTool(Name = "google_search"), Description("Search the web using Google")]
    public static async Task<SearchResult[]> Search(string searchString, int resultCount = 10)
    {
        var apiKey = Environment.GetEnvironmentVariable("GOOGLE_API_KEY");
        var searchEngineId = Environment.GetEnvironmentVariable("GOOGLE_SEARCH_ENGINE_ID");
        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(searchEngineId))
            throw new InvalidOperationException("Google API key or search engine ID is not set.");
        var searchClient = new GoogleSearchClient(apiKey, searchEngineId);
        var results = await searchClient.SearchAndParseAsync(searchString, resultCount);
        return results;
    }
}

public class GoogleSearchClient(string apiKey, string searchEngineId)
{
    private readonly HttpClient _httpClient = new();

    private async Task<string> SearchAsync(string query, int resultCount = 10)
    {
        var url = $"https://www.googleapis.com/customsearch/v1?key={apiKey}&cx={searchEngineId}&q={Uri.EscapeDataString(query)}&num={resultCount}";
        
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadAsStringAsync();
    }
    
    public async Task<SearchResult[]> SearchAndParseAsync(string query, int resultCount = 10)
    {
        var jsonResponse = await SearchAsync(query, resultCount);
        using var doc = JsonDocument.Parse(jsonResponse);
        
        var items = doc.RootElement.GetProperty("items");
        var results = new SearchResult[items.GetArrayLength()];
        
        for (var i = 0; i < results.Length; i++)
        {
            var item = items[i];
            results[i] = new SearchResult
            {
                Title = item.GetProperty("title").GetString(),
                Link = item.GetProperty("link").GetString(),
                Snippet = item.GetProperty("snippet").GetString()
            };
        }
        
        return results;
    }
}

public class SearchResult
{
    public string Title { get; set; }
    public string Link { get; set; }
    public string Snippet { get; set; }
}