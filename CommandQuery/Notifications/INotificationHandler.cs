namespace CommandQuery.Notifications;

/// <summary>
/// INotification interface for notifications in the CommandQuery library.
/// </summary>
/// <typeparam name="TNotification"></typeparam>
public interface INotificationHandler<in TNotification>
    where TNotification : INotification
{
    /// <summary>
    /// Handle a notification of type TNotification.
    /// </summary>
    /// <param name="notification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task Handle(TNotification notification, CancellationToken cancellationToken);
}