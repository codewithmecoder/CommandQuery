using System.Reflection;
using CommandQuery.Notifications;
using CommandQuery.PostRequest;
using CommandQuery.PreRequest;
using Microsoft.Extensions.DependencyInjection;

namespace CommandQuery.ServiceRegisters;

public class CommandQueryConfig
{
    /// <summary>
    /// Service lifetime to register services under. Default value is <see cref="ServiceLifetime.Transient"/>
    /// </summary>
    public ServiceLifetime Lifetime { get; set; } = ServiceLifetime.Transient;

    /// <summary>
    /// List of behaviors to register in specific order
    /// </summary>
    public List<ServiceDescriptor> BehaviorsToRegister { get; } = [];

    /// <summary>
    /// Strategy for publishing notifications. Defaults to <see cref="ForeachAwaitPublisher"/>
    /// </summary>
    public INotificationPublisher NotificationPublisher { get; set; } = new ForeachAwaitPublisher();

    /// <summary>
    /// Type of notification publisher strategy to register. If set, overrides <see cref="NotificationPublisher"/>
    /// </summary>
    public Type? NotificationPublisherType { get; set; }

    /// <summary>
    /// List of request pre-processors to register in specific order
    /// </summary>
    public List<ServiceDescriptor> RequestPreProcessorsToRegister { get; } = [];

    /// <summary>
    /// List of request post processors to register in specific order
    /// </summary>
    public List<ServiceDescriptor> RequestPostProcessorsToRegister { get; } = [];


    internal List<Assembly> AssembliesToRegister { get; } = [];

    /// <summary>
    /// Register various handlers from assembly
    /// </summary>
    /// <param name="assembly">Assembly to scan</param>
    /// <returns>This</returns>
    public CommandQueryConfig RegisterAssembly(Assembly assembly)
    {
        AssembliesToRegister.Add(assembly);

        return this;
    }

    public CommandQueryConfig AddBehavior(Type type, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (!type.IsGenericTypeDefinition)
            throw new InvalidOperationException($"{type.Name} must be an open generic type definition.");

        var pipelineBehaviorType = typeof(IPipelineBehavior<,>);
        var implementsPipelineBehavior = type.GetInterfaces().ToList()
            .Exists(i => i.IsGenericType && i.GetGenericTypeDefinition() == pipelineBehaviorType);

        if (!implementsPipelineBehavior)
            throw new InvalidOperationException($"{type.Name} must implement {pipelineBehaviorType.FullName}.");

        BehaviorsToRegister.Add(new ServiceDescriptor(pipelineBehaviorType, type, serviceLifetime));
        return this;
    }


    public CommandQueryConfig AddRequestPreProcessor(Type type, ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        ArgumentNullException.ThrowIfNull(type);
        var implementedInterfaces = type.GetInterfaces()
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestPreProcessor<>))
            .ToList();

        if (implementedInterfaces.Count == 0)
        {
            throw new InvalidOperationException($"{type.Name} must implement IRequestPreProcessor<>.");
        }

        if (type.IsGenericTypeDefinition)
        {
            // Register open generic version
            RequestPreProcessorsToRegister.Add(
                new ServiceDescriptor(typeof(IRequestPreProcessor<>), type, lifetime));
        }
        else
        {
            // Register closed generic(s)
            foreach (var interfaceType in implementedInterfaces)
            {
                RequestPreProcessorsToRegister.Add(new ServiceDescriptor(interfaceType, type, lifetime));
            }
        }

        return this;
    }


    public CommandQueryConfig AddRequestPostProcessor(Type type, ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        ArgumentNullException.ThrowIfNull(type);

        var interfaceTypes = type.GetInterfaces()
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestPostProcessor<,>))
            .ToList();

        if (interfaceTypes.Count == 0)
        {
            throw new InvalidOperationException($"{type.Name} must implement IRequestPostProcessor<,>.");
        }

        if (type.IsGenericTypeDefinition)
        {
            // Register open generic
            RequestPostProcessorsToRegister.Add(
                new ServiceDescriptor(typeof(IRequestPostProcessor<,>), type, lifetime));
        }
        else
        {
            // Register all closed generic interfaces it implements
            foreach (var interfaceType in interfaceTypes)
            {
                RequestPostProcessorsToRegister.Add(
                    new ServiceDescriptor(interfaceType, type, lifetime));
            }
        }

        return this;
    }
}