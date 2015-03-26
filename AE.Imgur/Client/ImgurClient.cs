using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AE.Imgur.Client
{
    /// <summary>
    /// Purpose was to get access to api without user interaction
    /// this is only possible if we have a refresh_token already.
    /// 
    /// This can be only acquired with user loggin to site and giving
    /// access rights for the external application.
    /// </summary>
    internal class ImgurClient
    {
        /// <summary>
        /// Todo: Have not found a way to do this programmatically
        /// </summary>
        /// <returns></returns>
        internal async Task Authorize(string client_id)
        {
            string url = "https://api.imgur.com/oauth2/authorize?";
            url += "client_id=" + client_id;
            url += "&response_type=pin";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Timeout = 3000;
            try
            {
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
            catch (WebException)
            {
                throw;
            }
        }


        /// <summary>
        /// Token can be refreshed without user interaction
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        internal async Task RefreshToken(string clientId, string clientSecret, string refreshToken)
        {
            string url = "https://api.imgur.com/oauth2/token";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/json; charset=utf-8";
            req.Accept = "application/json";
            req.Timeout = 3000;
            string data = JsonConvert.SerializeObject(new
            {
                refresh_token = refreshToken,
                grant_type = "refresh_token",
                client_id = clientId,
                client_secret = clientSecret
            });
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
                            // Todo
                            string content = sr.ReadToEnd();
                            if (content.Length == 0)
                            {
                                throw new WebException("uh oh");
                            }
                        }
                    }
                }
            }
            catch (WebException)
            {
                throw;
            }
        }
    }
}
