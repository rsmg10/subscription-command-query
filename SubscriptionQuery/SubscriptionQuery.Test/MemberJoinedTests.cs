using Microsoft.EntityFrameworkCore;
using SubscriptionQuery.Domain.Enums;
using SubscriptionQuery.Infrastructure.Presistance.Entities;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Mvc.Testing;
using SubscriptionQuery.Test.Helpers;
using SubscriptionQuery.Commands.InvitationAccepted;
using SubscriptionQuery.Commands.InvitationCancelled;
using SubscriptionQuery.Commands.MemberJoined;
namespace SubscriptionQuery.Test;

public class MemberJoinedTests : TestBase, IClassFixture<WebApplicationFactory<Program>>
{
    public MemberJoinedTests(WebApplicationFactory<Program> factory, ITestOutputHelper testOutput) : base(factory, testOutput)
    {
    }


    [Fact]
    public async Task JoinMember_NoPriorInvitationAndHigherSequence_ShouldReturnFalse()
    {
        var aggregateId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var invitationId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var accountId = Guid.NewGuid();

        var e = new MemberJoined(aggregateId,
                                   new MemberJoinedData(accountId, userId, memberId, subscriptionId, Permissions.Transfer, JoinedBy.Admin),
                                   DateTime.UtcNow, 2, userId.ToString(), 1);

        var result = await Mediator.Send(e);

        Assert.False(result);

        Assert.Equal(0, await Database.Invitations.CountAsync());
    }
    [Fact]
    public async Task JoinMember_NoPriorInvitationAndCorrectSequence_ShouldReturnTrue()
    {
        var aggregateId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var invitationId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var accountId = Guid.NewGuid();

        var e = new MemberJoined(aggregateId,
                                   new MemberJoinedData(accountId, userId, memberId, subscriptionId, Permissions.Transfer, JoinedBy.Admin),
                                   DateTime.UtcNow, 1, userId.ToString(), 1);

        var result = await Mediator.Send(e);

        Assert.True(result);


        Assert.Equal(0, await Database.Invitations.CountAsync());
        Assert.Equal(1, await Database.Subscriptions.CountAsync());


        var userSubscriptoinDb = await Database.Subscriptions.Include(x => x.Invitations).FirstAsync();

        Assert.Equal(accountId, userSubscriptoinDb.AccountId);
        Assert.Equal(aggregateId, userSubscriptoinDb.Id);
        // todo: check how command can send the ownder Id of the subscritpoin
        //Assert.Equal(ownerId, userSubscriptoinDb.OwnerId);
        Assert.Equal(subscriptionId, userSubscriptoinDb.SubscriptionId);
        Assert.Equal(1, userSubscriptoinDb.Sequence);
        Assert.True(userSubscriptoinDb.IsJoined);
        Assert.Equal(Permissions.Transfer, userSubscriptoinDb.Permissions);
    }

    [Fact]
    public async void MemberJoin_WithHigerSequenceAndCorrectData_ShouldReturnFalse()
    {
        var aggregateId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var invitationId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        UserSubscription userSubscription = GetUserSubscription(aggregateId, userId, subscriptionId, invitationId,
                                                                accountId, memberId, 3, Permissions.PurchaseCards,
                                                                InvitationStatus.Pending, false);

        await Database.Subscriptions.AddAsync(userSubscription);
        await Database.SaveChangesAsync();

         
        var e = new MemberJoined(aggregateId,
                           new MemberJoinedData(accountId, userId, memberId, subscriptionId, Permissions.Transfer, JoinedBy.Admin),
                                   DateTime.UtcNow, 5, userId.ToString(), 1);

        var result = await Mediator.Send(e);

        Assert.False(result);

        var userSubscriptoinDb = await Database.Subscriptions.Include(x => x.Invitations).FirstAsync();

        Assert.Equal(userSubscription.AccountId, userSubscriptoinDb.AccountId);
        Assert.Equal(userSubscription.Id, userSubscriptoinDb.Id);
        Assert.Equal(userSubscription.OwnerId, userSubscriptoinDb.OwnerId);
        Assert.Equal(userSubscription.SubscriptionId, userSubscriptoinDb.SubscriptionId);
        Assert.Equal(3, userSubscriptoinDb.Sequence);
        Assert.False(userSubscriptoinDb.IsJoined);
        Assert.Equal(Permissions.PurchaseCards, userSubscriptoinDb.Permissions);

    }



