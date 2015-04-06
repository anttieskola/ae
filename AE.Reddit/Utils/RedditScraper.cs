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

        /// <summary>
        /// Parse comments from given id
        /// 
        /// Todo: Order problem for some yet unknown reason
        /// when we loop the comment array the order of elements
        /// is not same as actuall json file!!!
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<List<RedditComment>> ParseComments(string id)
        {
            List<RedditComment> comments = new List<RedditComment>();
            string url = "http://www.reddit.com/" + id;
            url += ".json";

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
                                // response is array
                                JArray arrayAll = JArray.Load(jtr);
                                // first element is post, that is ignored
                                // second element is comments
                                JToken rootToken = arrayAll[1];
                                getComments(ref comments, rootToken);
                                return comments;
                            }
                            catch (JsonReaderException jre)
                            {
                                Debug.WriteLine(jre.Message);
                                throw new WebException("ParseComments, invalid data in response from " + url);
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
            return null;
        }

        /// <summary>
        /// Gather recursively comments from given comment element
        /// </summary>
        /// <param name="comments"></param>
        /// <param name="comment"></param>
        private static void getComments(ref List<RedditComment> comments, JToken comment)
        {
            // array of comments on current level
            JArray commentArray = (JArray)comment["data"]["children"];
            foreach (JToken c in commentArray)
            {
                // check this is actual comment
                if(c["data"]["body"] == null )
                {
                    continue;
                }
                // add each
                comments.Add(c["data"].ToObject<RedditComment>());
                // check if any have replies
                JToken replies = c["data"]["replies"];
                if (replies != null && replies.Type == JTokenType.Object)
                {
                    // recursion
                    getComments(ref comments, replies);
                }
            }
        }
    }
}
