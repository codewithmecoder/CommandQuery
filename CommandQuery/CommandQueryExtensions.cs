using CommandQuery.Notifications;
using CommandQuery.PostRequest;
using CommandQuery.PreRequest;
using CommandQuery.ServiceRegisters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CommandQuery;

public static class CommandQueryExtensions
{

    public static IServiceCollection AddCommandQuery(this IServiceCollection services, Action<CommandQueryConfig> configuration)
    {
        var serviceConfig = new CommandQueryConfig();

        configuration.Invoke(serviceConfig);

        return services.AddCommandQuery(serviceConfig);
    }

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