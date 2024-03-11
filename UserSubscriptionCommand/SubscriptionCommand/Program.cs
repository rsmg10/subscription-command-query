using Azure.Messaging.ServiceBus;
using Microsoft.EntityFrameworkCore;
using SubscriptionCommand.Abstraction;
using SubscriptionCommand.Infrastructure.MessageBus;
using SubscriptionCommand.Infrastructure.Presistance;
using SubscriptionCommand.Services;
using Calzolari.Grpc.AspNetCore.Validation;
using MediatR;
using SubscriptionCommand;
using SubscriptionCommand.Behaviours;
using SubscriptionQuery.Interceptors;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration["ConnectionStrings:ApplicationDatabase"];
Console.WriteLine(connectionString);

builder.Services.AddMediatR(o => o.RegisterServicesFromAssemblyContaining<Program>());
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ExceptionBehaviour<,>));
builder.Services.AddDbContext<ApplicationDatabase>(o => o.UseSqlServer(connectionString));
builder.Services.AddScoped<IEventStore, EventStore>();

//builder.WebHost.ConfigureKestrel(serverOptions =>
//{
//    serverOptions.ListenAnyIP(8080, o => o.Protocols = HttpProtocols.Http2);
//    serverOptions.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
//    serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(1); 
//    serverOptions.Limits.Http2.InitialStreamWindowSize = 98_304;
//    serverOptions.Limits.Http2.KeepAlivePingDelay = TimeSpan.FromSeconds(30);
//    serverOptions.Limits.Http2.KeepAlivePingTimeout = TimeSpan.FromMinutes(1);
//    serverOptions.Limits.Http2.InitialConnectionWindowSize = 131_072;
//});

var busConnection = builder.Configuration["ServiceBusClient"];
builder.Services.AddSingleton(new ServiceBusClient(busConnection));
builder.Services.AddSingleton<AzureMessageBus>();
builder.Services.AddGrpc(options =>
{
    options.Interceptors.Add<ServerLoggerInterceptor>();
    options.EnableDetailedErrors = true;
    options.EnableMessageValidation();
    
});

builder.Services.AddValidators();
builder.Services.AddGrpcValidation();

Console.WriteLine("______________________________TEST2222222_______________________________");


var app = builder.Build();

app.MapGrpcService<InvitationService>();
app.UseMiddleware<ExceptionMiddleware>();

var scope = app.Services.CreateScope();
var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDatabase>();
ctx.Database.Migrate();
app.Run();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

namespace SubscriptionCommand
{
    public partial class Program { }
}