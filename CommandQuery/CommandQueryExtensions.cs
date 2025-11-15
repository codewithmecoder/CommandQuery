using Bas24.CommandQuery.Notifications;
using Bas24.CommandQuery.PostRequest;
using Bas24.CommandQuery.PreRequest;
using Bas24.CommandQuery.ServiceRegisters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Bas24.CommandQuery;

/// <summary>
/// Service extensions for CommandQuery library.
/// </summary>
public static class CommandQueryExtensions
{
    /// <summary>
    /// Add CommandQuery services to the service collection with default configuration.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddCommandQuery(this IServiceCollection services, Action<CommandQueryConfig> configuration)
    {
        var serviceConfig = new CommandQueryConfig();

        configuration.Invoke(serviceConfig);

        return services.AddCommandQuery(serviceConfig);
    }

    /// <summary>
    /// Add CommandQuery services to the service collection with specified configuration.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static IServiceCollection AddCommandQuery(this IServiceCollection services,
        CommandQueryConfig configuration)
    {
        if (configuration.AssembliesToRegister.Count <= 0)
        {
            throw new ArgumentException("No assemblies found to scan. Supply at least one assembly to scan for handlers.");
        }
        
        AddRequiredServices(services, configuration);

        return services;
    }

    /// <summary>
    /// Add required services to the service collection based on the provided configuration.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    public static void AddRequiredServices(IServiceCollection services, CommandQueryConfig configuration)
    {
        services.AddScoped<ICommandQuery, CommandQuery>();

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPostProcessorBehavior<,>));

        if (configuration.RequestPreProcessorsToRegister.Count > 0)
        {
            services.TryAddEnumerable(configuration.RequestPreProcessorsToRegister);
        }

        if (configuration.RequestPostProcessorsToRegister.Count > 0)
        {
            services.TryAddEnumerable(configuration.RequestPostProcessorsToRegister);
        }

        foreach (var serviceDescriptor in configuration.BehaviorsToRegister)
        {
            services.TryAddEnumerable(serviceDescriptor);
        }

        var notificationPublisherServiceDescriptor = configuration.NotificationPublisherType != null
            ? new ServiceDescriptor(typeof(INotificationPublisher), configuration.NotificationPublisherType, configuration.Lifetime)
            : new ServiceDescriptor(typeof(INotificationPublisher), configuration.NotificationPublisher);

        services.TryAdd(notificationPublisherServiceDescriptor);

        services.Scan(scan => scan
            .FromAssemblies(configuration.AssembliesToRegister)
            .AddClasses(classes => classes.AssignableTo(typeof(IRequestHandler<,>)))
                .AsImplementedInterfaces()
                .WithLifetime(configuration.Lifetime)
            .AddClasses(classes => classes.AssignableTo(typeof(IRequestHandler<>)))
                .AsImplementedInterfaces()
                .WithLifetime(configuration.Lifetime)
            .AddClasses(classes => classes.AssignableTo(typeof(INotificationHandler<>)))
                .AsImplementedInterfaces()
                .WithLifetime(configuration.Lifetime)
        );
    }
}