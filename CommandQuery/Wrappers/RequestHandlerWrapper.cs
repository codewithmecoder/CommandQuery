using Microsoft.Extensions.DependencyInjection;

namespace CommandQuery.Wrappers;

public abstract class RequestHandlerBase
{
    public abstract Task<object?> Handle(object request, IServiceProvider serviceProvider, CancellationToken cancellationToken);
}

public abstract class RequestHandlerWrapper<TResponse> : RequestHandlerBase
{
    public abstract Task<TResponse> Handle(IRequest<TResponse> request, IServiceProvider serviceProvider, CancellationToken cancellationToken);
}

public abstract class RequestHandlerWrapper : RequestHandlerBase
{
    public abstract Task<VoidValue> Handle(IRequest request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken);
}

public class RequestHandlerWrapperImpl<TRequest, TResponse> : RequestHandlerWrapper<TResponse>
    where TRequest : IRequest<TResponse>
{
    public override async Task<object?> Handle(object request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken) =>
        await Handle((IRequest<TResponse>)request, serviceProvider, cancellationToken).ConfigureAwait(false);

    public override Task<TResponse> Handle(IRequest<TResponse> request, IServiceProvider serviceProvider,
       CancellationToken cancellationToken)
    {
        return serviceProvider
            .GetServices<IPipelineBehavior<TRequest, TResponse>>()
            .Reverse()
            .Aggregate((RequestHandlerDelegate<TResponse>)Handler,
                (next, pipeline) => 
                    t => pipeline.Handle((TRequest)request, next,
                        t == CancellationToken.None ? cancellationToken : t))(cancellationToken);

        Task<TResponse> Handler(CancellationToken ct = default) => 
            serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>()
            .Handle((TRequest)request, cancellationToken);
    }
}

public class RequestHandlerWrapperImpl<TRequest> : RequestHandlerWrapper
    where TRequest : IRequest
{
    public override async Task<object?> Handle(object request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken) =>
        await Handle((IRequest)request, serviceProvider, cancellationToken).ConfigureAwait(false);

    public override Task<VoidValue> Handle(IRequest request, IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        return serviceProvider
            .GetServices<IPipelineBehavior<TRequest, VoidValue>>()
            .Reverse()
            .Aggregate((RequestHandlerDelegate<VoidValue>)Handler,
                (next, pipeline) => 
                    t => pipeline.Handle((TRequest)request, next,
                        t == CancellationToken.None ? cancellationToken : t))(cancellationToken);

        async Task<VoidValue> Handler(CancellationToken ct = default)
        {
            await serviceProvider.GetRequiredService<IRequestHandler<TRequest>>()
                .Handle((TRequest)request, ct == CancellationToken.None ? cancellationToken : ct);

            return VoidValue.Value;
        }
    }
}