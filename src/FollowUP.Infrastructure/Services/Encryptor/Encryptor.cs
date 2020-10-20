using FollowUP.Infrastructure.Extensions;
using System;
using System.Security.Cryptography;

namespace FollowUP.Infrastructure.Services
{
    public class Encryptor : IEncryptor
    {
        private static readonly int _deriveBytesIterationsCount = 10000;
        private static readonly int _saltSize = 40;

        public string GetSalt(string value)
        {
            if (value.Empty())
            {
                throw new ArgumentException("Can not generate salt from an empty value.", nameof(value));
            }

            var saltBytes = new byte[_saltSize];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(saltBytes);

            return Convert.ToBase64String(saltBytes);
        }

        public string GetHash(string value, string salt)
        {
            if (value.Empty())
            {
                throw new ArgumentException("Can not generate hash from an empty value.", nameof(value));
            }
            if (salt.Empty())
            {
                throw new ArgumentException("Can not use an empty salt from hashing value.", nameof(value));
            }

            var pbkdf2 = new Rfc2898DeriveBytes(value, GetBytes(salt), _deriveBytesIterationsCount);

            return Convert.ToBase64String(pbkdf2.GetBytes(_saltSize));
        }

        private static byte[] GetBytes(string value)
        {
            var bytes = new byte[value.Length * sizeof(char)];
            Buffer.BlockCopy(value.ToCharArray(), 0, bytes, 0, bytes.Length);

            return bytes;
        }
    }
}
