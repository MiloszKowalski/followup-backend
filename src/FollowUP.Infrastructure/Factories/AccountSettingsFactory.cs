using FollowUP.Core.Domain;
using System;

namespace FollowUP.Infrastructure.Factories
{
    public static class AccountSettingsFactory
    {
        public static AccountSettings GetDefaultAccountSettings(Guid accountId)
        {
            return new AccountSettings(Guid.NewGuid(), accountId);
        }
    }
}
