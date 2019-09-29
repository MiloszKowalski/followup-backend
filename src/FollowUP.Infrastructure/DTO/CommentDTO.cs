using FollowUP.Core.Domain;
using System;
using System.Collections.Generic;

namespace FollowUP.Infrastructure.DTO
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public long AuthorId { get; set; }
        public string Author { get; set; }
        public string ProfilePictureUri { get; set; }
        public string Content { get; set; }
        public int LikesCount { get; set; }
        public List<ChildComment> ChildComments { get; set; }
        public string ParentImageUri { get; set; }
        public string ParentMediaId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}
