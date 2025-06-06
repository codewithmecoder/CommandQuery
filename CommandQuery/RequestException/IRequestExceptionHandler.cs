namespace CommandQuery.RequestException;

/// <summary>
/// IRequestExceptionHandler interface for handling exceptions thrown by request handlers.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
/// <typeparam name="TException"></typeparam>
public interface IRequestExceptionHandler<in TRequest, TResponse, in TException>
    where TRequest : notnull
    where TException : Exception
{
    /// <summary>
    /// Called when the request handler throws an exception
    /// </summary>
    /// <param name="request">Request instance</param>
    /// <param name="exception">The thrown exception</param>
    /// <param name="state">The current state of handling the exception</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>An awaitable task</returns>
    Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state, CancellationToken cancellationToken);
}