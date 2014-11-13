using AE.News.Abstract;
using AE.News.Entity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AE.News
{
    /// <summary>
    /// http://yle.fi/uutiset/rss-syotteet/6075085
    /// </summary>
    internal class YleFeeds : IFeed
    {
        public async Task<IEnumerable<NewsArticle>> Fetch()
        {
            List<NewsArticle> all = new List<NewsArticle>();
            foreach (var feed in Feeds)
            {
                foreach(var article in await FetchFeed(feed))
                {
                    if (!all.Any(a => a.Hash == article.Hash))
                    {
                        // new article
                        all.Add(article);
                    }
                    else
                    {
                        // already have it, add tag to first
                        all.First(a => a.Hash == article.Hash).Tag.Add(feed.Tag);
                    }
                }
            }
            return all;
        }

        private IEnumerable<NewsFeed> Feeds
        {
            get
            {
                return new List<NewsFeed>()
                {
                    new NewsFeed { Tag = "In English", Uri = new Uri("http://yle.fi/uutiset/rss/uutiset.rss?osasto=news")},
                    new NewsFeed { Tag = "Pääuutiset", Uri = new Uri("http://yle.fi/uutiset/rss/paauutiset.rss")},
                    new NewsFeed { Tag = "Uusimmat", Uri = new Uri("http://yle.fi/uutiset/rss/uutiset.rss")},
                    new NewsFeed { Tag = "Kotimaa", Uri = new Uri("http://yle.fi/uutiset/rss/uutiset.rss?osasto=kotimaa")},
                    new NewsFeed { Tag = "Ulkomaat", Uri = new Uri("http://yle.fi/uutiset/rss/uutiset.rss?osasto=ulkomaat")},
                    new NewsFeed { Tag = "Talous", Uri = new Uri("http://yle.fi/uutiset/rss/uutiset.rss?osasto=talous")},
                    new NewsFeed { Tag = "Politiikka", Uri = new Uri("http://yle.fi/uutiset/rss/uutiset.rss?osasto=politiikka")},
                    new NewsFeed { Tag = "Kulttuuri", Uri = new Uri("http://yle.fi/uutiset/rss/uutiset.rss?osasto=kulttuuri")},
                    new NewsFeed { Tag = "Viihde", Uri = new Uri("http://yle.fi/uutiset/rss/uutiset.rss?osasto=viihde")},
                    new NewsFeed { Tag = "Tiede", Uri = new Uri("http://yle.fi/uutiset/rss/uutiset.rss?osasto=tiede")},
                    new NewsFeed { Tag = "Luonto", Uri = new Uri("http://yle.fi/uutiset/rss/uutiset.rss?osasto=luonto")},
                    new NewsFeed { Tag = "Terveys", Uri = new Uri("http://yle.fi/uutiset/rss/uutiset.rss?osasto=terveys")},
                    new NewsFeed { Tag = "Media", Uri = new Uri("http://yle.fi/uutiset/rss/uutiset.rss?osasto=media")},
                    new NewsFeed { Tag = "Urheilu",  Uri = new Uri("http://yle.fi/urheilu/rss/paauutiset.rss")},
                    new NewsFeed { Tag = "Internet", Uri = new Uri("http://yle.fi/uutiset/rss/uutiset.rss?osasto=internet")},
                    new NewsFeed { Tag = "Pelit", Uri = new Uri("http://yle.fi/uutiset/rss/uutiset.rss?osasto=pelit")},
                    new NewsFeed { Tag = "Näkökulmat", Uri = new Uri("http://yle.fi/uutiset/rss/uutiset.rss?osasto=nakokulmat")},
                    new NewsFeed { Tag = "Ilmiöt", Uri = new Uri("http://yle.fi/uutiset/rss/uutiset.rss?osasto=ilmiot")},
                    new NewsFeed { Tag = "Blogit", Uri = new Uri("http://yle.fi/uutiset/rss/uutiset.rss?osasto=Blogi")},
                    new NewsFeed { Tag = "Oulu", Uri = new Uri("http://yle.fi/uutiset/rss/uutiset.rss?osasto=oulu")},
                    new NewsFeed { Tag = "Perämeri", Uri = new Uri("http://yle.fi/uutiset/rss/uutiset.rss?osasto=perameri")},
                    new NewsFeed { Tag = "Lappi", Uri = new Uri("http://yle.fi/uutiset/rss/uutiset.rss?osasto=lappi")}
                };
            }
        }

        private async Task<IEnumerable<NewsArticle>> FetchFeed(NewsFeed feed)
        {
            List<NewsArticle> news = new List<NewsArticle>();
            try
            {
                // get rss feed
                WebRequest req = WebRequest.Create(feed.Uri);
                WebResponse response = await req.GetResponseAsync();
                XDocument xml = XDocument.Load(response.GetResponseStream());
                // pickup only item's (news articles)
                var data = from item in xml.Descendants("item")
                           select item;
                // create list of em
                foreach (var d in data)
                {
                    NewsArticle a = new NewsArticle();
                    // title
                    if (d.Element("title") != null)
                    {
                        a.Title = d.Element("title").Value;
                    }
                    // description
                    if (d.Element("description") != null)
                    {
                        a.Description = d.Element("description").Value;
                    }
                    // date
                    if (d.Element("pubDate") != null)
                    {
                        // <pubDate>Thu, 9 Oct 2014 13:22:17 +0300</pubDate>
                        // http://msdn.microsoft.com/en-us/library/8kb3ddd4(v=vs.110).aspx
                        const string format = "ddd, d MMM yyyy HH:mm:ss zzz";
                        try
                        {
                            a.Date = DateTime.ParseExact(d.Element("pubDate").Value, format, CultureInfo.InvariantCulture);
                        }
                        catch (FormatException e)
                        {
                            Debug.WriteLine(e.Message);
                            a.Date = DateTime.Now;
                        }
                    }
                    else
                    {
                        a.Date = DateTime.Now;
                    }
                    // content
                    if (d.Element(XName.Get("encoded", "http://purl.org/rss/1.0/modules/content/")) != null)
                        a.Content = d.Element(XName.Get("encoded", "http://purl.org/rss/1.0/modules/content/")).Value;
                    // image
                    if (d.Element("enclosure") != null && d.Element("enclosure").HasAttributes)
                    {
                        string imageUrl = (string)d.Element("enclosure").Attribute("url");
                        try
                        {
                            a.Image = new Uri(imageUrl);
                        }
                        catch (AggregateException)
                        {
                            a.Image = null;
                        }
                    }

                    // link to whole story
                    if (d.Element("guid") != null)
                    {
                        try
                        {
                            a.Source = new Uri(d.Element("guid").Value);
                        }
                        catch (AggregateException)
                        {

                        }
                    }
                    a.Tag.Add(feed.Tag);
                    a.Hash = a.Title.GetHashCode();
                    news.Add(a);
                }
            } catch (Exception e)
            {
                Debug.WriteLine("YleFeeds - FetchFeed - Fatal exception: {0}", e.Message);
            }
            return news;
        }
    }
}
