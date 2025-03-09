namespace CodingTest.Services;

public interface ICustomerService
{
    Task<List<Customer>> GetAllAsync(CustomerFilters filters);
    Task AddCustomersAsync(List<Customer> models);
}

public class CustomerService(
    IInternalNotificationService notificationService) : ICustomerService
{
    public string AddCustomers(List<Customer> newCustomers)
    {
        foreach (var customer in newCustomers)
        {
            if (string.IsNullOrEmpty(customer.FirstName) || string.IsNullOrEmpty(customer.LastName) || customer.Age < 18)
                return "Invalid customer data.";

            if (_customers.Any(c => c.Id == customer.Id))
                return "Duplicate customer ID.";

            customer.Id = _nextId++;

            InsertSorted(customer);
        }

        return "Customers added successfully.";
    }
    private void InsertSorted(Customer customer)
    {
        int index = 0;
        while (index < _customers.Count &&
               (string.Compare(_customers[index].LastName, customer.LastName) < 0 ||
               (string.Compare(_customers[index].LastName, customer.LastName) == 0 &&
                string.Compare(_customers[index].FirstName, customer.FirstName) < 0)))
        {
            index++;
        }

        _customers.Insert(index, customer);
    }

    public async Task<List<Customer>> GetAllAsync(CustomerFilters filters)
    {
        throw new NotImplementedException();
    }

    public async Task AddCustomersAsync(List<Customer> models)
    {
        throw new NotImplementedException();
    }
}
