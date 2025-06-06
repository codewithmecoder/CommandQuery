namespace CommandQuery.PreRequest;

/// <summary>
/// RequestPreProcessorBehavior is a pipeline behavior that processes requests before they are handled.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
/// <param name="preProcessors"></param>
public class RequestPreProcessorBehavior<TRequest, TResponse>(IEnumerable<IRequestPreProcessor<TRequest>> preProcessors)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <summary>
    /// Handle the request by invoking the next handler in the pipeline and then processing the request with registered pre-processors.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="next"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        foreach (var processor in preProcessors)
        {
            await processor.Process(request, cancellationToken).ConfigureAwait(false);
        }

        return await next(cancellationToken).ConfigureAwait(false);
    }
}