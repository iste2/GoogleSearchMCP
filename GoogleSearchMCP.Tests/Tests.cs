namespace GoogleSearchMCP.Tests;

public class Tests
{

    [SetUp]
    public void Setup()
    {
        Environment.SetEnvironmentVariable("GOOGLE_API_KEY", "AIzaSyCDhsuGCzVzF1TfYch9_jktw9o95UHSGDY");
        Environment.SetEnvironmentVariable("GOOGLE_SEARCH_ENGINE_ID", "c3bfee9f1a29c4c27");
    }
    
    [Test]
    public async Task TestThatSearchResultsAreGeneratedAsync()
    {
        const string searchString = "Julius Caesar";
        var result = await GoogleSearchTool.Search(searchString);
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Not.Empty);
    }
}