using InstagramApiSharp.API;
using System;
using System.Collections.Generic;

namespace InstagramApiSharp.Classes.Models
{
    [Serializable]
    public class FeedViewInfo
    {
        public string MediaId { get; set; }
        public int Version { get; set; }
        public double MediaPct { get; set; }
        public Dictionary<string, int> TimeInfo { get; set; }

        public FeedViewInfo(string mediaId, double mediaPct)
        {
            MediaId = mediaId;
            Version = InstaApiConstants.FEED_VIEW_INFO_VERSION;
            MediaPct = mediaPct;
            TimeInfo = GetRandomTimeInfo();
        }

        private Dictionary<string, int> GetRandomTimeInfo()
        {
            var random = new Random();
            var timeInfo = new Dictionary<string, int>();
            var stareDelay = InstaApiConstants.MAX_TIMELINE_STARING_MILLISECONDS;
            var tempTime = random.Next(0, stareDelay);
            timeInfo.Add("10", tempTime);
            if (random.Next(0, 100) <= 25) tempTime = random.Next(0, stareDelay);
            timeInfo.Add("25", tempTime);
            if (random.Next(0, 100) <= 25) tempTime = random.Next(0, stareDelay);
            timeInfo.Add("50", tempTime);
            if (random.Next(0, 100) <= 25) tempTime = random.Next(0, stareDelay);
            timeInfo.Add("75", tempTime);

            return timeInfo;
        }
    }
}
