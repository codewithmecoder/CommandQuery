namespace CommandQuery.Notifications;

/// <summary>
/// NotificationHandlerExecutor is a record that encapsulates the handler instance and its callback for executing notifications.
/// </summary>
/// <param name="HandlerInstance"></param>
/// <param name="HandlerCallback"></param>
public record NotificationHandlerExecutor(object HandlerInstance, Func<INotification, CancellationToken, Task> HandlerCallback);