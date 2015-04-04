using System.Diagnostics;

namespace AE.News.Entity
{
    [DebuggerDisplay("{FeedId} {Url}")]
    public class Feed
    {
        public int FeedId { get; set; }
        public string Url { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
