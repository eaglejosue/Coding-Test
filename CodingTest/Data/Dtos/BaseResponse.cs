namespace Api.Data.Dtos;

public class BaseResponse<T>(T? data, IReadOnlyCollection<InternalNotification>? messages = null) where T : class
{
    public T? Data { get; set; } = data;
    public IReadOnlyCollection<InternalNotification>? Messages { get; set; } = messages;
}

public static class BaseResponse
{
    public static BaseResponse<T> New<T>(T? data = null, IReadOnlyCollection<InternalNotification>? messages = null) where T : class => new(data, messages);
}
