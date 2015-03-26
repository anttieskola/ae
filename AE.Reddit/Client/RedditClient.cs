using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace AE.Reddit.Client
{
    internal class RedditClient
    {
        // Todo: when needed
        private async Task<bool> Token()
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://www.reddit.com/api/v1/access_token");
            req.Method = "POST";
            req.ContentType = "application/json; charset=utf-8";
            req.Accept = "application/json";
            req.Timeout = 3000;
            req.Credentials = new NetworkCredential("appId", "appSecret");
            string data = JsonConvert.SerializeObject(new
            {
                grant_type = "password", /* note does not work for some reason */
                username = "username",
                password = "password"
            });
            // catch network/request issues
            try
            {
                // write data to request, can already trigger exception if no network
                using (var sw = new StreamWriter(await req.GetRequestStreamAsync()))
                {
                    sw.Write(data);
                    sw.Flush();
                    sw.Close(); // set's content length
                }
                // get response and handle result
                using (HttpWebResponse res = (HttpWebResponse)await req.GetResponseAsync())
                {
                    if (res.StatusCode == HttpStatusCode.OK)
                    {
                        using (StreamReader sr = new StreamReader(res.GetResponseStream()))
                        {
                            string content = sr.ReadToEnd();
                            if (content.Length == 0)
                            {
                                throw new WebException("uh oh");
                            }
                        }
                    }
                }
            }
            catch (WebException we)
            {
                // check response and it status
                if (we.Response != null)
                {
                    using (HttpWebResponse res = (HttpWebResponse)we.Response)
                    {
                        if (res.StatusCode == HttpStatusCode.Forbidden)
                        {
                            // add code
                        }
                        else if (res.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            // add code
                        }
                        else if (res.StatusCode == HttpStatusCode.NotFound)
                        {
                            // add code
                        }
                    }
                }
                else
                {
                    // timeout, no network, not working dns...
                }
            }
            return false;
        }
    }
}
