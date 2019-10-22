using FollowUP.Core.Domain;
using System;

namespace FollowUP.Infrastructure.Commands
{
    public class CreatePromotion : AuthenticatedCommandBase
    {
        public Guid AccountId { get; set; }
        public PromotionType PromotionType { get; set; }
        public string Label { get; set; }
    }
}
