using FollowUP.Core.Domain;
using System;

namespace FollowUP.Infrastructure.DTO
{
    public class PromotionDto
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string Label { get; set; }
        public PromotionType PromotionType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
