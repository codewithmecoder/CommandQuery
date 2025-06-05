using CommandQuery.PreRequest;

namespace Sample;

public class GenericRequestPreProcessor<TRequest>(ILogger<GenericRequestPreProcessor<TRequest>> logger) : IRequestPreProcessor<TRequest>
    where TRequest : notnull
{
    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        logger.LogDebug("- Starting Up");
        await Task.CompletedTask;
    }
}