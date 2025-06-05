using System.Reflection;
using CommandQuery.Notifications;
using CommandQuery.PostRequest;
using CommandQuery.PreRequest;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CommandQuery;

public static class CommandQueryExtensions
{
    public static IServiceCollection AddCommandQuery(this IServiceCollection services, Assembly assembly)
    {
        // Register mediator
        services.AddScoped<ICommandQuery, CommandQuery>();

        // Register handlers
        RegisterHandlers(services, assembly);

        return services;
    }

    private static void RegisterHandlers(IServiceCollection services, Assembly assembly)
    {
        services.TryAdd(new ServiceDescriptor(typeof(ICommandQuery), typeof(CommandQuery), ServiceLifetime.Transient));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));

        services.Scan(scan => scan
            .FromAssemblies(assembly)

            // Register IRequestHandler<TRequest, TResponse>
            .AddClasses(classes => classes.AssignableTo(typeof(IRequestHandler<,>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime()

            // Register IRequestHandler<TRequest, TResponse>
            .AddClasses(classes => classes.AssignableTo(typeof(IRequestHandler<,>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime()

            // Register IRequestHandler<TRequest>
            .AddClasses(classes => classes.AssignableTo(typeof(IRequestHandler<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime()

            // Register INotificationHandler<TNotification>
            .AddClasses(classes => classes.AssignableTo(typeof(INotificationHandler<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime()
        );
    }
}