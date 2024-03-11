using Xunit.Abstractions;
using Microsoft.AspNetCore.Mvc.Testing;
using SubscriptionQuery.Test.Helpers;
using SubscriptionQuery.Domain.Enums;
using SubscriptionQuery.Infrastructure.Presistance.Entities;
using Microsoft.Azure.Amqp.Transport;
namespace SubscriptionQuery.Test;

public class PendingInvitationsSentTests : TestBase, IClassFixture<WebApplicationFactory<Program>>
{
    public PendingInvitationsSentTests(WebApplicationFactory<Program> factory, ITestOutputHelper testOutput) : base(factory, testOutput)
    {
    }
     

    [Fact]
    public async Task PendingInvitationsSent()
    {
        var aggregateId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var invitationId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var rand = new Random();
            var subscription = Guid.NewGuid();
        List<UserSubscription> users = new List<UserSubscription>();
        for (int i = 0; i < 10; i++)
        {

            UserSubscription userSubscription = GetUserSubscription(Guid.NewGuid(), userId, subscription, Guid.NewGuid(),
                                                                    accountId, memberId, rand.Next(0, 9), (Permissions)rand.Next(0, 3),
                                                                    InvitationStatus.Pending, false);

            UserSubscription userSubscriptionOther = GetUserSubscription(Guid.NewGuid(), userId, subscription, Guid.NewGuid(),
                                                               accountId, Guid.NewGuid(), rand.Next(0, 9), (Permissions)rand.Next(0, 3),
                                                               InvitationStatus.Accepted, false);
            users.Add(userSubscription);
            users.Add(userSubscriptionOther);

        }
        await Database.Subscriptions.AddRangeAsync(users);
        await Database.SaveChangesAsync();


        var client = new SubscriptionQuery.SubscriptionQueryClient(CreateGrpcChannel(Factory));
        var joinedSubscriptions = await client.PendingInvitationsSentAsync(new PendingInvitationsSentRequest
        {
            UserId = userId.ToString(),
            AccountId = accountId.ToString(),
            SubscriptionId = subscription.ToString()
        });

        Assert.NotNull(joinedSubscriptions);
        Assert.Equal(10, joinedSubscriptions.Invitations.Count);
    }
}