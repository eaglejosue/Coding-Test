namespace Api.Services;

public interface IAtmService
{
    Task<List<string>> GetPayoutCombinationsAsync(int amount);
}

public class AtmService(
    IInternalNotificationService notificationService) : IAtmService
{
    private static readonly int[] Denominations = [100, 50, 10];

    public Task<List<string>> GetPayoutCombinationsAsync(int amount)
    {
        var results = new HashSet<string>();
        FindCombinations(amount, [], results);

        if (results.Count == 0)
        {
            var closestAmount = FindClosestAmount(amount);
            FindCombinations(closestAmount, [], results);

            if (results.Count > 0)
                notificationService.AddNotification("Note", $"You requested {amount} EUR, but the closest possible payout is {closestAmount} EUR.");
        }

        return Task.FromResult(results.Reverse().ToList());
    }

    private static void FindCombinations(int amount, Dictionary<int, int> current, HashSet<string> results, int index = 0)
    {
        if (amount == 0)
        {
            results.Add(FormatCombination(current));
            return;
        }

        for (int i = index; i < Denominations.Length; i++)
        {
            if (amount >= Denominations[i])
            {
                if (!current.ContainsKey(Denominations[i]))
                    current[Denominations[i]] = 0;

                current[Denominations[i]]++;
                FindCombinations(amount - Denominations[i], current, results, i);

                if (current[Denominations[i]] == 1)
                    current.Remove(Denominations[i]);
                else
                    current[Denominations[i]]--;
            }
        }
    }

    private static int FindClosestAmount(int amount)
    {
        int remainder = amount % 10;
        return remainder == 0 ? amount : amount - remainder;
    }

    private static string FormatCombination(Dictionary<int, int> combination) =>
        string.Join(" + ", combination.OrderByDescending(x => x.Key).Select(g => $"{g.Value} x {g.Key} EUR"));
}
