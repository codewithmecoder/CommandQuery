namespace CommandQuery.PostRequest;

public class RequestPostProcessorBehavior<TRequest, TResponse>(IEnumerable<IRequestPostProcessor<TRequest, TResponse>> postProcessors)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
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

public class RequestPostProcessorBehavior<TRequest>(IEnumerable<IRequestPostProcessor<TRequest>> postProcessors)
    : IPipelineBehavior<TRequest>
    where TRequest : notnull
{
    public async Task Handle(TRequest request, RequestHandlerDelegate next, CancellationToken cancellationToken)
    {
        await next(cancellationToken).ConfigureAwait(false);

        foreach (var processor in postProcessors)
        {
            await processor.Process(request, cancellationToken).ConfigureAwait(false);
        }
    }
}