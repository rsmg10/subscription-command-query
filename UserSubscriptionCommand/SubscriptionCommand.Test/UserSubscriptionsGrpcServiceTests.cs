using Grpc.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using SubscriptionCommand.Domain;
using SubscriptionCommand.Domain.Enums;
using SubscriptionCommand.Events;
using SubscriptionCommand.Extensions;
using SubscriptionCommand.Infrastructure.Presistance;
using SubscriptionCommandProto;
using System.Text.Json;
using Xunit.Abstractions;

namespace SubscriptionCommand.Test
{
    public class UserSubscriptionsGrpcServiceTests : BaseTester, IClassFixture<WebApplicationFactory<Program>>
    {

        public UserSubscriptionsGrpcServiceTests(WebApplicationFactory<Program> factory, ITestOutputHelper testOutput)
            : base(factory, testOutput)
        {

        } 

        [Fact]
        public async Task SendInvitation_SendValidData_InvitationSent()
        {
            var client = new SubscriptionCommandProto.SubscriptionCommand.SubscriptionCommandClient(Factory.CreateGrpcChannel());
            var res = await client.SendInvitationAsync(new SubscriptionCommandProto.SendInvitationRequest
            {
                AccountId = Guid.NewGuid().ToString(),
                MemberId = Guid.NewGuid().ToString(),
                Permission = 1,
                SubscriptionId = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
            });

            using (var scope = Factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDatabase>();
                await Console.Out.WriteLineAsync(JsonConvert.SerializeObject(await context.Events.ToListAsync()));
                var eventCount = await context.Events.CountAsync();
                Assert.Equal(1, eventCount);
            }

            Assert.NotNull(res);
            int count = await Database.Events.CountAsync();
            Assert.True(count == 1);
            return;
        }


        [Theory]
        [InlineData("BA66601C-8A7E-43BC-BC95-BDCF6F5B79A3", "BA66601C-8A7E-43BC-BC95-BDCF6F5B79A3", 0, "9A687FB1-5555-4B39-BBDD-8202C0E77038", "1377561B-1C64-4DF9-8767-29C1C05F75B1")]
        [InlineData("2E576522-615F-4B43-BEE2-14413F5011C0", "", 1, "9A687FB1-5555-4B39-BBDD-8202C0E77038", "1377561B-1C64-4DF9-8767-29C1C05F75B1")]
        [InlineData("", "BA66601C-8A7E-43BC-BC95-BDCF6F5B79A3", 0, "9A687FB1-5555-4B39-BBDD-8202C0E77038", "1377561B-1C64-4DF9-8767-29C1C05F75B1")]
        [InlineData("318525A4-0DE4-4A3B-BFD9-1928F43FB493", "BA66601C-8A7E-43BC-BC95-BDCF6F5B79A3", 1, "", "1377561B-1C64-4DF9-8767-29C1C05F75B1")]
        [InlineData("AA66601C-8A7E-43BC-BC95-BDCF6F5B79A3", "BA66601C-8A7E-43BC-BC95-BDCF6F5B79A3", 1, "9A687FB1-5555-4B39-BBDD-8202C0E77038", "")]

        public async Task SendInvitation_SendInvalidData_ValidationError(string accountId, string memberId, int permission, string subscriptionId, string userId)
        {

            var client = new SubscriptionCommandProto.SubscriptionCommand.SubscriptionCommandClient(Factory.CreateGrpcChannel());

            var ex = await Assert.ThrowsAsync<RpcException>(async () =>
            {
                await client.SendInvitationAsync(new SubscriptionCommandProto.SendInvitationRequest
                {
                    AccountId = accountId,
                    MemberId = memberId,
                    Permission = permission,
                    SubscriptionId = subscriptionId,
                    UserId = userId
                });
            });
            Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
            int count = await Database.Events.CountAsync();
            Assert.True(count == 0);
        }

