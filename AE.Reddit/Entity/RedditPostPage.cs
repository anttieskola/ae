﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AE.Reddit.Entity
{
    public class RedditPostPage
    {
        public RedditPostPage()
        {
            Posts = new HashSet<RedditPost>();
        }
        public string After { get; set; }
        public string Before { get; set; }
        public HashSet<RedditPost> Posts { get; set; }
    }
}
