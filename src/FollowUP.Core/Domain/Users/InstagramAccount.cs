using System;
using System.IO;

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
        public string AndroidDevice { get; protected set; }
        public AuthenticationStep AuthenticationStep { get; protected set; }
        public DateTime CommentsModuleExpiry { get; protected set; }
        public DateTime PromotionsModuleExpiry { get; protected set; }
        public DateTime BannedUntil { get; protected set; }
        public DateTime ActionCooldown { get; protected set; }
        public int PreviousCooldownMilliseconds { get; protected set; }


        protected InstagramAccount()
        {
        }

        public InstagramAccount(Guid id, User user, string username, string password, string androidDevice)
        {
            Id = id;
            SetUserId(user.Id);
            SetUsername(username);
            SetPassword(password);
            SetAndroidDevice(androidDevice);
            SetFilePath(user, username);
            AuthenticationStep = AuthenticationStep.NotAuthenticated;
            CommentsModuleExpiry = DateTime.UtcNow;
            PromotionsModuleExpiry = DateTime.UtcNow;
            ActionCooldown = DateTime.UtcNow;
            PreviousCooldownMilliseconds = 0;
            SetBannedUntil(new DateTime(1970, 1, 1));
        }

        private void SetUserId(Guid userId)
        {
            if (userId == null)
                throw new DomainException(ErrorCodes.GuidIsNull, "The given User ID is null.");

            if (userId == Guid.Empty)
                throw new DomainException(ErrorCodes.GuidIsEmpty, "The given User ID is empty.");

            UserId = userId;
        }

        public void SetUsername(string username)
        {
            if (username == null)
                throw new DomainException(ErrorCodes.UsernameIsNull, "Account's username is null!");

            if (string.IsNullOrWhiteSpace(username))
                throw new DomainException(ErrorCodes.UsernameIsEmpty, "Account's username is empty!");

            if (username.Length > 100)
                throw new DomainException(ErrorCodes.UsernameTooLong, "Account's username is too long!");

            if (username == Username)
                return;

            Username = username;
        }

        public void SetPassword(string password)
        {
            if (password == null)
                throw new DomainException(ErrorCodes.PasswordIsNull, "Account's password is null!");

            if (string.IsNullOrWhiteSpace(password))
                throw new DomainException(ErrorCodes.PasswordIsEmpty, "Account's password is empty!");

            if (password.Length > 100)
                throw new DomainException(ErrorCodes.PasswordTooLong, "Account's password is too long!");

            if (password == Password)
                return;

            Password = password;
        }

        public void SetFilePath(User user, string username)
        {
            if (user.Username == null)
                throw new DomainException(ErrorCodes.UsernameIsNull, "User's username is null!");

            if (string.IsNullOrWhiteSpace(user.Username))
                throw new DomainException(ErrorCodes.UsernameIsEmpty, "User's username is empty!");

            if (username == null)
                throw new DomainException(ErrorCodes.UsernameIsNull, "Account's username is null!");

            if (string.IsNullOrWhiteSpace(username))
                throw new DomainException(ErrorCodes.UsernameIsEmpty, "Account's username is empty!");


            FilePath = Path.Combine("Accounts", user.Username, $"{username}-state.bin");
        }

        public void SetBannedUntil(DateTime bannedUntil)
        {
            if (bannedUntil == BannedUntil)
                return;

            BannedUntil = bannedUntil;
        }

        public void SetAuthenticationStep(AuthenticationStep step)
        {
            if (step < 0)
                throw new DomainException(ErrorCodes.NegativeEnum, "AuthenticationStep cannot be negative.");

            if (string.IsNullOrEmpty(Enum.GetName(typeof(AuthenticationStep), step)))
                throw new DomainException(ErrorCodes.EnumOutOfBounds, "Given AuthenticationStep doesn't exist.");

            if (step == AuthenticationStep)
                return;

            AuthenticationStep = step;
        }

        public void BuyComments(double days)
        {
            if (days < 0)
                throw new DomainException(ErrorCodes.NegativeDays, "Days of the comments' order can't be negative!");

            if (CommentsModuleExpiry > DateTime.UtcNow)
                CommentsModuleExpiry = CommentsModuleExpiry.AddDays(days);
            else
                CommentsModuleExpiry = DateTime.UtcNow.AddDays(days);
        }

        public void BuyPromotions(double days)
        {
            if (days < 0)
                throw new DomainException(ErrorCodes.NegativeDays, "Days of the promotions' order can't be negative!");

            if (PromotionsModuleExpiry > DateTime.UtcNow)
                PromotionsModuleExpiry = PromotionsModuleExpiry.AddDays(days);
            else
                PromotionsModuleExpiry = DateTime.UtcNow.AddDays(days);
        }

        public void SetPromotionsModuleExpiry(DateTime promotionsModuleExpiry)
        {
            if (promotionsModuleExpiry < DateTime.UtcNow)
                return;

            PromotionsModuleExpiry = promotionsModuleExpiry;
        }

        public void SetActionCooldown(int milliseconds)
        {
            if (milliseconds < 0)
                return;

            var dateAfterCooldown = DateTime.UtcNow;
            dateAfterCooldown = dateAfterCooldown.AddMilliseconds(milliseconds);

            if (dateAfterCooldown < DateTime.UtcNow)
                return;

            ActionCooldown = dateAfterCooldown;
            PreviousCooldownMilliseconds = milliseconds;
        }
        public void SetAndroidDevice(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException(ErrorCodes.InvalidDevice, "Device's name cannot be empty!");

            AndroidDevice = name;
        }
    }
}
