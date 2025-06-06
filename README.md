# CommandQuery
CommandQuery is a Mediator CQRS implementation.

Supports request/response, commands, queries, notifications and events, synchronous and async with intelligent dispatching via C# generic variance.

Examples in this [Project](https://github.com/codewithmecoder/CommandQuery/tree/main/Sample).

### Installing CommandQuery

You should install [CommandQuery with NuGet](https://github.com/codewithmecoder/CommandQuery)

```SHELL
Install-Package CommandQuery
```

Or via the .NET Core command line interface:

```SHELL
dotnet add package CommandQuery
```
### Registering with assembly

```CSharp
services.CommandQuery(cq => cq.RegisterAssembly(typeof(Program).Assembly));
```

This registers:

- `ICommandQuery` as scoped
- `IRequestHandler<,>` concrete implementations as transient
- `IRequestHandler<>` concrete implementations as transient
- `INotificationHandler<>` concrete implementations as transient

This also registers open generic implementations for:

- `INotificationHandler<>`

To register behaviors, pre/post processors:

```csharp
services.AddMediatR(cfg => {
    cq.RegisterAssembly(typeof(Program).Assembly);
    //cq.NotificationPublisher = new MultipleNotificationPublisher();
    //cq.NotificationPublisherType = typeof(MultipleNotificationPublisher);

    cq.AddBehavior(typeof(LoggingPipelineBehavior<,>));
    cq.AddRequestPreProcessor(typeof(GenericRequestPreProcessor<>));
    cq.AddRequestPostProcessor(typeof(GenericRequestPostProcessor<,>));
    });
```

With additional methods for open generics and overloads for explicit service types.
