using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunicationApi.Interfaces;
using CommunicationApi.Models;

namespace CommunicationApi.Services
{
    public class HardcodedUserStore : IUserStore
    {
        private static IDictionary<string, UserInfo> _inMemoryUsers = new Dictionary<string, UserInfo>();
        private static List<ActivatedDevice> _inMemoryTenants = new List<ActivatedDevice>();

        public Task<UserInfo> GetUserInfo(string phoneNumber)
        {
            if (_inMemoryUsers.ContainsKey(phoneNumber))
                return Task.FromResult(_inMemoryUsers[phoneNumber]);
            return Task.FromResult(new UserInfo
            {
                ConversationState = ConversationState.New,
                Name = null, PhoneNumber = phoneNumber, BoxInfo = null
            });
        }

        public Task<UserInfo> CreateUser(string phoneNumber, string name, ConversationState conversationState = ConversationState.New)
        {
            var userInfo = new UserInfo
            {
                ConversationState = conversationState,
                Name = name ?? "",
                PhoneNumber = phoneNumber,
                BoxInfo = null
            };
            _inMemoryUsers.Add(phoneNumber, userInfo);
            return Task.FromResult(userInfo);
        }

        public async Task<UserInfo> LinkUserToTenant(string phoneNumber, string tenantId)
        {
            var tenant =
                _inMemoryTenants.FirstOrDefault(t =>
                    t.BoxId.Equals(tenantId, StringComparison.InvariantCultureIgnoreCase));
            if (tenant != null)
            {
                var user = await GetUserInfo(phoneNumber);
                user.BoxInfo = tenant;
                return user;
            }

            return null;
        }

        public Task<IEnumerable<UserInfo>> GetUsersFromTenant(string tenant)
        {
            throw new NotImplementedException();
        }

        public Task UpdateUser(UserInfo userInfo)
        {
            _inMemoryUsers[userInfo.PhoneNumber] = userInfo;
            return Task.CompletedTask;
        }

    }
}