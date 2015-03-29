using AE.Reddit.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public static async Task<RedditPostPage> ParsePosts(string url)
        {
            RedditPostPage rpp = new RedditPostPage();
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
                            try
                            {
                                JObject all = JObject.Load(jtr);
                                rpp.Before = all["data"]["before"].ToObject<string>(); // token to previous page
                                rpp.After = all["data"]["after"].ToObject<string>(); // token to next page
                                                                                     // post array
                                JArray posts = (JArray)all["data"]["children"];
                                // parse posts, Todo: could we somehow avoid loop?
                                foreach (JToken post in posts)
                                {
                                    RedditPost p = post["data"].ToObject<RedditPost>();
                                    rpp.Posts.Add(p);
                                }
                            }
                            catch (JsonReaderException jre)
                            {
                                Debug.WriteLine(jre.Message);
                                throw new WebException("ParsePosts, invalid data in response from " + url);
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
            return rpp;
        }

        // Todo
        public static async Task<List<string>> ParseComments(string urlOrId, int maxAmount)
        {
            List<string> comments = new List<string>();
            comments.Add("first.");
            return comments;
        }
    }
}
