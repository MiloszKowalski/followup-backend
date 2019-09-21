using System;

namespace FollowUP.Core.Domain
{
    public class InstagramAccount
    {
        public Guid Id { get; protected set; }
        public Guid UserId { get; protected set; }
        public string Username { get; protected set; }
        public string Password { get; protected set; }
        public string PhoneNumber { get; protected set; }
        public string FilePath { get; protected set; }
        public AuthenticationStep AuthenticationStep { get; protected set; }

        protected InstagramAccount()
        {
        }

        public InstagramAccount(Guid id, User user, string username, string password)
        {
            Id = id;
            UserId = user.Id;
            Username = username;
            Password = password;
            AuthenticationStep = AuthenticationStep.NotAuthenticated;
            FilePath = $@"Accounts\{user.Username}\{username}-state.bin";
        }

        public void SetAuthenticationStep(AuthenticationStep step)
        {
            AuthenticationStep = step;
        }
    }
}