        [Theory]
        [InlineData("BA66601C-8A7E-43BC-BC95-BDCF6F5B79A3", "1377561B-1C64-4DF9-8767-29C1C05F75B1", 7, "9A687FB1-5555-4B39-BBDD-8202C0E77038", "1377561B-1C64-4DF9-8767-29C1C05F75B1")]
        public async Task SendInvitation_UserIdEqualMemberId_ValidationError(string accountId, string memberId, int permission, string subscriptionId, string userId)
        {

            var client = new SubscriptionCommandProto.SubscriptionCommand.SubscriptionCommandClient(Factory.CreateGrpcChannel());

            var ex = await Assert.ThrowsAsync<RpcException>(async () =>
            {
                await client.SendInvitationAsync(new SubscriptionCommandProto.SendInvitationRequest
                {
                    AccountId = accountId,
                    MemberId = memberId,
                    Permission = permission,
                    SubscriptionId = subscriptionId,
                    UserId = userId
                });
            });
            Assert.Equal(StatusCode.InvalidArgument, ex.StatusCode);
            int count = await Database.Events.CountAsync();
            Assert.True(count == 0);
        }
        [Fact]

        public async Task SendAndAcceptInvitation_Valid()
        {
            var userId = Guid.NewGuid().ToString();
            var memberId = Guid.NewGuid().ToString();
            var subscriptionId = Guid.NewGuid().ToString();
            var accountId = Guid.NewGuid().ToString();

            var client = new SubscriptionCommandProto.SubscriptionCommand.SubscriptionCommandClient(Factory.CreateGrpcChannel());
            var sendInvitationAsync = await client.SendInvitationAsync(new SubscriptionCommandProto.SendInvitationRequest
            {
                AccountId = accountId,
                MemberId = memberId,
                Permission = 7,
                SubscriptionId = subscriptionId,
                UserId = userId
            });

            Assert.NotNull(sendInvitationAsync);

            var acceptInvitationAsync = await client.AcceptInvitationAsync(new SubscriptionCommandProto.AcceptInvitationRequest()
            {
                AccountId = accountId,
                MemberId = memberId,
                SubscriptionId = subscriptionId,
                UserId = userId
            });

            Assert.NotNull(acceptInvitationAsync);
            int count = await Database.Events.CountAsync();
            Assert.True(count == 3);

        }
        [Fact]
        public async Task SendAndRejectInvitation_Valid()
        {
            var userId = Guid.NewGuid().ToString();
            var memberId = Guid.NewGuid().ToString();
            var subscriptionId = Guid.NewGuid().ToString();
            var accountId = Guid.NewGuid().ToString();

            var client = new SubscriptionCommandProto.SubscriptionCommand.SubscriptionCommandClient(Factory.CreateGrpcChannel());
            var sendInvitationAsync = await client.SendInvitationAsync(new SubscriptionCommandProto.SendInvitationRequest
            {
                AccountId = accountId,
                MemberId = memberId,
                Permission = 7,
                SubscriptionId = subscriptionId,
                UserId = userId
            });

            Assert.NotNull(sendInvitationAsync);

            var rejectInvitationAsync = await client.RejectInvitationAsync(new SubscriptionCommandProto.RejectInvitationRequest()
            {
                AccountId = accountId,
                MemberId = memberId,
                SubscriptionId = subscriptionId,
                UserId = userId
            });

            Assert.NotNull(rejectInvitationAsync);
            int count = await Database.Events.CountAsync();
            Assert.True(count == 2);
        }

        [Fact]
        public async Task SendAndRejectThenSendInvitation_Valid()
        {
            var userId = Guid.NewGuid().ToString();
            var memberId = Guid.NewGuid().ToString();
            var subscriptionId = Guid.NewGuid().ToString();
            var accountId = Guid.NewGuid().ToString();

            var client = new SubscriptionCommandProto.SubscriptionCommand.SubscriptionCommandClient(Factory.CreateGrpcChannel());
            var request = new SendInvitationRequest
            {
                AccountId = accountId,
                MemberId = memberId,
                Permission = 7,
                SubscriptionId = subscriptionId,
                UserId = userId
            };
            var sendInvitationAsync = await client.SendInvitationAsync(request);

            Assert.NotNull(sendInvitationAsync);

            var rejectInvitationAsync = await client.RejectInvitationAsync(new SubscriptionCommandProto.RejectInvitationRequest()
            {
                AccountId = accountId,
                MemberId = memberId,
                SubscriptionId = subscriptionId,
                UserId = userId
            });

            Assert.NotNull(rejectInvitationAsync);

            var secondSendInvitationAsync = await client.SendInvitationAsync(request);

            Assert.NotNull(secondSendInvitationAsync);
            Assert.True(Guid.TryParse(secondSendInvitationAsync.Id, out _));


            int count = await Database.Events.CountAsync();
            Assert.True(count == 3);

        }


