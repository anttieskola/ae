using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AE.Funny.Entity
{
    /// <summary>
    /// Post to make you laugh
    /// </summary>
    [DebuggerDisplay("{PostId}, {Title}, {RedditId}")]
    public class Post
    {
        public Post()
        {
            Comments = new HashSet<Comment>();
        }
        public int PostId { get; set; }
        public DateTime Created { get; set; }
        public string RedditId { get; set; }
        public string Title { get; set; }
        public String ImageUrl { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
