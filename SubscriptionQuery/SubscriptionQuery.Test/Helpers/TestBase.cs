using Grpc.Net.Client;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SubscriptionQuery.Domain.Enums;
using SubscriptionQuery.Infrastructure.Presistance;
using SubscriptionQuery.Infrastructure.Presistance.Entities;
using Xunit.Abstractions;

namespace SubscriptionQuery.Test.Helpers
{
    public class TestBase
    {
        public WebApplicationFactory<Program> Factory;
        public ApplicationDatabase Database;
        public IMediator Mediator;

        public TestBase(WebApplicationFactory<Program> factory, ITestOutputHelper testOutput)
        {
            Factory = factory.WithDefaultConfigurations(testOutput, services =>
            {
                var descriptor = services.Single(d => d.ServiceType == typeof(DbContextOptions<ApplicationDatabase>));
                services.Remove(descriptor);
                var dbName = Guid.NewGuid().ToString();
                services.AddDbContext<ApplicationDatabase>(options => options.UseInMemoryDatabase(dbName));
            });

            var scope = Factory.Services.CreateScope();
            Database = scope.ServiceProvider.GetRequiredService<ApplicationDatabase>();
            Mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        }
        public static GrpcChannel CreateGrpcChannel(WebApplicationFactory<Program> factory)
        {
            var client = factory.CreateDefaultClient();
            return GrpcChannel.ForAddress(
                client.BaseAddress ?? new Uri("http://localhost:5139"),
                new GrpcChannelOptions
                {
                    HttpClient = client
                });
        }
        protected static UserSubscription GetUserSubscription(Guid aggregateId, Guid userId, Guid subsciptionId, Guid invitationId, Guid accountId, Guid memberId, int sequence, Permissions permission, InvitationStatus invitationStatus, bool isJoined)
        {
            return new UserSubscription
            {
                Id = aggregateId,
                Permissions = permission,
                AccountId = accountId,
                IsJoined = isJoined,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                MemberId = memberId,
                OwnerId = userId,
                Sequence = sequence,
                SubscriptionId = subsciptionId,
                Invitations = new List<Invitation>
            {
                new()
                {
                    DateUpdated = DateTime.UtcNow,
                    DateCreated= DateTime.UtcNow,
                    Id = invitationId,
                    Permission = permission,
                    Status  = invitationStatus,
                    UserSubscriptionId = aggregateId,
                }
            }
            };
        }

    }

}