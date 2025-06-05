using CommandQuery;
using FluentValidation;

namespace Sample;

public interface IUserRepository
{
    Task<int> AddAsync(User user, CancellationToken cancellationToken);
}

public class UserRepository : IUserRepository
{
    public Task<int> AddAsync(User user, CancellationToken cancellationToken)
    {
        return Task.FromResult(1);
    }
}

public class CreateUserCommand : IRequest<int>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(command => command.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters");

            RuleFor(command => command.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("A valid email address is required")
                .MaximumLength(255).WithMessage("Email cannot exceed 255 characters");
        }
    }
}

public class CreateUserCommandHandler(IUserRepository repository, ICommandQuery commandQuery)
    : IRequestHandler<CreateUserCommand, int>
{
    public async Task<int> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User { Id = Guid.NewGuid(), Name = request.Name, Email = request.Email };
        var userCreatedNotification = new UserCreatedNotification
        {
            Name = user.Name,
            UserId = user.Id,
        };
        await commandQuery.Publish(userCreatedNotification, cancellationToken);
        return await repository.AddAsync(user, cancellationToken);
    }
}
public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}