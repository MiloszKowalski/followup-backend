using System;

namespace FollowUP.Core.Domain
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Token { get; set; }
        public string UserAgent { get; set; }
        public bool Revoked { get; set; }
    }
}
