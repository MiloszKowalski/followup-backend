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
        public string Proxy { get; protected set; }
        public AuthenticationStep AuthenticationStep { get; protected set; }
        public DateTime CommentsModuleExpiry { get; protected set; }
        public DateTime PromotionsModuleExpiry { get; protected set; }

        protected InstagramAccount()
        {
        }

        public InstagramAccount(Guid id, User user, string username, string password, string proxy)
        {
            Id = id;
            UserId = user.Id;
            Username = username;
            Password = password;
            Proxy = proxy;
            AuthenticationStep = AuthenticationStep.NotAuthenticated;
            FilePath = $@"Accounts\{user.Username}\{username}-state.bin";
            CommentsModuleExpiry = DateTime.UtcNow;
            PromotionsModuleExpiry = DateTime.UtcNow;
        }

        public void SetAuthenticationStep(AuthenticationStep step)
        {
            AuthenticationStep = step;
        }

        public void BuyComments(double days)
        {
            if (CommentsModuleExpiry > DateTime.UtcNow)
                CommentsModuleExpiry = CommentsModuleExpiry.AddDays(days);
            else
                CommentsModuleExpiry = DateTime.UtcNow.AddDays(days);
        }

        public void BuyPromotions(double days)
        {
            if (PromotionsModuleExpiry > DateTime.UtcNow)
                PromotionsModuleExpiry = PromotionsModuleExpiry.AddDays(days);
            else
                PromotionsModuleExpiry = DateTime.UtcNow.AddDays(days);
        }
    }
}
