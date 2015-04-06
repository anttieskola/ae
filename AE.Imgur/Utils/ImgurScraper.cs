using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AE.Imgur.Utils
{
    public class ImgurScraper
    {
        /// <summary>
        /// This will acquire direct link to image by parsing
        /// the default image page.
        /// 
        /// Throws WebException in case of error
        /// </summary>
        /// <param name="urlOrId">Full url to imgur or just image id</param>
        /// <returns>image link or null</returns>
        public static async Task<string> GetImageUrl(string urlOrId)
        {
            string url = "";
            if (urlOrId.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) ||
                urlOrId.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
            {
                url = urlOrId;
            }
            else
            {
                url += "http://imgur.com/";
                url += urlOrId;
            }
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            req.Timeout = 3000;
            try
            {
                // get response and handle result
                using (HttpWebResponse res = (HttpWebResponse)await req.GetResponseAsync())
                {
                    // might be unnecessary check
                    if (res.StatusCode == HttpStatusCode.OK)
                    {
                        using (StreamReader sr = new StreamReader(res.GetResponseStream()))
                        {
                            string content = sr.ReadToEnd();
                            // <link rel="image_src" href="url"/>
                            int image_src = content.IndexOf("link rel=\"image_src\"");
                            if (image_src != -1)
                            {
                                // start and end indexes
                                int start = content.IndexOf("href=", image_src) + 6;
                                int end = content.IndexOf("\"", start);
                                // small sanity check
                                if (end - start > 0 && end - start < 40)
                                {
                                    string image_url = content.Substring(start, end - start);
                                    return removeParameters(image_url);
                                }
                            }
                            // <meta property="og:image" content="url" />
                            int og_image = content.IndexOf("property=\"og:image\"");
                            if (og_image != -1)
                            {
                                // start and end indexes
                                int start = content.IndexOf("content=", og_image) + 9;
                                int end = content.IndexOf("\"", start);
                                // small sanity check
                                if (end - start > 0 && end - start < 40)
                                {
                                    string image_url = content.Substring(start, end - start);
                                    return removeParameters(image_url);
                                }
                            }
                        }
                    }
                }
            }
            catch (WebException)
            {
                throw;
            }
            return null;
        }

        /// <summary>
        /// Simply if string contains ? remove that all rest
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        internal static string removeParameters(String s)
        {
            int q = s.IndexOf('?');
            if (q != -1)
            {
                s = s.Substring(0, q);
            }
            return s;
        }
    }
}
