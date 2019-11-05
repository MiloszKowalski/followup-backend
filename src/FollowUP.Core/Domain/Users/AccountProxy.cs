using System;

namespace FollowUP.Core.Domain
{
    public class AccountProxy
    {
        public Guid Id { get; protected set; }
        public Guid ProxyId { get; protected set; }
        public Guid AccountId { get; protected set; }

        protected AccountProxy()
        {
        }

        public AccountProxy(Guid id, Guid proxyId, Guid accountId)
        {
            Id = id;
            ProxyId = proxyId;
            AccountId = accountId;
        }
    }
}
