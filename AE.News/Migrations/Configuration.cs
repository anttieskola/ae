namespace AE.News.Migrations
{
    using Dal;
    using Entity;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    using System.Linq;

    /// <summary>
    /// helper to run code migrations from code
    /// </summary>
    public sealed class Run
    {
        public static void Migration()
        {
            var c = new Configuration();
            var m = new DbMigrator(c);
            m.Update();
        }
    }

    internal sealed class Configuration : DbMigrationsConfiguration<Dal.NewsContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(NewsContext db)
        {
            // Add default feeds to database
            if (db.Feeds.Count() == 0 && db.Tags.Count() == 0)
            {
                db.Feeds.AddRange(DEFAULT_FEEDS);
                db.SaveChanges();
            }
        }

        List<Feed> DEFAULT_FEEDS
        {
            get
            {
                return new List<Feed>
                {
                    new Feed { Tag = new Tag { Name = "In English"}, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=news" },
                    new Feed { Tag = new Tag { Name = "In Pääuutiset"}, Url = "http://yle.fi/uutiset/rss/paauutiset.rss" },
                    new Feed { Tag = new Tag { Name = "Kotimaa" }, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=kotimaa" },
                    new Feed { Tag = new Tag { Name = "Ulkomaat" }, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=ulkomaat" },
                    new Feed { Tag = new Tag { Name = "Talous" }, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=talous" },
                    new Feed { Tag = new Tag { Name = "Politiikka" }, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=politiikka" },
                    new Feed { Tag = new Tag { Name = "Kulttuuri" }, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=kulttuuri" },
                    new Feed { Tag = new Tag { Name = "Viihde" }, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=viihde" },
                    new Feed { Tag = new Tag { Name = "Tiede" }, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=tiede" },
                    new Feed { Tag = new Tag { Name = "Luonto" }, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=luonto" },
                    new Feed { Tag = new Tag { Name = "Terveys" }, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=terveys" },
                    new Feed { Tag = new Tag { Name = "Media" }, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=media" },
                    new Feed { Tag = new Tag { Name = "Urheilu" }, Url = "http://yle.fi/urheilu/rss/paauutiset.rss" },
                    new Feed { Tag = new Tag { Name = "Internet" }, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=internet" },
                    new Feed { Tag = new Tag { Name = "Pelit" }, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=pelit" },
                    new Feed { Tag = new Tag { Name = "Näkökulmat" }, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=nakokulmat" },
                    new Feed { Tag = new Tag { Name = "Ilmiöt" }, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=ilmiot" },
                    new Feed { Tag = new Tag { Name = "Blogit" }, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=Blogi" },
                    new Feed { Tag = new Tag { Name = "Oulu" }, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=oulu" },
                    new Feed { Tag = new Tag { Name = "Perämeri" }, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=perameri" },
                    new Feed { Tag = new Tag { Name = "Lappi" }, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=lappi" },
                    new Feed { Tag = new Tag { Name = "Keski-Pohjanmaa"}, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=keski-pohjanmaa" }
                };
            }
        }
    }
}
