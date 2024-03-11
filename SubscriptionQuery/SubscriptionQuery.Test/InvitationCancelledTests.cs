using Microsoft.EntityFrameworkCore;
using SubscriptionQuery.Domain.Enums;
using SubscriptionQuery.Infrastructure.Presistance.Entities;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Mvc.Testing;
using SubscriptionQuery.Test.Helpers;
using SubscriptionQuery.Commands.InvitationAccepted;
using SubscriptionQuery.Commands.InvitationCancelled;
namespace SubscriptionQuery.Test;

public class InvitationCancelledTests : TestBase, IClassFixture<WebApplicationFactory<Program>>
{
    public InvitationCancelledTests(WebApplicationFactory<Program> factory, ITestOutputHelper testOutput) : base(factory, testOutput)
    {
    }


    [Fact]
    public async Task CancelledInvitation_NoPriorInvitationAndHigherSequence_ShouldReturnFalse()
    {
        var aggregateId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var invitationId = Guid.NewGuid();

        var e = new InvitationCancelled(aggregateId,
                                   new InvitationCancelledData(),
                                   DateTime.UtcNow, 2, userId.ToString(), 1);

        var result = await Mediator.Send(e);

        Assert.False(result);

        Assert.Equal(0, await Database.Invitations.CountAsync());
    }

    [Fact]
    public async void CancelledInvitation_WithHigerSequenceAndCorrectData_ShouldReturnFalse()
    {
        var aggregateId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var subsciptionId = Guid.NewGuid();
        var invitationId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        UserSubscription userSubscription = GetUserSubscription(aggregateId, userId, subsciptionId, invitationId,
                                                                accountId, memberId, 3, Permissions.PurchaseCards,
                                                                InvitationStatus.Pending, false);

        await Database.Subscriptions.AddAsync(userSubscription);
        await Database.SaveChangesAsync();


        var e = new InvitationCancelled(aggregateId,
                                   new InvitationCancelledData(),
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
    public async Task SendInvitation_WithLowerSequence_ShouldReturnTrueAsync()
    {
        var aggregateId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var subsciptionId = Guid.NewGuid();
        var invitationId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        UserSubscription userSubscription = GetUserSubscription(aggregateId, userId, subsciptionId, invitationId,
                                                                accountId, memberId, 3, Permissions.PurchaseCards,
                                                                InvitationStatus.Cancelled, true);

        await Database.Subscriptions.AddAsync(userSubscription);
        await Database.SaveChangesAsync();


        var e = new InvitationCancelled(aggregateId,
                                   new InvitationCancelledData(),
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
    public async Task CancelledInvitation_WithCorrectSequenceAndPriorInvitationAllows_ShouldReturnTrueAsync()
    {
        var aggregateId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var subsciptionId = Guid.NewGuid();
        var invitationId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        UserSubscription userSubscription = GetUserSubscription(aggregateId, userId, subsciptionId, invitationId,
                                                                accountId, memberId, 3, Permissions.PurchaseCards,
                                                                InvitationStatus.Pending, false);

        await Database.Subscriptions.AddAsync(userSubscription);
        await Database.SaveChangesAsync();


        var e = new InvitationCancelled(aggregateId,
                                   new InvitationCancelledData(),
                                   DateTime.UtcNow, 4, userId.ToString(), 1);

        var result = await Mediator.Send(e);

        Assert.True(result);

        var userSubscriptoinDb = await Database.Subscriptions.Include(x => x.Invitations).FirstAsync();
        Assert.Equal(1, userSubscriptoinDb.Invitations.Count);
        Assert.False(userSubscriptoinDb.IsJoined);
        Assert.Equal(InvitationStatus.Cancelled, userSubscriptoinDb.Invitations.First().Status);
        Assert.Equal(userSubscription.AccountId, userSubscriptoinDb.AccountId);
        Assert.Equal(userSubscription.Id, userSubscriptoinDb.Id);
        Assert.Equal(userSubscription.OwnerId, userSubscriptoinDb.OwnerId);
        Assert.Equal(userSubscription.SubscriptionId, userSubscriptoinDb.SubscriptionId);
        Assert.Equal(4, userSubscriptoinDb.Sequence);
        Assert.Equal(userSubscription.Invitations, userSubscriptoinDb.Invitations);
        Assert.False(userSubscriptoinDb.IsJoined);
        Assert.Equal(Permissions.PurchaseCards, userSubscriptoinDb.Permissions);

    }


    [Fact]
    public async Task CancelledInvitation_WithCorrectSequenceAndIncorrectData_ShouldReturnTrueAsync()
    {
        var aggregateId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var subsciptionId = Guid.NewGuid();
        var invitationId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        UserSubscription userSubscription = GetUserSubscription(aggregateId, userId, subsciptionId, invitationId,
                                                                accountId, memberId, 3, Permissions.PurchaseCards,
                                                                InvitationStatus.Pending, false);

        await Database.Subscriptions.AddAsync(userSubscription);
        await Database.SaveChangesAsync();


        var e = new InvitationCancelled(Guid.NewGuid(),
                                   new InvitationCancelledData(),
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