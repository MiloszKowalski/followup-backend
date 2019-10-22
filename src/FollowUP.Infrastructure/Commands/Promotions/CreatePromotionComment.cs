using System;

namespace FollowUP.Infrastructure.Commands
{
    public class CreatePromotionComment : AuthenticatedCommandBase
    {
        public Guid AccountId { get; set; }
        public string Content { get; set; }
    }
}