        [Fact]
        public async Task SendInvitationTwice_RejectSecondOne_Valid()
        {
            var userId = Guid.NewGuid().ToString();
            var memberId = Guid.NewGuid().ToString();
            var subscriptionId = Guid.NewGuid().ToString();
            var accountId = Guid.NewGuid().ToString();

            SendInvitationRequest request = new SendInvitationRequest
            {
                AccountId = accountId,
                MemberId = memberId,
                Permission = 7,
                SubscriptionId = subscriptionId,
                UserId = userId
            };
            var client = new SubscriptionCommandProto.SubscriptionCommand.SubscriptionCommandClient(Factory.CreateGrpcChannel());

            var sendInvitationAsync = await client.SendInvitationAsync(request);

            Assert.NotNull(sendInvitationAsync);
            Assert.ThrowsAsync<RpcException>(async () => await client.SendInvitationAsync(request));


            int count = await Database.Events.CountAsync(); 
            Assert.True(count == 1);

        }

        [Fact]
        public async Task SendAndCancelInvitation_Valid()
        {
            var userId = Guid.NewGuid().ToString();
            var memberId = Guid.NewGuid().ToString();
            var subscriptionId = Guid.NewGuid().ToString();
            var accountId = Guid.NewGuid().ToString();

            var client = new SubscriptionCommandProto.SubscriptionCommand.SubscriptionCommandClient(Factory.CreateGrpcChannel());
            var sendInvitationAsync = await client.SendInvitationAsync(new SubscriptionCommandProto.SendInvitationRequest
            {
                AccountId = accountId,
                MemberId = memberId,
                Permission = 7,
                SubscriptionId = subscriptionId,
                UserId = userId
            });

            Assert.NotNull(sendInvitationAsync);

            var cancelnvitationAsync = await client.CancelInvitationAsync(new SubscriptionCommandProto.CancelInvitationRequest()
            {
                AccountId = accountId,
                MemberId = memberId,
                SubscriptionId = subscriptionId,
                UserId = userId
            });

            Assert.NotNull(cancelnvitationAsync);
            int count = await Database.Events.CountAsync();
            Assert.True(count == 2);

        }




        [Fact]
        public async Task JoinMemberThenChangePermission_Valid()
        {
            var userId = Guid.NewGuid();
            var memberId = Guid.NewGuid();
            var subscriptionId = Guid.NewGuid();
            var accountId = Guid.NewGuid();

            var client = new SubscriptionCommandProto.SubscriptionCommand.SubscriptionCommandClient(Factory.CreateGrpcChannel());
            var sendInvitationAsync = await client.JoinMemberAsync(new SubscriptionCommandProto.JoinMemberRequest
            {
                AccountId = accountId.ToString(),
                MemberId = memberId.ToString(),
                Permission = 7,
                SubscriptionId = subscriptionId.ToString(),
                UserId = userId.ToString()
            });

            Assert.NotNull(sendInvitationAsync);
            
            int count = await Database.Events.CountAsync();
            Assert.True(count == 1);


           var usersubscription = UserSubscription.LoadFromHistory(await EventStore.GetAllAsync(GuidExtensions.CombineGuids(subscriptionId, memberId), default));
           Assert.NotNull(usersubscription);
           Assert.True(usersubscription.Permission == (Permissions)7);
           Assert.True(usersubscription.IsJoined);

            // change permission 

            var changePermission = await client.ChangePermissionAsync(new SubscriptionCommandProto.ChangePermissionRequest
            {
                AccountId = accountId.ToString(),
                MemberId = memberId.ToString(),
                Permission = 1,
                SubscriptionId = subscriptionId.ToString(),
                UserId = userId.ToString()
            });

            Assert.NotNull(changePermission);

            count = await Database.Events.CountAsync();
            Assert.True(count == 2);


            usersubscription = UserSubscription.LoadFromHistory(await EventStore.GetAllAsync(GuidExtensions.CombineGuids(subscriptionId, memberId), default));
            Assert.NotNull(usersubscription);
            Assert.True(usersubscription.Permission == (Permissions)1);
            Assert.True(usersubscription.IsJoined);

        }

