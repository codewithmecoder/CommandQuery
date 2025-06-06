namespace CommandQuery;

/// <summary>
/// IRequest interface for defining a request in the CommandQuery framework.
/// </summary>
/// <typeparam name="TResponse"></typeparam>
public interface IRequest<out TResponse>;

/// <summary>
/// IRequest interface for defining a request in the CommandQuery framework without a response type.
/// </summary>
public interface IRequest;