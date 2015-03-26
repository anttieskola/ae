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
        /// <param name="id"></param>
        /// <returns>image link or null</returns>
        public static async Task<string> GetImageUrl(string id)
        {
            string url = "http://imgur.com/";
            url += id;
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
                                    return image_url;
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
    }
}