    [Fact]
    public async Task JoinMember_WithLowerSequence_ShouldReturnTrueAsync()
    {
        var aggregateId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var invitationId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        UserSubscription userSubscription = GetUserSubscription(aggregateId, userId, subscriptionId, invitationId,
                                                                accountId, memberId, 3, Permissions.PurchaseCards,
                                                                InvitationStatus.Cancelled, true);

        await Database.Subscriptions.AddAsync(userSubscription);
        await Database.SaveChangesAsync();


        var e = new MemberJoined(aggregateId,
                           new MemberJoinedData(accountId, userId, memberId, subscriptionId, Permissions.Transfer, JoinedBy.Admin),
                                   DateTime.UtcNow, 1, userId.ToString(), 1);

        var result = await Mediator.Send(e);

        Assert.True(result);

        var userSubscriptoinDb = await Database.Subscriptions.Include(x => x.Invitations).FirstAsync();

        Assert.Equal(userSubscription.AccountId, userSubscriptoinDb.AccountId);
        Assert.Equal(userSubscription.Id, userSubscriptoinDb.Id);
        Assert.Equal(userSubscription.OwnerId, userSubscriptoinDb.OwnerId);
        Assert.Equal(userSubscription.SubscriptionId, userSubscriptoinDb.SubscriptionId);
        Assert.Equal(3, userSubscriptoinDb.Sequence);
        Assert.Equal(userSubscription.Invitations, userSubscriptoinDb.Invitations);
        Assert.True(userSubscriptoinDb.IsJoined);
        Assert.Equal(Permissions.PurchaseCards, userSubscriptoinDb.Permissions);

    }


    [Fact]
    public async Task JoinMember_WithCorrectSequenceAndPriorInvitationAllows_ShouldReturnTrueAsync()
    {
        var aggregateId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var invitationId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        UserSubscription userSubscription = GetUserSubscription(aggregateId, userId, subscriptionId, invitationId,
                                                                accountId, memberId, 3, Permissions.PurchaseCards,
                                                                InvitationStatus.Accepted, false);

        await Database.Subscriptions.AddAsync(userSubscription);
        await Database.SaveChangesAsync();



        var e = new MemberJoined(aggregateId,
                           new MemberJoinedData(accountId, userId, memberId, subscriptionId, Permissions.Transfer, JoinedBy.Admin),
                                   DateTime.UtcNow, 4, userId.ToString(), 1);

        var result = await Mediator.Send(e);

        Assert.True(result);

        var userSubscriptoinDb = await Database.Subscriptions.Include(x => x.Invitations).FirstAsync();
        Assert.Equal(1, userSubscriptoinDb.Invitations.Count);
        Assert.True(userSubscriptoinDb.IsJoined);
        Assert.Equal(InvitationStatus.Accepted, userSubscriptoinDb.Invitations.First().Status);
        Assert.Equal(userSubscription.AccountId, userSubscriptoinDb.AccountId);
        Assert.Equal(userSubscription.Id, userSubscriptoinDb.Id);
        Assert.Equal(userSubscription.OwnerId, userSubscriptoinDb.OwnerId);
        Assert.Equal(userSubscription.SubscriptionId, userSubscriptoinDb.SubscriptionId);
        Assert.Equal(4, userSubscriptoinDb.Sequence);
        Assert.Equal(userSubscription.Invitations, userSubscriptoinDb.Invitations);
        Assert.Equal(Permissions.Transfer, userSubscriptoinDb.Permissions);

    }


    [Fact]
    public async Task JoinMember_WithCorrectSequenceAndIncorrectData_ShouldReturnFalseAsync()
    {
        var aggregateId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var invitationId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        UserSubscription userSubscription = GetUserSubscription(aggregateId, userId, subscriptionId, invitationId,
                                                                accountId, memberId, 3, Permissions.PurchaseCards,
                                                                InvitationStatus.Pending, false);

        await Database.Subscriptions.AddAsync(userSubscription);
        await Database.SaveChangesAsync();



        var e = new MemberJoined(Guid.NewGuid(),
                           new MemberJoinedData(accountId, userId, memberId, subscriptionId, Permissions.Transfer, JoinedBy.Admin),
                                   DateTime.UtcNow, 4, userId.ToString(), 1);

        var result = await Mediator.Send(e);

        Assert.False(result);

        var userSubscriptoinDb = await Database.Subscriptions.Include(x => x.Invitations).FirstAsync();

        Assert.Equal(userSubscription.AccountId, userSubscriptoinDb.AccountId);
        Assert.Equal(userSubscription.Id, userSubscriptoinDb.Id);
        Assert.Equal(userSubscription.OwnerId, userSubscriptoinDb.OwnerId);
        Assert.Equal(userSubscription.SubscriptionId, userSubscriptoinDb.SubscriptionId);
        Assert.Equal(3, userSubscriptoinDb.Sequence);
        Assert.Equal(userSubscription.Invitations, userSubscriptoinDb.Invitations);
        Assert.False(userSubscriptoinDb.IsJoined);
        Assert.Equal(Permissions.PurchaseCards, userSubscriptoinDb.Permissions);

    }

    private static UserSubscription GetUserSubscription(Guid aggregateId, Guid userId, Guid subsciptionId, Guid invitationId, Guid accountId, Guid memberId, int sequence, Permissions permission, InvitationStatus invitationStatus, bool isJoined)
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