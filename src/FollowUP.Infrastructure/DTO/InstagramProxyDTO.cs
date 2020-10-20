using System;

namespace FollowUP.Infrastructure.DTO
{
    public class InstagramProxyDto
    {
        public Guid Id { get; protected set; }

        public string Ip { get; protected set; }
        public string Port { get; protected set; }
        public string Username { get; protected set; }
        public string Password { get; protected set; }

        public bool IsTaken { get; protected set; }

        public DateTime ExpiryDate { get; protected set; }
    }
}
