namespace IntegrationTest;

public class CustomerControllerIntegrationTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact(DisplayName = "GET /api/customer returns NoContent when no customers exist")]
    public async Task GetCustomers_ReturnsNoContent_WhenNoCustomersExist()
    {
        // Use um filtro que garanta que não existam clientes na consulta
        var response = await _client.GetAsync("api/customer?Id=0");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact(DisplayName = "POST /api/customer adds customers and returns sorted list")]
    public async Task PostCustomers_AddsCustomers_AndReturnsSortedList()
    {
        // Arrange: Cria uma lista de clientes para inserir.
        // Os dados devem ser válidos (idade acima de 18, IDs não repetidos) conforme a lógica do serviço.
        var customersToPost = new List<Customer>
        {
            new() { Id = 1, FirstName = "Leia", LastName = "Anderson", Age = 25 },
            new() { Id = 2, FirstName = "Sadie", LastName = "Chan", Age = 30 },
            new() { Id = 3, FirstName = "Jose",  LastName = "Liberty", Age = 40 }
        };

        // Act: Realiza o POST dos clientes
        var postResponse = await _client.PostAsJsonAsync("api/customer", customersToPost);
        postResponse.EnsureSuccessStatusCode();

        // Supondo que a API retorne um objeto BaseResponse com as propriedades Data (lista de Customer)
        var baseResponse = await postResponse.Content.ReadFromJsonAsync<BaseResponse<List<Customer>>>();
        baseResponse.Should().NotBeNull();
        baseResponse.Data.Should().NotBeNull();
        baseResponse.Data.Should().HaveCount(customersToPost.Count);

        // Verifica se os clientes estão ordenados por LastName e, em caso de empate, por FirstName
        var sorted = baseResponse.Data.OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ToList();
        sorted.Select(c => c.Id).Should().Equal(baseResponse.Data.Select(c => c.Id));

        // Opcional: use o GET para verificar se os dados persistiram (utilize outro CacheKey, se necessário)
        var getResponse = await _client.GetAsync("api/customer");

        // Se a rota retornar conteúdo, verifique a ordenação
        if (getResponse.StatusCode != HttpStatusCode.NoContent)
        {
            var getBaseResponse = await getResponse.Content.ReadFromJsonAsync<BaseResponse<List<Customer>>>();
            getBaseResponse.Should().NotBeNull();
            getBaseResponse.Data.Should().NotBeNull();

            var sortedGet = getBaseResponse.Data.OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ToList();
            sortedGet.Select(c => c.Id).Should().Equal(getBaseResponse.Data.Select(c => c.Id));
        }
    }

    [Fact(DisplayName = "POST /api/customer returns BadRequest when no valid customers are provided")]
    public async Task PostCustomers_ReturnsBadRequest_WhenNoValidCustomers()
    {
        // Arrange: Cria clientes com dados inválidos (por exemplo, idade abaixo de 18)
        var invalidCustomers = new List<Customer>
        {
            new() { Id = 10, FirstName = "Invalid", LastName = "User", Age = 16 }
        };

        // Act: Realiza o POST dos clientes inválidos
        var response = await _client.PostAsJsonAsync("api/customer", invalidCustomers);

        // Assert: A API deve retornar BadRequest
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        // Se desejar, deserializa a resposta para verificar as notificações retornadas
        var baseResponse = await response.Content.ReadFromJsonAsync<BaseResponse<object>>();
        baseResponse.Should().NotBeNull();
        baseResponse.Messages.Should().NotBeEmpty();
    }
}