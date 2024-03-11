using Microsoft.EntityFrameworkCore;
using SubscriptionQuery.Domain.Enums;
using SubscriptionQuery.Infrastructure.Presistance.Entities;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Mvc.Testing;
using SubscriptionQuery.Test.Helpers;
using SubscriptionQuery.Commands.InvitationAccepted;
using SubscriptionQuery.Commands.InvitationCancelled;
using SubscriptionQuery.Commands.MemberRemoved;
namespace SubscriptionQuery.Test;

public class MemberRemovedTests : TestBase, IClassFixture<WebApplicationFactory<Program>>
{
    public MemberRemovedTests(WebApplicationFactory<Program> factory, ITestOutputHelper testOutput) : base(factory, testOutput)
    {
    }


    [Fact]
    public async Task RemoveMember_NoPriorInvitationAndHigherSequence_ShouldReturnFalse()
    {
        var aggregateId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var invitationId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var accountId = Guid.NewGuid();

        var e = new MemberRemoved(aggregateId,
                                   new MemberRemovedData( userId, subscriptionId, memberId),
                                   DateTime.UtcNow, 2, userId.ToString(), 1);

        var result = await Mediator.Send(e);

        Assert.False(result);

        Assert.Equal(0, await Database.Invitations.CountAsync());
    }

    [Fact]
    public async Task RemoveMember_NoPriorInvitationAndCorrectSequence_ShouldReturnFalse()
    {
        var aggregateId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var invitationId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        var accountId = Guid.NewGuid();

        var e = new MemberRemoved(aggregateId,
                                   new MemberRemovedData(userId, subscriptionId, memberId),
                                   DateTime.UtcNow, 1, userId.ToString(), 1);

        var result = await Mediator.Send(e);

        Assert.False(result);


        Assert.Equal(0, await Database.Invitations.CountAsync());
        Assert.Equal(0, await Database.Subscriptions.CountAsync());
         
    }

    [Fact]
    public async void MemberRemove_WithHigerSequenceAndCorrectData_ShouldReturnFalse()
    {
        var aggregateId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var invitationId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        UserSubscription userSubscription = GetUserSubscription(aggregateId, userId, subscriptionId, invitationId,
                                                                accountId, memberId, 3, Permissions.PurchaseCards,
                                                                InvitationStatus.Accepted, true);

        await Database.Subscriptions.AddAsync(userSubscription);
        await Database.SaveChangesAsync();

         
        var e = new MemberRemoved(aggregateId,
                                   new MemberRemovedData(userId, subscriptionId, memberId),
                                   DateTime.UtcNow, 5, userId.ToString(), 1);

        var result = await Mediator.Send(e);

        Assert.False(result);

        var userSubscriptoinDb = await Database.Subscriptions.Include(x => x.Invitations).FirstAsync();

        Assert.Equal(userSubscription.AccountId, userSubscriptoinDb.AccountId);
        Assert.Equal(userSubscription.Id, userSubscriptoinDb.Id);
        Assert.Equal(userSubscription.OwnerId, userSubscriptoinDb.OwnerId);
        Assert.Equal(userSubscription.SubscriptionId, userSubscriptoinDb.SubscriptionId);
        Assert.Equal(3, userSubscriptoinDb.Sequence);
        Assert.True(userSubscriptoinDb.IsJoined);
        Assert.Equal(Permissions.PurchaseCards, userSubscriptoinDb.Permissions);

    }



    [Fact]
    public async Task RemoveMember_WithLowerSequence_ShouldReturnTrueAsync()
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


        var e = new MemberRemoved(aggregateId,
                                   new MemberRemovedData(userId, subscriptionId, memberId),
                                   DateTime.UtcNow, 2, userId.ToString(), 1);

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
    public async Task RemoveMember_WithCorrectSequenceAndPriorInvitationAllows_ShouldReturnTrueAsync()
    {
        var aggregateId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        var invitationId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        UserSubscription userSubscription = GetUserSubscription(aggregateId, userId, subscriptionId, invitationId,
                                                                accountId, memberId, 3, Permissions.PurchaseCards,
                                                                InvitationStatus.Accepted, true);

        await Database.Subscriptions.AddAsync(userSubscription);
        await Database.SaveChangesAsync();



        var e = new MemberRemoved(aggregateId,
                                   new MemberRemovedData(userId, subscriptionId, memberId),
                                   DateTime.UtcNow, 4, userId.ToString(), 1);

        var result = await Mediator.Send(e);

        Assert.True(result);

        var userSubscriptoinDb = await Database.Subscriptions.Include(x => x.Invitations).FirstAsync();
        Assert.Equal(1, userSubscriptoinDb.Invitations.Count);
        Assert.False(userSubscriptoinDb.IsJoined);
        Assert.Equal(InvitationStatus.Accepted, userSubscriptoinDb.Invitations.First().Status);
        Assert.Equal(userSubscription.AccountId, userSubscriptoinDb.AccountId);
        Assert.Equal(userSubscription.Id, userSubscriptoinDb.Id);
        Assert.Equal(userSubscription.OwnerId, userSubscriptoinDb.OwnerId);
        Assert.Equal(userSubscription.SubscriptionId, userSubscriptoinDb.SubscriptionId);
        Assert.Equal(4, userSubscriptoinDb.Sequence);
        Assert.Equal(userSubscription.Invitations, userSubscriptoinDb.Invitations);
        Assert.Equal(Permissions.PurchaseCards, userSubscriptoinDb.Permissions);

    }


    [Fact]
    public async Task RemoveMember_WithCorrectSequenceAndIncorrectData_ShouldReturnFalseAsync()
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



        var e = new MemberRemoved(Guid.NewGuid(),
                                   new MemberRemovedData(userId, subscriptionId, memberId),
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