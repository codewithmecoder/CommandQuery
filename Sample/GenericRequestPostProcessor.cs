using CommandQuery.PostRequest;

namespace Sample;

public class GenericRequestPostProcessor<TRequest, TResponse>(ILogger<GenericRequestPostProcessor<TRequest, TResponse>> logger)
    : IRequestPostProcessor<TRequest, TResponse> where TRequest : notnull
{
    public async Task Process(TRequest request, TResponse response, CancellationToken cancellationToken)
    {
        logger.LogDebug("- All Done GenericRequestPostProcessor<TRequest, TResponse>");
        await Task.CompletedTask.ConfigureAwait(false);
    }
}

public class GenericRequestPostProcessor<TRequest>(ILogger<GenericRequestPostProcessor<TRequest>> logger)
    : IRequestPostProcessor<TRequest> where TRequest : notnull
{
    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        logger.LogDebug("- All Done GenericRequestPostProcessor<TRequest>");
        await Task.CompletedTask.ConfigureAwait(false);
    }
}