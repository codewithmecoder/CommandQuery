using Bas24.CommandQuery.Notifications;

namespace Sample;

public interface IEmailService
{
    Task SendWelcomeEmailAsync(Guid notificationUserId, string notificationName);
}

public class EmailService(ILogger<EmailService> logger) : IEmailService
{
    public Task SendWelcomeEmailAsync(Guid notificationUserId, string notificationName)
    {
        logger.LogDebug("EmailService: Sending welcome email to {NotificationName} with ID {NotificationUserId}", notificationName, notificationUserId);
        return Task.CompletedTask;
    }
}
public interface ISmsService
{
    Task SendWelcomeSmsAsync(Guid notificationUserId, string notificationName);
}

public class SmsService(ILogger<SmsService> logger) : ISmsService
{
    public Task SendWelcomeSmsAsync(Guid notificationUserId, string notificationName)
    {
        logger.LogDebug("SmsService: Sending welcome SMS to {NotificationName} with ID {NotificationUserId}", notificationName, notificationUserId);
        return Task.CompletedTask;
    }
}
public class UserCreatedNotification : INotification
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class EmailNotificationHandler(IEmailService emailService) : INotificationHandler<UserCreatedNotification>
{
    public async Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
    {
        await emailService.SendWelcomeEmailAsync(notification.UserId, notification.Name);
    }
}

public class SmsNotificationHandler(ISmsService smsService) : INotificationHandler<UserCreatedNotification>
{
    public async Task Handle(UserCreatedNotification notification, CancellationToken cancellationToken)
    {
        await smsService.SendWelcomeSmsAsync(notification.UserId, notification.Name);
    }
}