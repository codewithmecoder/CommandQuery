namespace CommandQuery.Notifications;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TNotification"></typeparam>
public interface IPriorityNotificationHandler<in TNotification>
    : INotificationHandler<TNotification>
    where TNotification : IPriorityNotification
{
    /// <summary>
    /// 
    /// </summary>
    public int Priority { get; }
}