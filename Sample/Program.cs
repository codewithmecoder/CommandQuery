using CommandQuery;
using CommandQuery.Notifications;
using Sample;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole(options =>
    {
        options.LogToStandardErrorThreshold = LogLevel.Debug;
    });
    logging.AddDebug();
    logging.SetMinimumLevel(LogLevel.Debug); // Important!
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddTransient<ISmsService, SmsService>();

// Add mediator with the assembly containing handlers
builder.Services.AddCommandQuery(cq =>
{
    cq.RegisterAssembly(typeof(Program).Assembly);
    cq.NotificationPublisher = new MultipleNotificationPublisher();
    cq.NotificationPublisherType = typeof(MultipleNotificationPublisher);
    
    cq.AddBehavior(typeof(LoggingPipelineBehavior<,>));
    cq.AddRequestPreProcessor(typeof(GenericRequestPreProcessor<>));
    cq.AddRequestPostProcessor(typeof(GenericRequestPostProcessor<,>));
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
