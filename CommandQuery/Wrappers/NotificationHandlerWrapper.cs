using Bas24.CommandQuery.Notifications;
using Microsoft.Extensions.DependencyInjection;

namespace Bas24.CommandQuery.Wrappers;

/// <summary>
/// Notification handler wrapper base class.
/// </summary>
public abstract class NotificationHandlerWrapper
{
    /// <summary>
    /// Handle a notification using the provided service factory and publish method.
    /// </summary>
    /// <param name="notification"></param>
    /// <param name="serviceFactory"></param>
    /// <param name="publish"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task Handle(INotification notification, IServiceProvider serviceFactory,
        Func<IEnumerable<NotificationHandlerExecutor>, INotification, CancellationToken, Task> publish,
        CancellationToken cancellationToken);
}

/// <summary>
/// Notification handler wrapper implementation for a specific notification type.
/// </summary>
/// <typeparam name="TNotification"></typeparam>
public class NotificationHandlerWrapperImpl<TNotification> : NotificationHandlerWrapper
    where TNotification : INotification
{
    /// <summary>
    /// Handle a notification of type TNotification using the provided service factory and publish method.
    /// </summary>
    /// <param name="notification"></param>
    /// <param name="serviceFactory"></param>
    /// <param name="publish"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override Task Handle(INotification notification, IServiceProvider serviceFactory,
        Func<IEnumerable<NotificationHandlerExecutor>, INotification, CancellationToken, Task> publish,
        CancellationToken cancellationToken)
    {
        var handlers = serviceFactory
            .GetServices<INotificationHandler<TNotification>>()
            .Select(static x => new NotificationHandlerExecutor(x, (theNotification, theToken) => x.Handle((TNotification)theNotification, theToken)));

        return publish(handlers, notification, cancellationToken);
    }
}