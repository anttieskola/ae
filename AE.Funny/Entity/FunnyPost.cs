using System;
using System.Collections.Generic;

namespace AE.Funny.Entity
{
    /// <summary>
    /// FunnyPost, purpose is to cause you to smile
    /// </summary>
    public class FunnyPost
    {
        public FunnyPost()
        {
            Comments = new List<string>();
        }
        public int FunnyPostId { get; set; }
        public string RedditId { get; set; }
        public string Title { get; set; }
        public String ImageUrl { get; set; }
        public List<string> Comments { get; set; }
    }
}
