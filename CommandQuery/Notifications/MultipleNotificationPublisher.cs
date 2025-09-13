namespace CommandQuery.Notifications;

/// <summary>
/// 
/// </summary>
public class MultipleNotificationPublisher : INotificationPublisher
{
    private readonly TaskWhenAllPublisher _taskWhenAllPublisher = new();
    private readonly ForeachAwaitPublisher _foreachAwaitPublisher = new();
    private readonly PriorityNotificationPublisher _priorityNotificationPublisher = new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handlerExecutors"></param>
    /// <param name="notification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task Publish(
        IEnumerable<NotificationHandlerExecutor> handlerExecutors,
        INotification notification,
        CancellationToken cancellationToken
    )
    {
        switch (notification)
        {
            case IPriorityNotification:
                await _priorityNotificationPublisher.Publish(handlerExecutors, notification, cancellationToken);
                break;
            case IParallelNotification:
                await _taskWhenAllPublisher
                    .Publish(handlerExecutors, notification, cancellationToken)
                    .ConfigureAwait(false);
                break;
            default:
                await _foreachAwaitPublisher.Publish(handlerExecutors, notification, cancellationToken);
                break;
        }
    }
}