        [Fact]
        public async Task JoinMemberThenRemove_Valid()
        {
            var userId = Guid.NewGuid();
            var memberId = Guid.NewGuid();
            var subscriptionId = Guid.NewGuid();
            var accountId = Guid.NewGuid();

            var client = new SubscriptionCommandProto.SubscriptionCommand.SubscriptionCommandClient(Factory.CreateGrpcChannel());
            var sendInvitationAsync = await client.JoinMemberAsync(new SubscriptionCommandProto.JoinMemberRequest
            {
                AccountId = accountId.ToString(),
                MemberId = memberId.ToString(),
                Permission = 7,
                SubscriptionId = subscriptionId.ToString(),
                UserId = userId.ToString()
            });

            Assert.NotNull(sendInvitationAsync);

            var joinEvent = await Database.Events.ToListAsync();
            Assert.True(joinEvent.Count == 1);
            Assert.True(joinEvent.First().Type == nameof(MemberJoined));
            string? data = joinEvent.First().Data;
            MemberJoinedData? memberJoined = System.Text.Json.JsonSerializer.Deserialize<MemberJoinedData>(data);
            Assert.True(memberJoined.JoinedBy == JoinedBy.Admin);

            var usersubscription = UserSubscription.LoadFromHistory(await EventStore.GetAllAsync(GuidExtensions.CombineGuids(subscriptionId, memberId), default));
            Assert.NotNull(usersubscription);
            Assert.True(usersubscription.Permission == (Permissions)7);
            Assert.True(usersubscription.IsJoined);

            // remove member

            var changePermission = await client.RemoveMemberAsync(new SubscriptionCommandProto.RemoveMemberRequest
            {
                AccountId = accountId.ToString(),
                MemberId = memberId.ToString(), 
                SubscriptionId = subscriptionId.ToString(),
                UserId = userId.ToString()
            });

            Assert.NotNull(changePermission);

            var count = await Database.Events.CountAsync();
            Assert.True(count == 2);


            usersubscription = UserSubscription.LoadFromHistory(await EventStore.GetAllAsync(GuidExtensions.CombineGuids(subscriptionId, memberId), default));
            Assert.NotNull(usersubscription);
            Assert.False(usersubscription.IsJoined);

        }

        [Fact]
        public async Task JoinMemberThenLeave_Valid()
        {
            var userId = Guid.NewGuid();
            var memberId = Guid.NewGuid();
            var subscriptionId = Guid.NewGuid();
            var accountId = Guid.NewGuid();

            var client = new SubscriptionCommandProto.SubscriptionCommand.SubscriptionCommandClient(Factory.CreateGrpcChannel());
            var sendInvitationAsync = await client.JoinMemberAsync(new SubscriptionCommandProto.JoinMemberRequest
            {
                AccountId = accountId.ToString(),
                MemberId = memberId.ToString(),
                Permission = 7,
                SubscriptionId = subscriptionId.ToString(),
                UserId = userId.ToString()
            });

            Assert.NotNull(sendInvitationAsync);

            var joinEvent = await Database.Events.ToListAsync();
            Assert.True(joinEvent.Count == 1);
            Assert.True(joinEvent.First().Type == nameof(MemberJoined));
            Assert.True(System.Text.Json.JsonSerializer.Deserialize<MemberJoinedData>(joinEvent.First().Data).JoinedBy == JoinedBy.Admin);

            var userSubscription = UserSubscription.LoadFromHistory(await EventStore.GetAllAsync(GuidExtensions.CombineGuids(subscriptionId, memberId), default));
            Assert.NotNull(userSubscription);
            Assert.True(userSubscription.Permission == (Permissions)7);
            Assert.True(userSubscription.IsJoined);

            // leave 

            var leaveRequest = await client.LeaveAsync(new SubscriptionCommandProto.LeaveRequest
            {
                AccountId = accountId.ToString(),
                MemberId = memberId.ToString(),
                SubscriptionId = subscriptionId.ToString()
            });

            Assert.NotNull(leaveRequest);

            var count = await Database.Events.CountAsync();
            Assert.True(count == 2);


            userSubscription = UserSubscription.LoadFromHistory(await EventStore.GetAllAsync(GuidExtensions.CombineGuids(subscriptionId, memberId), default));
            Assert.NotNull(userSubscription); 
            Assert.False(userSubscription.IsJoined);

        }


    }
}
