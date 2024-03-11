using Xunit.Abstractions;
using Microsoft.AspNetCore.Mvc.Testing;
using SubscriptionQuery.Test.Helpers;
using SubscriptionQuery.Domain.Enums;
using SubscriptionQuery.Infrastructure.Presistance.Entities;
using Microsoft.Azure.Amqp.Transport;
namespace SubscriptionQuery.Test;

public class MembersInSubscriptionTests : TestBase, IClassFixture<WebApplicationFactory<Program>>
{
    public MembersInSubscriptionTests(WebApplicationFactory<Program> factory, ITestOutputHelper testOutput) : base(factory, testOutput)
    {
    }
     

    [Fact]
    public async Task MembersInSubscriptionTests_ShouldReturnValidResults()
    { 
        var userId = Guid.NewGuid(); 
        var accountId = Guid.NewGuid();
        var rand = new Random();
            var subscription = Guid.NewGuid();
        List<UserSubscription> users = new List<UserSubscription>();
        for (int i = 0; i < 10; i++)
        {

  
            UserSubscription userSubscriptionOther = GetUserSubscription(Guid.NewGuid(), userId, subscription, Guid.NewGuid(),
                                                               accountId, Guid.NewGuid(), rand.Next(0, 9), (Permissions)rand.Next(0, 3),
                                                               InvitationStatus.Accepted, true);
         
            users.Add(userSubscriptionOther);
        }
        await Database.Subscriptions.AddRangeAsync(users);
        await Database.SaveChangesAsync();


        var client = new SubscriptionQuery.SubscriptionQueryClient(CreateGrpcChannel(Factory));
        var joinedSubscriptions = await client.GetMembersInSubscriptionAsync(new MembersInSubscriptionRequest
        {
            UserId = userId.ToString(),
            AccountId = accountId.ToString(),
            SubscriptionId = subscription.ToString()
        });

        Assert.NotNull(joinedSubscriptions);
        Assert.Equal(10, joinedSubscriptions.Members.Count);
    }
}