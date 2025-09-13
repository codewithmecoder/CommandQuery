using Bas24.CommandQuery;

namespace Sample;

public class RequestTestReturn : IRequest<string>
{
    public string Id { get; set; } = string.Empty;
}

public class RequestTestReturnHandler : IRequestHandler<RequestTestReturn, string>
{
    public async Task<string> Handle(RequestTestReturn request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"RequestTestReturnHandler: {request.Id}");
        return await Task.FromResult("RequestTestReturnHandler");
    }
}

public class RequestTest: IRequest
{
    public string Id { get; set; } = string.Empty;
}

public class RequestTestHandler : IRequestHandler<RequestTest>
{
    public async Task Handle(RequestTest request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"RequestTestReturnHandler: {request.Id}");
        await Task.CompletedTask;
    }
}