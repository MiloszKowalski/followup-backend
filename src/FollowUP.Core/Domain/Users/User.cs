using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace FollowUP.Core.Domain
{
    public class User
    {
        private static readonly Regex _usernameRegex =
            new Regex(@"^(?![_.-])(?!.*[_.-]{2})[a-zA-Z0-9._.-]+(?<![_.-])$");

        private static readonly Regex _fullNameRegex =
            new Regex(@"^[a-zA-ZżźćńółęąśŻŹĆĄŚĘŁÓŃ]*\s{1}[a-zA-ZżźćńółęąśŻŹĆĄŚĘŁÓŃ]+(\s[a-zA-ZżźćńółęąśŻŹĆĄŚĘŁÓŃ]*)?$");

        public Guid Id { get; protected set; }
        public string Email { get; protected set; }
        public string Password { get; protected set; }
        public string Salt { get; protected set; }
        public string Username { get; protected set; }
        public string FullName { get; protected set; }
        public string Role { get; protected set; }
        public bool Verified { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }

        public IList<RefreshToken> RefreshTokens { get; set; }
        public IList<InstagramAccount> InstagramAccounts { get; set; }

        protected User()
        {
        }

        public User(Guid userId, string email, string username, string fullName, string role,
            string password, string salt)
        {
            Id = userId;
            SetEmail(email);
            SetUsername(username);
            SetFullName(fullName);
            SetRole(role);
            SetVerified(false);
            SetPassword(password, salt);
            CreatedAt = DateTime.UtcNow;
        }

        public void SetUsername(string username)
        {
            if (Username == username)
            {
                return;
            }

            if (!_usernameRegex.IsMatch(username))
            {
                throw new DomainException(ErrorCodes.InvalidUsername,
                    $"Username: {username} is invalid.");
            }

            if (string.IsNullOrEmpty(username))
            {
                throw new DomainException(ErrorCodes.UsernameIsEmpty,
                    "Username is null or empty.");
            }

            Username = username.ToLowerInvariant();
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetFullName(string fullName)
        {
            if (FullName == fullName)
            {
                return;
            }

            if (!_fullNameRegex.IsMatch(fullName))
            {
                throw new DomainException(ErrorCodes.InvalidFullName,
                    $"Full name: {fullName} is invalid.");
            }

            if (string.IsNullOrEmpty(fullName))
            {
                throw new DomainException(ErrorCodes.InvalidFullName,
                    "Full name is null or empty.");
            }

            FullName = fullName;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetEmail(string email)
        {
            if (Email == email)
            {
                return;
            }

            if (email.Length > 320)
            {
                throw new DomainException(ErrorCodes.InvalidEmail,
                    "Email is too long.");
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                throw new DomainException(ErrorCodes.InvalidEmail,
                    "Email can not be empty.");
            }

            Email = email.ToLowerInvariant();
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetRole(string role)
        {
            if (Role == role)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(role))
            {
                throw new DomainException(ErrorCodes.InvalidRole,
                    "Role can not be empty.");
            }

            if (Role == role)
            {
                return;
            }

            Role = role;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetVerified(bool isVerified)
        {
            if (Verified == isVerified)
            {
                return;
            }

            Verified = isVerified;
        }

        public void SetPassword(string password, string salt)
        {
            if (Password == password && Salt == salt)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new DomainException(ErrorCodes.InvalidPassword,
                    "Password can not be empty.");
            }

            if (string.IsNullOrWhiteSpace(salt))
            {
                throw new DomainException(ErrorCodes.InvalidPassword,
                    "Salt can not be empty.");
            }

            if (password.Length < 8)
            {
                throw new DomainException(ErrorCodes.InvalidPassword,
                    "Password must contain at least 8 characters.");
            }

            if (password.Length > 100)
            {
                throw new DomainException(ErrorCodes.InvalidPassword,
                    "Password can not contain more than 100 characters.");
            }

            Password = password;
            Salt = salt;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
