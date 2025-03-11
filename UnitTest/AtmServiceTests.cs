namespace UnitTest;

public class AtmServiceTests
{
    private readonly Mock<IInternalNotificationService> _notificationServiceMock;
    private readonly AtmService _atmService;

    public AtmServiceTests()
    {
        // Cria um mock para o serviço de notificações
        _notificationServiceMock = new Mock<IInternalNotificationService>();
        // Instancia o AtmService passando o mock
        _atmService = new AtmService(_notificationServiceMock.Object);
    }

    [Theory]
    [InlineData(30, "3 x 10 EUR")]
    public async Task GetPayoutCombinationsAsync_DeveRetornar_CombinacaoExata_Para30EUR(int amount, string expectedCombination)
    {
        // Act
        var result = await _atmService.GetPayoutCombinationsAsync(amount);

        // Assert
        Assert.Contains(expectedCombination, result);

        // Não deve haver notificações, pois 30 é exato
        _notificationServiceMock.Verify(n => n.AddNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetPayoutCombinationsAsync_DeveRetornar_CombinacoesExatas_Para50EUR()
    {
        int amount = 50;
        var expectedCombinations = new HashSet<string>
        {
            "1 x 50 EUR",
            "5 x 10 EUR"
        };

        var result = await _atmService.GetPayoutCombinationsAsync(amount);
        var resultSet = new HashSet<string>(result);

        Assert.Equal(expectedCombinations, resultSet);
        _notificationServiceMock.Verify(n => n.AddNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetPayoutCombinationsAsync_DeveRetornar_CombinacoesExatas_Para60EUR()
    {
        int amount = 60;
        var expectedCombinations = new HashSet<string>
        {
            "1 x 50 EUR + 1 x 10 EUR",
            "6 x 10 EUR"
        };

        var result = await _atmService.GetPayoutCombinationsAsync(amount);
        var resultSet = new HashSet<string>(result);

        Assert.Equal(expectedCombinations, resultSet);
        _notificationServiceMock.Verify(n => n.AddNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetPayoutCombinationsAsync_DeveRetornar_CombinacoesExatas_Para80EUR()
    {
        int amount = 80;
        var expectedCombinations = new HashSet<string>
        {
            "1 x 50 EUR + 3 x 10 EUR",
            "8 x 10 EUR"
        };

        var result = await _atmService.GetPayoutCombinationsAsync(amount);
        var resultSet = new HashSet<string>(result);

        Assert.Equal(expectedCombinations, resultSet);
        _notificationServiceMock.Verify(n => n.AddNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetPayoutCombinationsAsync_DeveRetornar_CombinacoesExatas_Para140EUR()
    {
        int amount = 140;
        var expectedCombinations = new HashSet<string>
        {
            "1 x 100 EUR + 4 x 10 EUR",
            "2 x 50 EUR + 4 x 10 EUR",
            "1 x 50 EUR + 9 x 10 EUR",
            "14 x 10 EUR"
        };

        var result = await _atmService.GetPayoutCombinationsAsync(amount);
        var resultSet = new HashSet<string>(result);

        Assert.Equal(expectedCombinations, resultSet);
        _notificationServiceMock.Verify(n => n.AddNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetPayoutCombinationsAsync_DeveRetornar_CombinacoesExatas_Para230EUR()
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

        var result = await _atmService.GetPayoutCombinationsAsync(amount);
        var resultSet = new HashSet<string>(result);

        Assert.Equal(expectedCombinations, resultSet);
        _notificationServiceMock.Verify(n => n.AddNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Theory]
    [InlineData(370)]
    [InlineData(610)]
    [InlineData(980)]
    public async Task GetPayoutCombinationsAsync_DeveRetornar_ResultadosNaoVazios_ParaValoresMaiores(int amount)
    {
        var result = await _atmService.GetPayoutCombinationsAsync(amount);
        Assert.NotEmpty(result);
        _notificationServiceMock.Verify(n => n.AddNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetPayoutCombinationsAsync_DeveEnviarNotificacao_QuandoValorNaoPossuiCombinacaoExata()
    {
        // Exemplo: 23 EUR não é múltiplo de 10, então deverá buscar o valor mais próximo (20 EUR)
        int amount = 23;
        int expectedClosest = 20;

        var result = await _atmService.GetPayoutCombinationsAsync(amount);

        // Com 20 EUR, a única combinação é "2 x 10 EUR"
        Assert.Contains("2 x 10 EUR", result);
        _notificationServiceMock.Verify(n => n.AddNotification(
            "Note",
            It.Is<string>(msg => msg.Contains($"{amount} EUR") && msg.Contains($"{expectedClosest} EUR"))),
            Times.Once);
    }
}
