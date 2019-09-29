using System;

namespace FollowUP.Infrastructure.DTO
{
    public class AccountDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string AuthenticationStep { get; set; }
    }
}
