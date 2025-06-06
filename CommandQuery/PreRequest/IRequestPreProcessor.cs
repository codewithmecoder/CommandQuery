namespace CommandQuery.PreRequest;

/// <summary>
/// IRequestPreProcessor interface for processing requests before they are handled.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
public interface IRequestPreProcessor<in TRequest> where TRequest : notnull
{

    /// <summary>
    /// Process a request before it is handled.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task Process(TRequest request, CancellationToken cancellationToken);
}