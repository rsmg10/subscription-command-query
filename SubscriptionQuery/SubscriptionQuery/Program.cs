using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SubscriptionQuery.GrpcServices;
using SubscriptionQuery.Infrastructure.MessageBus;
using SubscriptionQuery.Infrastructure.Presistance;
using System;
using SubscriptionQuery.Services;
using SubscriptionQuery.Interceptors;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

Log.Logger = LoggerServiceBuilder.Build();
builder.Services.AddDbContext<ApplicationDatabase>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationDatabase")));
builder.Services.AddHostedService<UserSubscriptionsListener>();
builder.Services.AddSingleton(new ServiceBusClient(builder.Configuration["ServiceBusClient"]));
builder.Services.AddMediatR(options=>
{
    options.RegisterServicesFromAssembly(typeof(Program).Assembly);
 
});

// Add services to the container.
builder.Services.AddGrpc(options =>
{
    {
        options.Interceptors.Add<ServerLoggerInterceptor>();
        options.EnableDetailedErrors = true;
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline. 
app.MapGrpcService<SubscriptionQueryService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

var scope = app.Services.CreateScope();
var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDatabase>();
ctx.Database.Migrate();
app.Run();
namespace SubscriptionQuery
{
    public partial class Program { }
}