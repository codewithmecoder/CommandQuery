namespace Bas24.CommandQuery.PostRequest;

/// <summary>
/// IRequestPostProcessor interface for processing requests after they have been handled.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public interface IRequestPostProcessor<in TRequest, in TResponse> where TRequest : notnull
{
    /// <summary>
    /// Process a request after it has been handled and a response has been generated.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="response"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task Process(TRequest request, TResponse response, CancellationToken cancellationToken);
}