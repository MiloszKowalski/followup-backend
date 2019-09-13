using System;

namespace FollowUP.Infrastructure.Commands.Accounts
{
    public class Register : ICommand
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
    }
}
