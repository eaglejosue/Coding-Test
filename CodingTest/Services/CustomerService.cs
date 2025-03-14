namespace Api.Services;

public interface ICustomerService
{
    Task<List<Customer>?> GetAllAsync(CustomerFilters filters);
    IList<Customer>? AddCustomers(List<Customer> customers);
}

public class CustomerService(
    DbCoding db,
    ICacheService cacheService,
    IValidator<Customer> addValidator,
    IInternalNotificationService notificationService) : ICustomerService
{
    public async Task<List<Customer>?> GetAllAsync(CustomerFilters filters)
    {
        var cachedCustomers = cacheService.Get<List<Customer>?>(filters.CacheKey());
        if (cachedCustomers != null)
            return cachedCustomers;

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

        cacheService.Set(filters.CacheKey(), customers);

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

                if (validCustomers.Count == 0)
                    return default;

                var allCustomers = existingCustomers.Concat(validCustomers).ToList();
                var sortedCustomers = MergeSort(allCustomers);

                db.Customers.RemoveRange(existingCustomers);
                db.SaveChanges();

                db.Customers.AddRange(sortedCustomers);
                db.SaveChanges();

                transaction.Commit();

                cacheService.ClearAll();

                return sortedCustomers;
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                notificationService.AddNotification(new InternalNotification("Error", ex.Message, HttpStatusCode.BadRequest));
            }
        }

        return default;
    }

    private static List<Customer> MergeSort(List<Customer> customers)
    {
        if (customers.Count <= 1)
            return customers;

        int mid = customers.Count / 2;
        List<Customer> left = customers.GetRange(0, mid);
        List<Customer> right = customers.GetRange(mid, customers.Count - mid);

        return Merge(MergeSort(left), MergeSort(right));
    }

    private static List<Customer> Merge(List<Customer> left, List<Customer> right)
    {
        List<Customer> result = [];
        int i = 0, j = 0;

        while (i < left.Count && j < right.Count)
        {
            if (CompareCustomers(left[i], right[j]) <= 0)
                result.Add(left[i++]);
            else
                result.Add(right[j++]);
        }

        result.AddRange(left.GetRange(i, left.Count - i));
        result.AddRange(right.GetRange(j, right.Count - j));

        return result;
    }

    private static int CompareCustomers(Customer a, Customer b)
    {
        int lastNameComparison = string.Compare(a.LastName, b.LastName);
        if (lastNameComparison != 0)
            return lastNameComparison;

        return string.Compare(a.FirstName, b.FirstName);
    }
}
