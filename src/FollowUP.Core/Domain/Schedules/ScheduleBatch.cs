using System;

namespace FollowUP.Core.Domain
{
    public class ScheduleBatch
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string Name { get; set; }
        public BatchColour Colour { get; set; }
    }
}
