using AE.Reddit.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AE.Reddit.Utils
{
    public class RedditScraper
    {
        /// <summary>
        /// Parse reddit posts from given url. i.e. http://www.reddit.com/r/worldnews.json
        /// Throws AggregateException as this is task. Should contain WebException with info.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<List<RedditPost>> ParsePosts(string url)
        {
            List<RedditPost> lp = new List<RedditPost>();
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            req.Accept = "application/json";
            req.Timeout = 3000;
            try
            {
                // get response and handle result
                using (HttpWebResponse res = (HttpWebResponse)await req.GetResponseAsync())
                {
                    // might be unnecessary check
                    if (res.StatusCode == HttpStatusCode.OK)
                    {
                        // stream -> json readers
                        using (StreamReader sr = new StreamReader(res.GetResponseStream()))
                        using (JsonTextReader jtr = new JsonTextReader(sr))
                        {
                            // load json
                            JObject all = JObject.Load(jtr);
                            // post array
                            JArray posts = (JArray)all["data"]["children"];
                            // parse posts, Todo: could we somehow avoid loop?
                            foreach (JToken post in posts)
                            {
                                RedditPost p = post["data"].ToObject<RedditPost>();
                                lp.Add(p);
                            }
                        }
                    }
                }
            }
            catch (WebException)
            {
                // Todo: could remove try as we don't handle exception...
                throw;
            }
            return lp;
        }
    }
}
