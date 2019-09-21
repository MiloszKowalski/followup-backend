using System;
using System.Collections.Generic;

namespace FollowUP.Core.Domain
{
    public class Comment
    {
        public Guid Id { get; protected set; }
        public string ParentMediaId { get; protected set; }
        public string Author { get; protected set; }
        public string Content { get; protected set; }
        public List<Comment> ChildComment { get; protected set; }
        public Comment ParentComment { get; protected set; }
        public int LikesCount { get; protected set; }

        protected Comment() { }


    }
}
