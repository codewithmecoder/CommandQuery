using Microsoft.Extensions.DependencyInjection;

namespace CommandQuery.Wrappers;

/// <summary>
/// Request handler base class for handling requests in CommandQuery framework.
/// </summary>
public abstract class RequestHandlerBase
{
    /// <summary>
    /// Handle a request using the provided service provider and cancellation token.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="serviceProvider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task<object?> Handle(object request, IServiceProvider serviceProvider, CancellationToken cancellationToken);
}

/// <summary>
/// Request handler wrapper base class for handling requests with a response type.
/// </summary>
/// <typeparam name="TResponse"></typeparam>
public abstract class RequestHandlerWrapper<TResponse> : RequestHandlerBase
{
    /// <summary>
    /// Handle a request of type <see cref="IRequest{TResponse}"/> using the provided service provider and cancellation token.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="serviceProvider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task<TResponse> Handle(IRequest<TResponse> request, IServiceProvider serviceProvider, CancellationToken cancellationToken);
}

/// <summary>
/// Request handler wrapper base class for handling requests without a response type.
/// </summary>
public abstract class RequestHandlerWrapper : RequestHandlerBase
{
    /// <summary>
    /// Handle a request of type <see cref="IRequest"/> using the provided service provider and cancellation token.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="serviceProvider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public abstract Task<VoidValue> Handle(IRequest request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}


/// <summary>
/// Request handler wrapper implementation for handling requests of type <see cref="IRequest{TResponse}"/>.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class RequestHandlerWrapperImpl<TRequest, TResponse> : RequestHandlerWrapper<TResponse>
    where TRequest : IRequest<TResponse>
{

    /// <summary>
    /// Handle a request of type <see cref="IRequest{TResponse}"/> using the provided service provider and cancellation token.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="serviceProvider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<object?> Handle(object request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken) =>
        await Handle((IRequest<TResponse>)request, serviceProvider, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Handle a request of type <see cref="IRequest{TResponse}"/> using the provided service provider and cancellation token.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="serviceProvider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override Task<TResponse> Handle(IRequest<TResponse> request, IServiceProvider serviceProvider,
       CancellationToken cancellationToken)
    {
        return serviceProvider
            .GetServices<IPipelineBehavior<TRequest, TResponse>>()
            .Reverse()
            .Aggregate((RequestHandlerDelegate<TResponse>)Handler,
                (next, pipeline) => 
                    t => pipeline.Handle((TRequest)request, next,
                        t == CancellationToken.None ? cancellationToken : t))(cancellationToken);

        Task<TResponse> Handler(CancellationToken ct = default) => 
            serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>()
            .Handle((TRequest)request, cancellationToken);
    }
}

/// <summary>
/// Request handler wrapper implementation for handling requests of type <see cref="IRequest"/> without a response type.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
public class RequestHandlerWrapperImpl<TRequest> : RequestHandlerWrapper
    where TRequest : IRequest
{
    /// <summary>
    /// Handle a request of type <see cref="IRequest"/> using the provided service provider and cancellation token.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="serviceProvider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task<object?> Handle(object request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken) =>
        await Handle((IRequest)request, serviceProvider, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Handle a request of type <see cref="IRequest"/> using the provided service provider and cancellation token.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="serviceProvider"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override Task<VoidValue> Handle(IRequest request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        return serviceProvider
            .GetServices<IPipelineBehavior<TRequest, VoidValue>>()
            .Reverse()
            .Aggregate((RequestHandlerDelegate<VoidValue>)Handler,
                (next, pipeline) => 
                    t => pipeline.Handle((TRequest)request, next,
                        t == CancellationToken.None ? cancellationToken : t))(cancellationToken);

        async Task<VoidValue> Handler(CancellationToken ct = default)
        {
            await serviceProvider.GetRequiredService<IRequestHandler<TRequest>>()
                .Handle((TRequest)request, ct == CancellationToken.None ? cancellationToken : ct);

            return VoidValue.Value;
        }
    }
}