using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace FollowUP.Core.Domain
{
    public class InstagramAccount
    {
        private readonly Regex _usernameRegex = new Regex("^[a-zA-Z][a-zA-Z1-9.]*$");

        public Guid Id { get; protected set; }

        public string AndroidDevice { get; protected set; }
        public string FilePath { get; protected set; }
        public string Password { get; protected set; }
        public string PhoneNumber { get; protected set; }
        public string Pk { get; protected set; }
        public string Username { get; protected set; }

        public DateTime ActionCooldown { get; protected set; }
        public DateTime BannedUntil { get; protected set; }
        public DateTime CommentsModuleExpiry { get; protected set; }
        public DateTime PromotionsModuleExpiry { get; protected set; }

        public int PreviousCooldownMilliseconds { get; protected set; }

        public AccountSettings AccountSettings { get; set; }
        public InstagramProxy InstagramProxy { get; set; }
        public Guid UserId { get; protected set; }
        public User User { get; set; }

        public IList<AccountStatistics> AccountStatistics { get; set; }
        public IList<Comment> Comments { get; set; }
        public IList<ExplicitDaySchedule> ExplicitDaySchedules { get; set; }
        public IList<FollowedProfile> FollowedProfiles { get; set; }
        public IList<MonthlyGroupSchedule> MonthlyGroupSchedules { get; set; }
        public IList<FollowPromotion> FollowPromotions { get; set; }
        public IList<PromotionComment> PromotionComments { get; set; }
        public IList<ScheduleGroup> ScheduleGroups { get; set; }
        public IList<SingleScheduleDay> SingleScheduleDays { get; set; }

        protected InstagramAccount()
        {
        }

        public InstagramAccount(Guid id, Guid userId, string pk, string usersUsername,
            string username, string password, string androidDevice)
        {
            Id = id;
            UserId = userId;
            SetPk(pk);
            SetUsername(username);
            SetPassword(password);
            SetAndroidDevice(androidDevice);
            SetFilePath(usersUsername, username);
            CommentsModuleExpiry = DateTime.UtcNow;
            PromotionsModuleExpiry = DateTime.UtcNow;
            ActionCooldown = DateTime.UtcNow;
            PreviousCooldownMilliseconds = 0;
        }

        private void SetPk(string pk)
        {
            if (Pk == pk)
            {
                return;
            }

            if (pk == null)
            {
                throw new DomainException(ErrorCodes.ProfileIdIsNull,
                    "The given Profile ID (PK) is null.");
            }

            if (pk == string.Empty)
            {
                throw new DomainException(ErrorCodes.ProfileIdIsEmpty,
                    "The given Profile ID (PK) is empty.");
            }

            if (pk.Length > 15)
            {
                throw new DomainException(ErrorCodes.ProfileIdTooLong,
                    "The given Profile ID (PK) is too long.");
            }

            Pk = pk;
        }

        public void SetUsername(string username)
        {
            if (Username == username)
            {
                return;
            }

            if (username == null)
            {
                throw new DomainException(ErrorCodes.UsernameIsNull,
                    "Account's username is null!");
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new DomainException(ErrorCodes.UsernameIsEmpty,
                    "Account's username is empty!");
            }

            if (username.Length > 35)
            {
                throw new DomainException(ErrorCodes.UsernameTooLong,
                    "Account's username is too long!");
            }

            if (!_usernameRegex.IsMatch(username))
            {
                throw new DomainException(ErrorCodes.InvalidUsername,
                    "Account's username is invalid!");
            }

            Username = username;
        }

        public void SetPassword(string password)
        {
            if (Password == password)
            {
                return;
            }

            if (password == null)
            {
                throw new DomainException(ErrorCodes.PasswordIsNull,
                    "Account's password is null!");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new DomainException(ErrorCodes.PasswordIsEmpty,
                    "Account's password is empty!");
            }

            // Encrypted length => ((len / 8) + 1)*20 + ((len / 64) + 1)*4
            // For 25 characters, max length is 88
            if (password.Length > 88)
            {
                throw new DomainException(ErrorCodes.PasswordTooLong,
                    "Account's password is too long!");
            }

            Password = password;
        }

        public void SetFilePath(string userUsername, string username)
        {
            if (userUsername == null)
            {
                throw new DomainException(ErrorCodes.UsernameIsNull,
                    "User's username is null!");
            }

            if (string.IsNullOrWhiteSpace(userUsername))
            {
                throw new DomainException(ErrorCodes.UsernameIsEmpty,
                    "User's username is empty!");
            }

            if (username == null)
            {
                throw new DomainException(ErrorCodes.UsernameIsNull,
                    "Account's username is null!");
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new DomainException(ErrorCodes.UsernameIsEmpty,
                    "Account's username is empty!");
            }

            FilePath = Path.Combine("Accounts", userUsername, $"{username}-state.bin");
        }

        public void SetBannedUntil(DateTime bannedUntil)
        {
            if (BannedUntil == bannedUntil)
            {
                return;
            }

            BannedUntil = bannedUntil;
        }

        public void BuyComments(double days)
        {
            if (days < 0)
            {
                throw new DomainException(ErrorCodes.NegativeDays,
                    "Days of the comments' order can't be negative!");
            }

            if (CommentsModuleExpiry > DateTime.UtcNow)
            {
                CommentsModuleExpiry = CommentsModuleExpiry.AddDays(days);
            }
            else
            {
                CommentsModuleExpiry = DateTime.UtcNow.AddDays(days);
            }
        }

        public void BuyPromotions(double days)
        {
            if (days < 0)
            {
                throw new DomainException(ErrorCodes.NegativeDays,
                    "Days of the promotions' order can't be negative!");
            }

            if (PromotionsModuleExpiry > DateTime.UtcNow)
            {
                PromotionsModuleExpiry = PromotionsModuleExpiry.AddDays(days);
            }
            else
            {
                PromotionsModuleExpiry = DateTime.UtcNow.AddDays(days);
            }
        }

        public void SetPromotionsModuleExpiry(DateTime promotionsModuleExpiry)
        {
            if (promotionsModuleExpiry < DateTime.UtcNow)
            {
                return;
            }

            PromotionsModuleExpiry = promotionsModuleExpiry;
        }

        public void SetActionCooldown(int milliseconds)
        {
            if (milliseconds < 0)
            {
                return;
            }

            var dateAfterCooldown = DateTime.UtcNow;
            dateAfterCooldown = dateAfterCooldown.AddMilliseconds(milliseconds);

            if (dateAfterCooldown < DateTime.UtcNow)
            {
                return;
            }

            ActionCooldown = dateAfterCooldown;
            PreviousCooldownMilliseconds = milliseconds;
        }

        public void SetAndroidDevice(string name)
        {
            if (AndroidDevice == name)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new DomainException(ErrorCodes.InvalidDevice,
                    "Device's name cannot be empty!");
            }

            AndroidDevice = name;
        }
    }
}
