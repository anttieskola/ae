﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AE.Funny.Entity
{
    public class Comment
    {
        public int CommentId { get; set; }
        public string Text { get; set; }
        public virtual Post Post { get; set; }
    }
}
