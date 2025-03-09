namespace CodingTest.Services;

public record InternalNotification(
    string? Key,
    string? Message,
    HttpStatusCode? HttpStatusCode = null
);

public interface IInternalNotificationService
{
    bool HasNotifications { get; }
    IReadOnlyCollection<InternalNotification> Notifications { get; }

    void AddNotification(InternalNotification notification);
    void AddNotification(string key, string message);
    void AddNotifications(IList<InternalNotification> notifications);
    void AddNotifications(FluentValidation.Results.ValidationResult validationResult);
    void ClearNotifications();
}

public sealed class InternalNotificationService : IInternalNotificationService
{
    private readonly List<InternalNotification> _notifications;
    public IReadOnlyCollection<InternalNotification> Notifications => _notifications;
    public bool HasNotifications => _notifications.Count > 0;

    public InternalNotificationService()
    {
        _notifications = [];
    }

    public void AddNotification(string key, string message) => _notifications.Add(new InternalNotification(key, message));

    public void AddNotification(InternalNotification notification) => _notifications.Add(notification);

    public void AddNotifications(IList<InternalNotification> notifications) => _notifications.AddRange(notifications);

    public void AddNotifications(FluentValidation.Results.ValidationResult validationResult) =>
        _notifications.AddRange((from e in validationResult.Errors select new InternalNotification(e.PropertyName, e.ErrorMessage)).ToList());

    public void ClearNotifications() => _notifications.Clear();
}

