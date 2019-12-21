using System;

namespace FollowUP.Infrastructure.DTO
{
    public class PromotionCommentDto
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
