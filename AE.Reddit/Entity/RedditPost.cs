using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AE.Reddit.Entity
{
    /// <summary>
    /// Parsed from original reddit data. Not stored in DB.
    /// </summary>
    [DebuggerDisplay("{Id}, {Title}")]
    public class RedditPost
    {
        #region properties
        public string Id { get; set; }
        public int Score { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Permalink { get; set; }
        public double Created_Utc { get; set; }
        #endregion

        #region comparison overloads
        public static bool operator ==(RedditPost leftHandSide, RedditPost rightHandSide)
        {
            return leftHandSide.Id == rightHandSide.Id;
        }
        public static bool operator !=(RedditPost leftHandSide, RedditPost rightHandSide)
        {
            return leftHandSide.Id != rightHandSide.Id;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            RedditPost rp = obj as RedditPost;
            if ((object)rp == null)
            {
                return false;
            }
            return ((RedditPost)obj).Id == Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        #endregion
    }
}
