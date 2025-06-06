using CommandQuery.Notifications;

namespace CommandQuery;

/// <summary>
/// CommandQuery interface for sending commands, queries, and notifications.
/// </summary>
public interface ICommandQuery
{
    /// <summary>
    /// Send a request and get a response.
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send a request without expecting a response.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest;

    /// <summary>
    /// Push a notification to all registered handlers.
    /// </summary>
    /// <typeparam name="TNotification"></typeparam>
    /// <param name="notification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification;
}