namespace Bas24.CommandQuery;

/// <summary>
/// IRequest interface for defining a request in the CommandQuery framework.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{

    /// <summary>
    /// Handle the request and return a response asynchronously.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}


/// <summary>
/// IRequestHandler interface for handling requests without a response type in the CommandQuery framework.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
public interface IRequestHandler<in TRequest>
    where TRequest : IRequest
{

    /// <summary>
    /// Handle the request without returning a response asynchronously.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task Handle(TRequest request, CancellationToken cancellationToken);
}