namespace CommandQuery.Notifications;

/// <summary>
/// Awaits each notification handler in a single foreach loop:
/// <code>
/// foreach (var handler in handlers) {
///     await handler(notification, cancellationToken);
/// }
/// </code>
/// </summary>
public class ForeachAwaitPublisher : INotificationPublisher
{
    /// <summary>
    /// Publishes a notification to all registered handlers by awaiting each handler in a foreach loop.
    /// </summary>
    /// <param name="handlerExecutors"></param>
    /// <param name="notification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task Publish(IEnumerable<NotificationHandlerExecutor> handlerExecutors, INotification notification, CancellationToken cancellationToken)
    {
        foreach (var handler in handlerExecutors)
        {
            await handler.HandlerCallback(notification, cancellationToken).ConfigureAwait(false);
        }
    }
}