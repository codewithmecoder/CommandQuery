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


    private static void AddRequiredServices(IServiceCollection services, CommandQueryConfig commandQueryConfig)
    {
        services.AddScoped<ICommandQuery, CommandQuery>();
        services.Scan(scan => scan
            .FromAssemblies(commandQueryConfig.AssembliesToRegister)

            // Register IRequestHandler<TRequest, TResponse>
            .AddClasses(classes => classes.AssignableTo(typeof(IRequestHandler<,>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime()

            // Register IRequestHandler<TRequest>
            .AddClasses(classes => classes.AssignableTo(typeof(IRequestHandler<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime()
        );

        services.Scan(scan => scan
            .FromAssemblies(commandQueryConfig.AssembliesToRegister)
            .AddClasses(classes => classes
                .Where(type => commandQueryConfig.NotificationPublisherType != null &&
                               type == commandQueryConfig.NotificationPublisherType))
            .As<INotificationPublisher>()
            .WithTransientLifetime());

        // Handle factory instance separately if needed
        if (commandQueryConfig is { NotificationPublisher: not null, NotificationPublisherType: null })
        {
            services.TryAdd(new ServiceDescriptor(typeof(INotificationPublisher), commandQueryConfig.NotificationPublisher));
        }

        // Register pre-processors
        if (commandQueryConfig.RequestPreProcessorsToRegister.Count > 0)
        {
            services.TryAddEnumerable(ServiceDescriptor.Describe(
                typeof(IPipelineBehavior<,>),
                typeof(RequestPreProcessorBehavior<,>),
                ServiceLifetime.Transient));

            services.Scan(scan => scan
                .FromAssembliesOf(commandQueryConfig.RequestPreProcessorsToRegister.Select(d => d.ImplementationType).ToArray()!)
                .AddClasses(classes => classes.AssignableTo(typeof(IPipelineBehavior<,>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());
        }

        // Register post-processors
        if (commandQueryConfig.RequestPostProcessorsToRegister.Count > 0)
        {
            services.TryAddEnumerable(ServiceDescriptor.Describe(
                typeof(IPipelineBehavior<,>),
                typeof(RequestPostProcessorBehavior<,>),
                ServiceLifetime.Transient));

            services
                .Scan(scan => scan
                    .FromAssembliesOf(commandQueryConfig.RequestPostProcessorsToRegister.Select(d => d.ImplementationType).ToArray()!)
                    .AddClasses(classes => classes.AssignableTo(typeof(IPipelineBehavior<,>)))
                    .AsImplementedInterfaces()
                    .WithTransientLifetime());
        }

        // Register pipeline behaviors
        services
            .Scan(scan => scan
                .FromAssembliesOf(commandQueryConfig.BehaviorsToRegister.Select(d => d.ImplementationType).ToArray()!)
                .AddClasses(classes => classes.AssignableTo(typeof(IPipelineBehavior<,>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());
    }
}