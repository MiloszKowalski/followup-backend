﻿using System;

namespace FollowUP.Infrastructure.Commands.Accounts
{
    public class Login : ICommand
    {
        public Guid TokenId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserAgent { get; set; }
    }
}
