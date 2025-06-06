using CommandQuery.Notifications;
using CommandQuery.Wrappers;
using System.Collections.Concurrent;

namespace CommandQuery;

/// <summary>
/// CommandQuery implementation for sending commands, queries, and notifications.
/// </summary>
public class CommandQuery : ICommandQuery
{
    private readonly IServiceProvider _serviceProvider;
    private readonly INotificationPublisher _publisher;
    private static readonly ConcurrentDictionary<Type, RequestHandlerBase> RequestHandlers = new();
    private static readonly ConcurrentDictionary<Type, NotificationHandlerWrapper> NotificationHandlers = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandQuery"/> class.
    /// </summary>
    /// <param name="serviceProvider">Service provider. Can be a scoped or root provider</param>
    public CommandQuery(IServiceProvider serviceProvider)
        : this(serviceProvider, new ForeachAwaitPublisher()) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandQuery"/> class.
    /// </summary>
    /// <param name="serviceProvider">Service provider. Can be a scoped or root provider</param>
    /// <param name="publisher">Notification publisher. Defaults to <see cref="ForeachAwaitPublisher"/>.</param>
    public CommandQuery(IServiceProvider serviceProvider, INotificationPublisher publisher)
    {
        _serviceProvider = serviceProvider;
        _serviceProvider = serviceProvider;
        _publisher = publisher;
    }

    /// <summary>
    /// Send a request and get a response.
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var handler = (RequestHandlerWrapper<TResponse>)RequestHandlers.GetOrAdd(request.GetType(), static requestType =>
        {
            var wrapperType = typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(requestType, typeof(TResponse));
            var wrapper = Activator.CreateInstance(wrapperType) ?? throw new InvalidOperationException($"Could not create wrapper type for {requestType}");
            return (RequestHandlerBase)wrapper;
        });

        return await handler.Handle(request, _serviceProvider, cancellationToken);
    }

    /// <summary>
    /// Send a request without expecting a response.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default) where TRequest : IRequest
    {
        ArgumentNullException.ThrowIfNull(request);

        var handler = (RequestHandlerWrapper)RequestHandlers.GetOrAdd(request.GetType(), static requestType =>
        {
            var wrapperType = typeof(RequestHandlerWrapperImpl<>).MakeGenericType(requestType);
            var wrapper = Activator.CreateInstance(wrapperType) ?? throw new InvalidOperationException($"Could not create wrapper type for {requestType}");
            return (RequestHandlerBase)wrapper;
        });

        return handler.Handle(request, _serviceProvider, cancellationToken);
    }

    /// <summary>
    /// Push a notification to all registered handlers.
    /// </summary>
    /// <typeparam name="TNotification"></typeparam>
    /// <param name="notification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
    {
        if (notification == null)
        {
            throw new ArgumentNullException(nameof(notification));
        }

        return PublishNotification(notification, cancellationToken);
    }

    /// <summary>
    /// Override in a derived class to control how the tasks are awaited. By default, the implementation calls the <see cref="INotificationPublisher"/>.
    /// </summary>
    /// <param name="handlerExecutors">Enumerable of tasks representing invoking each notification handler</param>
    /// <param name="notification">The notification being published</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A task representing invoking all handlers</returns>
    protected virtual Task PublishCore(IEnumerable<NotificationHandlerExecutor> handlerExecutors, INotification notification, CancellationToken cancellationToken)
        => _publisher.Publish(handlerExecutors, notification, cancellationToken);

    private Task PublishNotification(INotification notification, CancellationToken cancellationToken = default)
    {
        var handler = NotificationHandlers.GetOrAdd(notification.GetType(), static notificationType =>
        {
            var wrapperType = typeof(NotificationHandlerWrapperImpl<>).MakeGenericType(notificationType);
            var wrapper = Activator.CreateInstance(wrapperType) ?? throw new InvalidOperationException($"Could not create wrapper for type {notificationType}");
            return (NotificationHandlerWrapper)wrapper;
        });

        return handler.Handle(notification, _serviceProvider, PublishCore, cancellationToken);
    }
}