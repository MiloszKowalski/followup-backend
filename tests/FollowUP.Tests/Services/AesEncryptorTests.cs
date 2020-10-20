using FollowUP.Infrastructure.Exceptions;
using FollowUP.Infrastructure.Services;
using FollowUP.Infrastructure.Settings;
using System;
using Xunit;

namespace FollowUP.Tests
{
    public class AesEncryptorTests
    {
        private readonly AesSettings settings = new AesSettings { Password = "1234" };

        [Fact]
        void DecryptedStringShouldBeSameAsOriginal()
        {
            var aes = new AesEncryptor(settings);
            var encrypted = aes.Encrypt("supersecret");
            var decrypted = aes.Decrypt(encrypted);
            Assert.Equal("supersecret", decrypted);
        }

        [Fact]
        void ShouldWorkWithNonAlphanumericValues()
        {
            var aes = new AesEncryptor(settings);
            var encrypted = aes.Encrypt("!@#$%^&*(). secret");
            var decrypted = aes.Decrypt(encrypted);
            Assert.Equal("!@#$%^&*(). secret", decrypted);
        }

        [Fact]
        void Encrypt_ShouldThrowOnEmptyString()
        {
            var aes = new AesEncryptor(settings);
            Action action = () => aes.Encrypt("");
            Assert.Throws<ServiceException>(action);
        }
    }
}
