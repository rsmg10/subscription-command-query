using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SubscriptionCommand.Abstraction;
using SubscriptionCommand.Infrastructure.Presistance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace SubscriptionCommand.Test
{
    public class BaseTester
    {
        protected readonly WebApplicationFactory<Program> Factory;
        protected readonly ApplicationDatabase Database;

        protected readonly IEventStore EventStore;

        public BaseTester(WebApplicationFactory<Program> factory, ITestOutputHelper testOutput)
        {
            Factory = factory;

            Factory = factory.WithDefaultConfigurations(testOutput, services =>
            {
                var descriptor = services.Single(d => d.ServiceType == typeof(DbContextOptions<ApplicationDatabase>));
                services.Remove(descriptor);
                var dbName = Guid.NewGuid().ToString();
                services.AddDbContext<ApplicationDatabase>(options => options.UseInMemoryDatabase(dbName));
            });
            var scope = Factory.Services.CreateScope();

            Database = scope.ServiceProvider.GetService<ApplicationDatabase>()!; 
            EventStore = scope.ServiceProvider.GetService<IEventStore>()!; 
        }
         
    }
}
