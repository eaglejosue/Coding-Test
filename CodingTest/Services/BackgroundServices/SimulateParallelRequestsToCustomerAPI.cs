namespace CodingTest.Services.BackgroundServices;

public class SimulateParallelRequestsToCustomerApi(
    IConfiguration configuration,
    ILogger<SimulateParallelRequestsToCustomerApi> logger) : BackgroundService
{
    private static readonly HttpClient _httpClient = new();
    private readonly int _delayInSeconds = configuration.GetValue<int?>("BackgroundService:DelayInSeconds") ?? 5;
    private readonly int _runQtd = configuration.GetValue<int?>("BackgroundService:RunQtd") ?? 10;
    private int _executionCount = 0;
    private DateTime _lastExecutionDate = DateTime.UtcNow.Date;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("SimulateParallelRequestsToCustomerAPI started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (_executionCount >= _runQtd && _lastExecutionDate == DateTime.UtcNow.Date)
                {
                    logger.LogInformation("Maximum executions reached for today. Waiting for next day.");
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                    continue;
                }

                if (_lastExecutionDate != DateTime.UtcNow.Date)
                {
                    _executionCount = 0;
                    _lastExecutionDate = DateTime.UtcNow.Date;
                }

                var customers = GenerateRandomCustomers(2);
                var customersString = JsonSerializer.Serialize(customers);
                logger.LogInformation(string.Concat("POST Customers: ", customersString));

                var content = new StringContent(customersString, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("http://localhost:5000/api/customer", content, stoppingToken);

                if (response.IsSuccessStatusCode)
                    logger.LogInformation("Customers added successfully.");
                else
                    logger.LogInformation("Failed to add customers.");

                logger.LogInformation(string.Concat("POST Customers - StatusCode: ", response.StatusCode));
                var responseContent = await response.Content.ReadAsStringAsync(stoppingToken);
                logger.LogInformation(string.Concat("POST Customers - Result: ", responseContent));

                var getResponse = await _httpClient.GetStringAsync("http://localhost:5000/api/customer", stoppingToken);
                logger.LogInformation(string.Concat("GET Customers - Result: ", getResponse));

                _executionCount++;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error SimulateParallelRequestsToCustomerAPI.");
            }
            finally
            {
                await Task.Delay(TimeSpan.FromMinutes(_delayInSeconds), stoppingToken);
            }
        }

        logger.LogInformation("SimulateParallelRequestsToCustomerAPI is stopping.");
    }

    private static List<Customer> GenerateRandomCustomers(int count)
    {
        var firstNames = new[] { "Leia", "Sadie", "Jose", "Sara", "Frank", "Dewey", "Tomas", "Joel", "Lukas", "Carlos" };
        var lastNames = new[] { "Liberty", "Ray", "Harrison", "Ronan", "Drew", "Powell", "Larsen", "Chan", "Anderson", "Lane" };
        var random = new Random();
        var customers = new List<Customer>();

        var idRandon = random.Next(1, 100);

        for (int i = 0; i < count; i++)
        {
            var customer = new Customer
            {
                Id = i + idRandon,
                FirstName = firstNames[random.Next(firstNames.Length)],
                LastName = lastNames[random.Next(lastNames.Length)],
                Age = random.Next(10, 90)
            };

            customers.Add(customer);
        }

        return customers;
    }
}
