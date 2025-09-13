using System.Text.Json;
using Bas24.CommandQuery;

namespace Sample;

public class LoggingPipelineBehavior<TRequest, TResponse>(ILogger<LoggingPipelineBehavior<TRequest, TResponse>> logger) 
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var req = JsonSerializer.Serialize(request);
        logger.LogDebug("Request: {Req}", req);
        var response = await next(cancellationToken);
        var res = JsonSerializer.Serialize(response);
        logger.LogDebug("Response: {Res}", res);
        return response;
    }
}