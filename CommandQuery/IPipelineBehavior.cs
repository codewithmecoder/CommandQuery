namespace CommandQuery;

/// <summary>
/// Represents a delegate that handles a request and returns a response asynchronously.
/// </summary>
/// <typeparam name="TResponse"></typeparam>
/// <param name="cancellationToken"></param>
/// <returns></returns>
public delegate Task<TResponse> RequestHandlerDelegate<TResponse>(CancellationToken cancellationToken = default);

/// <summary>
/// Request handler delegate for handling requests without a response type.
/// </summary>
/// <param name="cancellationToken"></param>
/// <returns></returns>
public delegate Task RequestHandlerDelegate(CancellationToken cancellationToken = default);

/// <summary>
/// IPipelineBehavior interface for handling requests in a pipeline.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : notnull
{
    /// <summary>
    /// Handle the request by invoking the next handler in the pipeline and processing the response.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="next"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
}
