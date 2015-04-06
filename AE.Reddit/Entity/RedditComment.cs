using System.Diagnostics;

namespace AE.Reddit.Entity
{
    [DebuggerDisplay("{Body}")]
    public class RedditComment
    {
        public string Body { get; set; }
    }
}
