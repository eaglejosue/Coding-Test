namespace IntegrationTest;

public class AtmControllerIntegrationTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact(DisplayName = "GET /api/atm/payout/{amount} returns exact combinations for 230 EUR")]
    public async Task GetPayoutCombinations_ReturnsExactCombinations_For230EUR()
    {
        int amount = 230;
        var expectedCombinations = new HashSet<string>
        {
            "23 x 10 EUR",
            "1 x 50 EUR + 18 x 10 EUR",
            "2 x 50 EUR + 13 x 10 EUR",
            "3 x 50 EUR + 8 x 10 EUR",
            "4 x 50 EUR + 3 x 10 EUR",
            "1 x 100 EUR + 13 x 10 EUR",
            "1 x 100 EUR + 1 x 50 EUR + 8 x 10 EUR",
            "1 x 100 EUR + 2 x 50 EUR + 3 x 10 EUR",
            "2 x 100 EUR + 3 x 10 EUR"
        };

        var response = await _client.GetAsync($"api/atm/payout/{amount}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadAsStringAsync();
        var payoutResult = JsonSerializer.Deserialize<BaseResponse<List<string>>>(responseContent);

        payoutResult.Should().NotBeNull();
        payoutResult!.Data.Should().BeEquivalentTo(expectedCombinations);
    }
}