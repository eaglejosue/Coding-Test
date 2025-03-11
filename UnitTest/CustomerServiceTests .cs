namespace UnitTest;

public class CustomerServiceTests
{
    private readonly DbCoding _db;
    private readonly MemoryCache _memoryCache;
    private readonly Mock<IValidator<Customer>> _validatorMock;
    private readonly Mock<IInternalNotificationService> _notificationServiceMock;
    private readonly CustomerService _customerService;

    public CustomerServiceTests()
    {
        // Configura o EF Core InMemory com um banco com nome único para isolar os testes
        var options = new DbContextOptionsBuilder<DbCoding>().UseSqlite("Data Source=CodingTest.db;Cache=Shared").Options;
        _db = new DbCoding(options);

        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _validatorMock = new Mock<IValidator<Customer>>();
        _notificationServiceMock = new Mock<IInternalNotificationService>();

        // Garante que a coleção de clientes esteja vazia
        var existingCustomers = _db.Customers.ToList();
        _db.Customers.RemoveRange(existingCustomers);
        _db.SaveChanges();

        _customerService = new CustomerService(_db, _memoryCache, _validatorMock.Object, _notificationServiceMock.Object);
    }

    #region Testes para GetAllAsync

    [Fact]
    public async Task GetAllAsync_ReturnsCachedCustomers_WhenCacheExists()
    {
        // Arrange
        var filters = new CustomerFilters { Id = 1 };

        var cachedCustomers = new List<Customer>
        {
            new() { Id = 1, FirstName = "John", LastName = "Doe", Age = 30 }
        };

        _memoryCache.Set(filters.CacheKey(), cachedCustomers, TimeSpan.FromMinutes(5));

        // Act
        var result = await _customerService.GetAllAsync(filters);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(cachedCustomers.Count, result.Count);
        Assert.Equal(cachedCustomers.First().Id, result.First().Id);
    }

    [Fact]
    public async Task GetAllAsync_FetchesFromDatabase_AndCachesResult_WhenNoCacheEntry()
    {
        // Arrange
        var filters = new CustomerFilters { };

        var testCustomers = new List<Customer>
        {
            new() { Id = 1, FirstName = "Alice", LastName = "Smith", Age = 25 },
            new() { Id = 2, FirstName = "Bob", LastName = "Johnson", Age = 35 }
        };

        _db.Customers.AddRange(testCustomers);
        await _db.SaveChangesAsync();

        // Act
        var result = await _customerService.GetAllAsync(filters);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(testCustomers.Count, result.Count);

        // Verifica se o cache foi definido
        bool cacheExists = _memoryCache.TryGetValue(filters.CacheKey(), out List<Customer>? cached);
        Assert.True(cacheExists);
        Assert.Equal(testCustomers.Count, cached.Count);
    }

    #endregion

    #region Testes para AddCustomers

    [Fact]
    public void AddCustomers_AddsValidCustomers_AndSortsThem()
    {
        // Arrange
        // Insere clientes existentes no banco
        var existingCustomers = new List<Customer>
        {
            new() { Id = 1, FirstName = "Aaaa", LastName = "Aaaa", Age = 20 },
            new() { Id = 2, FirstName = "Bbbb", LastName = "Aaaa", Age = 56 },
            new() { Id = 3, FirstName = "Aaaa", LastName = "Cccc", Age = 32 }
        };
        _db.Customers.AddRange(existingCustomers);
        _db.SaveChanges();

        // Novos clientes válidos para inserir
        var newCustomers = new List<Customer>
        {
            new() { Id = 4, FirstName = "Bbbb", LastName = "Bbbb", Age = 26 },
            new() { Id = 5, FirstName = "Aaaa", LastName = "Dddd", Age = 70 }
        };

        // Configura o validador para retornar resultado válido para os novos clientes
        _validatorMock.Setup(v => v.Validate(It.IsAny<Customer>())).Returns(new ValidationResult());

        // Act
        var result = _customerService.AddCustomers(newCustomers);

        // Assert
        // Espera que o resultado não seja nulo e contenha todos os clientes (existentes + novos)
        Assert.NotNull(result);
        Assert.Equal(existingCustomers.Count + newCustomers.Count, result.Count);

        // Verifica se estão ordenados por lastName e depois por firstName
        var sorted = result.OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ToList();
        Assert.Equal(sorted.Select(c => c.Id), result.Select(c => c.Id));

        // Verifica que nenhuma notificação foi enviada
        _notificationServiceMock.Verify(n => n.AddNotification(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _notificationServiceMock.Verify(n => n.AddNotifications(It.IsAny<ValidationResult>()), Times.Never);
    }

    [Fact]
    public void AddCustomers_DoesNotAdd_WhenAllCustomersInvalid()
    {
        // Arrange
        // Configura o validador para retornar resultado inválido
        _validatorMock.Setup(v => v.Validate(It.IsAny<Customer>()))
            .Returns(new ValidationResult(new List<ValidationFailure> { new ("Age", "A idade deve ser maior que 18") }));

        var newCustomers = new List<Customer>
        {
            new() { Id = 10, FirstName = "Test", LastName = "User", Age = 17 } // Idade abaixo do permitido
        };

        // Act
        var result = _customerService.AddCustomers(newCustomers);

        // Assert
        Assert.Null(result); // Se nenhum cliente válido for inserido, retorna default (null)

        // Verifica se a notificação para cliente inválido foi chamada
        _notificationServiceMock.Verify(n => n.AddNotifications(It.IsAny<ValidationResult>()), Times.Exactly(newCustomers.Count));
    }

    [Fact]
    public void AddCustomers_DoesNotAdd_WhenDuplicateId()
    {
        // Arrange
        // Insere um cliente existente com Id = 20
        var existingCustomer = new Customer { Id = 20, FirstName = "Existing", LastName = "Customer", Age = 40 };
        _db.Customers.Add(existingCustomer);
        _db.SaveChanges();

        // Novo cliente com Id duplicado
        var newCustomers = new List<Customer>
        {
            new() { Id = 20, FirstName = "New", LastName = "Customer", Age = 30 }
        };

        // Configura o validador para retornar resultado válido
        _validatorMock.Setup(v => v.Validate(It.IsAny<Customer>())).Returns(new ValidationResult());

        // Act
        var result = _customerService.AddCustomers(newCustomers);

        // Assert
        Assert.Null(result);

        // Verifica se a notificação de duplicidade foi chamada
        _notificationServiceMock.Verify(n => n.AddNotification("20", "Id in use."), Times.Once);
    }

    #endregion
}
