using System;

namespace FollowUP.Core.Domain
{
    public class InstagramProxy
    {
        public Guid Id { get; protected set; }
        public string Ip { get; protected set; }
        public string Port { get; protected set; }
        public string Username { get; protected set; }
        public string Password { get; protected set; }
        public DateTime ExpiryDate { get; protected set; }

        protected InstagramProxy()
        {
        }

        public InstagramProxy(Guid id, string ip, string port, string username, string password, DateTime expiryDate)
        {
            Id = id;
            Ip = ip;
            Port = port;
            Username = username;
            Password = password;
            ExpiryDate = expiryDate;
        }
    }
}
