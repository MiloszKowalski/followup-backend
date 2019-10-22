using System;

namespace FollowUP.Infrastructure.Commands
{
    public class BuyComments : AuthenticatedCommandBase
    {
        public Guid AccountId { get; set; }
        public double DaysToAdd { get; set; }
    }
}
