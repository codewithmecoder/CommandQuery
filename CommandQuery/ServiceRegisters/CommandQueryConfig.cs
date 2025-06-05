using CommandQuery.Notifications;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CommandQuery.ServiceRegisters;

public class CommandQueryConfig
{
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
    /// List of request pre processors to register in specific order
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

    public CommandQueryConfig AddBehavior(Type openBehaviorType, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
    {
        ArgumentNullException.ThrowIfNull(openBehaviorType);

        if (!openBehaviorType.IsGenericTypeDefinition)
            throw new InvalidOperationException($"{openBehaviorType.Name} must be an open generic type definition.");

        var pipelineBehaviorType = typeof(IPipelineBehavior<,>);
        var implementsPipelineBehavior = openBehaviorType.GetInterfaces().ToList()
            .Exists(i => i.IsGenericType && i.GetGenericTypeDefinition() == pipelineBehaviorType);

        if (!implementsPipelineBehavior)
            throw new InvalidOperationException($"{openBehaviorType.Name} must implement {pipelineBehaviorType.FullName}.");

        BehaviorsToRegister.Add(new ServiceDescriptor(pipelineBehaviorType, openBehaviorType, serviceLifetime));
        return this;
    }

}