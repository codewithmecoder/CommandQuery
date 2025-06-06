namespace CommandQuery.PostRequest;

/// <summary>
/// RequestPostProcessorBehavior is a pipeline behavior that processes requests after they have been handled.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
/// <param name="postProcessors"></param>
public class RequestPostProcessorBehavior<TRequest, TResponse>(IEnumerable<IRequestPostProcessor<TRequest, TResponse>> postProcessors)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{


    /// <summary>
    /// Handle the request by invoking the next handler in the pipeline and then processing the response with registered post-processors.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="next"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var response = await next(cancellationToken).ConfigureAwait(false);

        foreach (var processor in postProcessors)
        {
            await processor.Process(request, response, cancellationToken).ConfigureAwait(false);
        }

        return response;
    }
}
