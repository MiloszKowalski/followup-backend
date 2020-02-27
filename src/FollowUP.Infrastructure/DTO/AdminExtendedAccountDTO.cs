using System;

namespace FollowUP.Infrastructure.DTO
{
    public class AdminExtendedAccountDto : ExtendedAccountDto
    {

        public string OwnerName { get; set; }
        public string OwnerEmail { get; set; }
        public Guid ProxyId { get; set; }
        public string ProxyIp { get; set; }
        public DateTime ProxyExpiryDate { get; set; }
    }
}
