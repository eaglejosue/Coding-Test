namespace CodingTest.Services;

public interface ICustomerService
{
    Task<List<Customer>> GetAllAsync(CustomerFilters filters);
    IList<Customer>? AddCustomers(List<Customer> customers);
}

public class CustomerService(
    DbCodingTest db,
    IValidator<Customer> addValidator,
    IInternalNotificationService notificationService) : ICustomerService
{
    public async Task<List<Customer>> GetAllAsync(CustomerFilters filters)
    {
        var predicate = PredicateBuilder.New<Customer>(true);

        if (filters.Id.HasValue)
            predicate.And(a => a.Id == filters.Id);

        if (!string.IsNullOrEmpty(filters.Name))
        {
            predicate.And(a =>
                EF.Functions.Like(a.FirstName, filters.Name.LikeConcat()) ||
                EF.Functions.Like(a.LastName, filters.Name.LikeConcat())
            );
        }

        if (filters.Age.HasValue)
            predicate.And(a => a.Age == filters.Age);

        var query = db.Customers.Where(predicate);

#if DEBUG
        var queryString = query.ToQueryString();
#endif

        var customers = await query.ToListAsync();

        return customers;
    }

    private static readonly object _lock = new();

    public IList<Customer>? AddCustomers(List<Customer> customers)
    {
        lock (_lock)
        {
            using var transaction = db.Database.BeginTransaction();

            try
            {
                var customerIds = customers.Select(s => s.Id);
                var existingCustomers = db.Customers.ToList();

                var existingCustomerIdsHash = existingCustomers.Select(s => s.Id).ToHashSet();

                var validCustomers = new List<Customer>(0);
                foreach (var customer in customers)
                {
                    var validation = addValidator.Validate(customer);
                    if (!validation.IsValid)
                    {
                        notificationService.AddNotifications(validation);
                        continue;
                    }

                    if (existingCustomerIdsHash.Contains(customer.Id))
                    {
                        notificationService.AddNotification(customer.Id.ToString(), "Id in use.");
                        continue;
                    }

                    validCustomers.Add(customer);
                }

                var allCustomers = existingCustomers.Concat(validCustomers).ToList();
                var sortedCustomers = SortCustomers(allCustomers);

                db.Customers.RemoveRange(existingCustomers);
                db.Customers.AddRange(sortedCustomers);
                db.SaveChanges();

                transaction.Commit();

                return allCustomers;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                notificationService.AddNotification(new InternalNotification("Error", ex.Message, HttpStatusCode.BadRequest));
            }
        }

        return default;
    }

    private static List<Customer> SortCustomers(List<Customer> customers)
    {
        for (int i = 0; i < customers.Count - 1; i++)
        {
            for (int j = i + 1; j < customers.Count; j++)
            {
                int lastNameComparison = string.Compare(customers[i].LastName, customers[j].LastName);
                if (lastNameComparison > 0 || (lastNameComparison == 0 && string.Compare(customers[i].FirstName, customers[j].FirstName) > 0))
                {
                    var temp = customers[i];
                    customers[i] = customers[j];
                    customers[j] = temp;
                }
            }
        }

        return customers;
    }
}
