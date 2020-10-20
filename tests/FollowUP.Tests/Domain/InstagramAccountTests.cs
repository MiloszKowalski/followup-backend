using FollowUP.Core.Domain;
using FollowUP.Infrastructure.Services;
using FollowUP.Infrastructure.Settings;
using System;
using Xunit;

namespace FollowUP.Tests.Domain
{
    public class InstagramAccountTests
    {
        private readonly static IAesEncryptor _aesEncryptor =
            new AesEncryptor(new AesSettings { Password = "1234" });

        [Fact]
        public void SetUsername_ValidUsernameShouldWork()
        {
            var account = GetDefaultValidAccount(GetDefaultValidUser());
            account.SetUsername("username2");
            Assert.Equal("username2", account.Username);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("2username")]
        [InlineData("user name")]
        [InlineData("AnObviouslyTooLongUsernameToBeEvenConsideredAsValid")]
        public void SetUsername_InvalidUsernameShouldThrow(string username)
        {
            var account = GetDefaultValidAccount(GetDefaultValidUser());
            Assert.Throws<DomainException>(() => account.SetUsername(username));
        }

        [Fact]
        public void SetPassword_ValidPasswordShouldWork()
        {
            var account = GetDefaultValidAccount(GetDefaultValidUser());
            account.SetPassword(_aesEncryptor.Encrypt("!QAZXSW@secret"));
            Assert.Equal(_aesEncryptor.Encrypt("!QAZXSW@secret"), account.Password);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("!amAv3rySec@e7Pa%sw$rd1234567890")]
        public void SetPassword_InvalidPasswordShouldThrow(string password)
        {
            var account = GetDefaultValidAccount(GetDefaultValidUser());
            var encrypted = _aesEncryptor.Encrypt(password);
            Action action = () => account.SetPassword(encrypted);
            Assert.Throws<DomainException>(action);
        }

        private User GetDefaultValidUser()
        {
            return new User(Guid.NewGuid(), "user@example.com",
                "username", "User Name", "user", "password", "salt");
        }

        private InstagramAccount GetDefaultValidAccount(User user)
        {
            return new InstagramAccount(Guid.NewGuid(), user.Id, "123456789",
                user.Username, "username", "password", "lg-k10");
        }
    }
}
