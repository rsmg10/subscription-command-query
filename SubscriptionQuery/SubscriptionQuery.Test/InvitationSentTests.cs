using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using SubscriptionQuery.Commands.InvitationSent;
using SubscriptionQuery.Commands.MemberRemoved;
using SubscriptionQuery.Domain.Enums;
using SubscriptionQuery.Infrastructure.Presistance.Entities;
using Xunit.Abstractions;
using SubscriptionQuery;
using Microsoft.AspNetCore.Mvc.Testing;
using SubscriptionQuery.Infrastructure.Presistance;
using SubscriptionQuery.Test.Helpers;
namespace SubscriptionQuery.Test;

public class InvitationSentTests : TestBase, IClassFixture<WebApplicationFactory<Program>>
{
    public InvitationSentTests(WebApplicationFactory<Program> factory, ITestOutputHelper testOutput) : base(factory, testOutput)
    {
    }

    [Fact]
    public async Task SendInvitation_NoPriorInvitation_ShouldReturnTrueAsync()
    {
        var aggregateId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var subsciptionId = Guid.NewGuid();
        var invitationId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        
        var e = new InvitationSent(aggregateId,
                                   new InvitationSentData(userId, subsciptionId, memberId, Permissions.PurchaseCards, invitationId, accountId),
                                   DateTime.UtcNow, 1, userId.ToString(), 1);

        var result = await Mediator.Send(e);

        Assert.True(result);

        Assert.Equal(1, await Database.Invitations.CountAsync());
        var invitation = await Database.Invitations.FirstAsync();
        Assert.NotNull(invitation);
        Assert.True(invitation.Permission == Permissions.PurchaseCards);
        Assert.Equal(aggregateId, invitation.UserSubscriptionId);
        Assert.Equal(invitationId, invitation.Id);
        Assert.Equal(InvitationStatus.Pending, invitation.Status);

        Assert.Equal(1, await Database.Subscriptions.CountAsync());
        var userSubscription = await Database.Subscriptions.FirstAsync();
        Assert.Equal(aggregateId, userSubscription.Id);
        Assert.Equal(userId, userSubscription.OwnerId);
        Assert.Equal(accountId, userSubscription.AccountId);
        Assert.Equal(Permissions.None, userSubscription.Permissions);
        Assert.False(userSubscription.IsJoined);

    }

    [Fact]
    public async void SendInvitation_WithHigerSequence_ShouldReturnFalse()
    {
        var aggregateId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var subsciptionId = Guid.NewGuid();
        var invitationId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        UserSubscription userSubscription = GetUserSubscription(aggregateId, userId, subsciptionId, invitationId,
                                                                accountId, memberId, 3, Permissions.PurchaseCards,
                                                                InvitationStatus.Accepted, true);

        await Database.Subscriptions.AddAsync(userSubscription);
        await Database.SaveChangesAsync();


        var e = new InvitationSent(aggregateId,
                                   new InvitationSentData(userId, subsciptionId, memberId, Permissions.PurchaseCards, invitationId, accountId),
                                   DateTime.UtcNow, 5, userId.ToString(), 1);

        var result = await Mediator.Send(e);

        Assert.False(result);
        
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
    public async Task SendInvitation_WithLowerSequence_ShouldReturnTrueAsync()
    {
        var aggregateId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var subsciptionId = Guid.NewGuid();
        var invitationId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        UserSubscription userSubscription = GetUserSubscription(aggregateId,
                                                                userId,
                                                                subsciptionId,
                                                                invitationId,
                                                                accountId,
                                                                memberId,
                                                                8,
                                                                Permissions.PurchaseCards,
                                                                InvitationStatus.Accepted,
                                                                true);

        await Database.Subscriptions.AddAsync(userSubscription);
        await Database.SaveChangesAsync();


        //var e = new MemberRemoved(aggregateId, new MemberRemovedData(userId, subsciptionId, memberId), DateTime.UtcNow, 7, Guid.NewGuid().ToString(), 1);
        var e = new InvitationSent(aggregateId,
                           new InvitationSentData(userId, subsciptionId, memberId, Permissions.PurchaseCards, invitationId, accountId),
                           DateTime.UtcNow, 7, userId.ToString(), 1);

        var result = await Mediator.Send(e);

        Assert.True(result);

        var userSubscriptoinDb = await Database.Subscriptions.Include(x => x.Invitations).FirstAsync();

        Assert.Equal(userSubscription.AccountId, userSubscriptoinDb.AccountId);
        Assert.Equal(userSubscription.Id, userSubscriptoinDb.Id);
        Assert.Equal(userSubscription.OwnerId, userSubscriptoinDb.OwnerId);
        Assert.Equal(userSubscription.SubscriptionId, userSubscriptoinDb.SubscriptionId);
        Assert.Equal(8, userSubscriptoinDb.Sequence);
        Assert.Equal(userSubscription.Invitations, userSubscriptoinDb.Invitations);
        Assert.True(userSubscriptoinDb.IsJoined);
        Assert.Equal(Permissions.PurchaseCards, userSubscriptoinDb.Permissions);
    }

    [Fact]
    public async Task SendInvitation_WithCorrectSequenceAndPriorInvitationAllows_ShouldReturnTrueAsync()
    { 
        var aggregateId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var subsciptionId = Guid.NewGuid();
        var invitationId = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var memberId = Guid.NewGuid();
        UserSubscription userSubscription = GetUserSubscription(aggregateId, userId, subsciptionId, invitationId,
                                                                accountId, memberId, 8, Permissions.PurchaseCards,
                                                                InvitationStatus.Accepted, false);

        await Database.Subscriptions.AddAsync(userSubscription);
        await Database.SaveChangesAsync();
        //Database.DetachAllEntities();


        //var e = new MemberRemoved(aggregateId, new MemberRemovedData(userId, subsciptionId, memberId), DateTime.UtcNow, 9, Guid.NewGuid().ToString(), 1);
        var e = new InvitationSent(aggregateId,
                   new InvitationSentData(userId, subsciptionId, memberId, Permissions.PurchaseCards, Guid.NewGuid(), accountId),
                   DateTime.UtcNow, 9, userId.ToString(), 1);

        var result = await Mediator.Send(e);

        Assert.True(result);

        Assert.Equal(2, await Database.Invitations.AsNoTracking().CountAsync());
        Assert.Equal(1, await Database.Subscriptions.AsNoTracking().CountAsync());

        var userSubscriptoinDb = await Database.Subscriptions.AsNoTracking().Include(x => x.Invitations).FirstAsync();

        Assert.Equal(userSubscription.AccountId, userSubscriptoinDb.AccountId);
        Assert.Equal(userSubscription.Id, userSubscriptoinDb.Id);
        Assert.Equal(userSubscription.OwnerId, userSubscriptoinDb.OwnerId);
        Assert.Equal(userSubscription.SubscriptionId, userSubscriptoinDb.SubscriptionId);
        Assert.Equal(9, userSubscriptoinDb.Sequence); 
        Assert.False(userSubscriptoinDb.IsJoined);
        Assert.Equal(Permissions.PurchaseCards, userSubscriptoinDb.Permissions);
    }


    private static UserSubscription GetUserSubscription(Guid aggregateId, Guid userId, Guid subsciptionId, Guid invitationId, Guid accountId, Guid memberId, int sequence, Permissions permission, InvitationStatus invitationStatus, bool isJoined )
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