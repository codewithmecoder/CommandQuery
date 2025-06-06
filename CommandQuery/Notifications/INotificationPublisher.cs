namespace CommandQuery.Notifications;


/// <summary>
/// INotificationPublisher interface for publishing notifications to registered handlers.
/// </summary>
public interface INotificationPublisher
{

    /// <summary>
    /// Push a notification to all registered handlers.
    /// </summary>
    /// <param name="handlerExecutors"></param>
    /// <param name="notification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task Publish(IEnumerable<NotificationHandlerExecutor> handlerExecutors, INotification notification,
        CancellationToken cancellationToken);
}