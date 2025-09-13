namespace CommandQuery.Notifications;

/// <summary>
/// 
/// </summary>
public class PriorityNotificationPublisher : INotificationPublisher
{
    private const int DefaultPriority = 99;

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
        var lookUp = handlerExecutors
            .ToLookup(key => GetPriority(key.HandlerInstance), value => value)
            .OrderBy(k => k.Key);

        foreach (var handler in lookUp)
        {
            foreach (var notificationHandler in handler.ToList())
            {
                await notificationHandler
                    .HandlerCallback(notification, cancellationToken)
                    .ConfigureAwait(false);
            }
        }
    }

    private static int GetPriority(object handler)
    {
        var priority = handler
            .GetType()
            .GetProperties().ToList()
            .Find(t =>
                t.Name == nameof(IPriorityNotificationHandler<IPriorityNotification>.Priority)
            );

        return priority == null ? DefaultPriority : int.Parse(priority.GetValue(handler)?.ToString() ?? DefaultPriority.ToString());
    }
}