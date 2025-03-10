namespace CodingTest.BackgroundServices;

public class SimulateParallelRequestsToCustomerAPI(
    IConfiguration configuration,
    ILogger<SimulateParallelRequestsToCustomerAPI> logger) : BackgroundService
{
    private static readonly HttpClient _httpClient = new();
    private readonly int _delayInSeconds = configuration.GetValue<int?>("BackgroundService:DelayInSeconds") ?? 5;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("SimulateParallelRequestsToCustomerAPI started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var customers = GenerateRandomCustomers(2); // Generate 2 customers per request

                // Simulate POST request
                var content = new StringContent(JsonSerializer.Serialize(customers), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("http://localhost:5000/api/customer", content, stoppingToken);

                if (response.IsSuccessStatusCode)
                    Console.WriteLine("Customers added successfully.");
                else
                    Console.WriteLine("Failed to add customers.");

                // Simulate GET request to see all customers
                var getResponse = await _httpClient.GetStringAsync("http://localhost:5000/api/customer", stoppingToken);
                Console.WriteLine(getResponse);
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

        for (int i = 0; i < count; i++)
        {
            var firstName = firstNames[random.Next(firstNames.Length)];
            var lastName = lastNames[random.Next(lastNames.Length)];
            var age = random.Next(10, 90);
            var customer = new Customer
            {
                FirstName = firstName,
                LastName = lastName,
                Age = age
            };

            customers.Add(customer);
        }

        return customers;
    }
}
