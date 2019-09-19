using System;
using System.Collections.Generic;
using System.Text;

namespace FollowUP.Infrastructure.DTO
{
    public class AccountDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string AuthenticationStep { get; set; }
    }
}
