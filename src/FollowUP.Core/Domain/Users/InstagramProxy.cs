using System;
using System.Text.RegularExpressions;

namespace FollowUP.Core.Domain
{
    public class InstagramProxy
    {
        private readonly Regex IpRegex = new Regex(@"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
        private readonly Regex PortRegex = new Regex(@"^[0-9]{5}$");

        public Guid Id { get; protected set; }
        public string Ip { get; protected set; }
        public string Port { get; protected set; }
        public string Username { get; protected set; }
        public string Password { get; protected set; }
        public DateTime ExpiryDate { get; protected set; }
        public bool IsTaken { get; protected set; }

        protected InstagramProxy()
        {
        }

        public InstagramProxy(Guid id, string ip, string port, string username,
                                string password, DateTime expiryDate)
        {
            Id = id;
            SetIp(ip);
            SetPort(port);
            SetUsername(username);
            SetPassword(password);
            SetExpiryDate(expiryDate);
            SetIsTaken(false);
        }

        public void SetIp(string ip)
        {
            if (!IpRegex.IsMatch(ip))
                throw new DomainException(ErrorCodes.InvalidProxy, "Proxy's IP adress is invalid.");

            Ip = ip;
        }

        public void SetPort(string port)
        {
            if (!PortRegex.IsMatch(port))
                throw new DomainException(ErrorCodes.InvalidProxy, "Proxy's port is invalid.");

            Port = port;
        }

        public void SetUsername(string username)
        {
            if (username == null)
                throw new DomainException(ErrorCodes.UsernameIsNull, "Proxy's username is null!");

            if (string.IsNullOrWhiteSpace(username))
                throw new DomainException(ErrorCodes.UsernameIsEmpty, "Proxy's username is empty!");

            if (username.Length > 128)
                throw new DomainException(ErrorCodes.UsernameTooLong, "Proxy's username is too long!");

            if (username == Username)
                return;

            Username = username;
        }

        public void SetPassword(string password)
        {
            if (password == null)
                throw new DomainException(ErrorCodes.PasswordIsNull, "Proxy's password is null!");

            if (string.IsNullOrWhiteSpace(password))
                throw new DomainException(ErrorCodes.PasswordIsEmpty, "Proxy's password is empty!");

            if (password.Length > 128)
                throw new DomainException(ErrorCodes.PasswordTooLong, "Proxy's password is too long!");

            if (password == Password)
                return;

            Password = password;
        }

        public void SetExpiryDate(DateTime expiryDate)
        {
            if (expiryDate < DateTime.UtcNow)
                throw new DomainException(ErrorCodes.DateFromPast, "Proxy's expiry date is earlier than tommorow's!");

            ExpiryDate = expiryDate;
        }

        public void SetIsTaken(bool value)
        {
            IsTaken = value;
        }
    }
}
