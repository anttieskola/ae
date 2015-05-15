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
            // we go thru all defined feeds if anyone missing add them
            bool added = false;
            foreach (Feed f in DEFAULT_FEEDS)
            {
                if (!db.Tags.Any(t => t.Name.Equals(f.Tag.Name)))
                {
                    db.Feeds.Add(f);
                    added = true;
                }
            }
            if (added)
            {
                db.SaveChanges();
            }
        }

        List<Feed> DEFAULT_FEEDS
        {
            get
            {
                return new List<Feed>
                {
                    // we want english to be 0 always, as we still used index as a default categorie
                    new Feed { Tag = new Tag { Name = "In English"}, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=news" },

                    // Normal news categories
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
                    new Feed { Tag = new Tag { Name = "Plus"}, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=Plus"},

                    // Finlands area news
                    new Feed { Tag = new Tag { Name = "Etelä-Karjala"}, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=etela-karjala"},
                    new Feed { Tag = new Tag { Name = "Etelä-Savo"}, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=etela-savo"},
                    new Feed { Tag = new Tag { Name = "Helsinki"}, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=yle-helsinki"},
                    new Feed { Tag = new Tag { Name = "Häme"}, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=hame"},
                    new Feed { Tag = new Tag { Name = "Kainuu"}, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=kainuu"},
                    new Feed { Tag = new Tag { Name = "Keski-Pohjanmaa"}, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=keski-pohjanmaa" },
                    new Feed { Tag = new Tag { Name = "Keski-Suomi"}, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=keski-suomi"},
                    new Feed { Tag = new Tag { Name = "Kymenlaakso"}, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=kymenlaakso"},
                    new Feed { Tag = new Tag { Name = "Lahti"}, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=lahti"},
                    new Feed { Tag = new Tag { Name = "Lappi" }, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=lappi" },
                    new Feed { Tag = new Tag { Name = "Oulu" }, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=oulu" },
                    new Feed { Tag = new Tag { Name = "Perämeri" }, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=perameri" },
                    new Feed { Tag = new Tag { Name = "Pohjanmaa"}, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=pohjanmaa"},
                    new Feed { Tag = new Tag { Name = "Pohjois-Karjala"}, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=pohjois-karjala"},
                    new Feed { Tag = new Tag { Name = "Satakunta"}, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=satakunta"},
                    new Feed { Tag = new Tag { Name = "Savo"}, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=savo"},
                    new Feed { Tag = new Tag { Name = "Tampere"}, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=tampere"},
                    new Feed { Tag = new Tag { Name = "Turku"}, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=turku"},
                    // Finnish area news in non finnish
                    new Feed { Tag = new Tag { Name = "Yle Sápmi"}, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=sapmi"},
                    new Feed { Tag = new Tag { Name = "Novosti Yle"}, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=novosti"},
                    new Feed { Tag = new Tag { Name = "Yle Uudizet karjalakse"}, Url = "http://yle.fi/uutiset/rss/uutiset.rss?osasto=karjalakse"},
                    // Sports
                    new Feed { Tag = new Tag { Name = "Urheilu pääuutiset"}, Url = "http://yle.fi/urheilu/rss/paauutiset.rss"},
                    new Feed { Tag = new Tag { Name = "Alppihiihto"}, Url = "http://yle.fi/urheilu/rss/uutiset.rss?osasto=alppihiihto"},
                    new Feed { Tag = new Tag { Name = "Ampumahiihto"}, Url = "http://yle.fi/urheilu/rss/uutiset.rss?osasto=ampumahiihto"},
                    new Feed { Tag = new Tag { Name = "Ampuminen"}, Url = "http://yle.fi/urheilu/rss/uutiset.rss?osasto=ampuminen"},
                    new Feed { Tag = new Tag { Name = "Formula 1"}, Url = "http://yle.fi/urheilu/rss/uutiset.rss?osasto=f1"},
                    new Feed { Tag = new Tag { Name = "Freestyle"}, Url = "http://yle.fi/urheilu/rss/uutiset.rss?osasto=freestyle"},
                    new Feed { Tag = new Tag { Name = "Golf"}, Url = "http://yle.fi/urheilu/rss/uutiset.rss?osasto=golf"},
                    new Feed { Tag = new Tag { Name = "Hiihto"}, Url = "http://yle.fi/urheilu/rss/uutiset.rss?osasto=hiihto"},
                    new Feed { Tag = new Tag { Name = "Jalkapallo"}, Url = "http://yle.fi/urheilu/rss/uutiset.rss?osasto=jalkapallo"},
                    new Feed { Tag = new Tag { Name = "Jääkiekko"}, Url = "http://yle.fi/urheilu/rss/uutiset.rss?osasto=jaakiekko"},
                    new Feed { Tag = new Tag { Name = "Koripallo"}, Url = "http://yle.fi/urheilu/rss/uutiset.rss?osasto=koripallo"},
                    new Feed { Tag = new Tag { Name = "Lentopallo"}, Url = "http://yle.fi/urheilu/rss/uutiset.rss?osasto=lentopallo"},
                    new Feed { Tag = new Tag { Name = "Lumilautailu"}, Url = "http://yle.fi/urheilu/rss/uutiset.rss?osasto=lumilautailu"},
                    new Feed { Tag = new Tag { Name = "Moottoripyöräily"}, Url = "http://yle.fi/urheilu/rss/uutiset.rss?osasto=moottoripyoraily"},
                    new Feed { Tag = new Tag { Name = "Muu palloilu"}, Url = "http://yle.fi/urheilu/rss/uutiset.rss?osasto=muu_palloilu"},
                    new Feed { Tag = new Tag { Name = "Muu urheilu"}, Url = "http://yle.fi/urheilu/rss/uutiset.rss?osasto=muut"},
                    new Feed { Tag = new Tag { Name = "Mäkihyppy"}, Url = "http://yle.fi/urheilu/rss/uutiset.rss?osasto=makihyppy"},
                    new Feed { Tag = new Tag { Name = "Nyrkkeily"}, Url = "http://yle.fi/urheilu/rss/uutiset.rss?osasto=nyrkkeily"},
                    new Feed { Tag = new Tag { Name = "Paini"}, Url = "http://yle.fi/urheilu/rss/uutiset.rss?osasto=paini"},
                    new Feed { Tag = new Tag { Name = "Pesäpallo"}, Url = "http://yle.fi/urheilu/rss/uutiset.rss?osasto=pesapallo"},
                    new Feed { Tag = new Tag { Name = "Pikaluistelu"}, Url = "http://yle.fi/urheilu/rss/uutiset.rss?osasto=pikaluistelu"},
                    new Feed { Tag = new Tag { Name = "Purjehdus"}, Url = "http://yle.fi/urheilu/rss/uutiset.rss?osasto=purjehdus"},
                    new Feed { Tag = new Tag { Name = "Pyöräily"}, Url = "http://yle.fi/urheilu/rss/uutiset.rss?osasto=pyoraily"},
                    new Feed { Tag = new Tag { Name = "Ralli"}, Url = "http://yle.fi/urheilu/rss/uutiset.rss?osasto=ralli"},
                    new Feed { Tag = new Tag { Name = "Ratsastus"}, Url = "http://yle.fi/urheilu/rss/uutiset.rss?osasto=ratsastus"},
                    new Feed { Tag = new Tag { Name = "Ravit"}, Url = "http://yle.fi/urheilu/rss/uutiset.rss?osasto=ravit"},
                    new Feed { Tag = new Tag { Name = "Salibandy"}, Url = "http://yle.fi/urheilu/rss/uutiset.rss?osasto=salibandy"},
                    new Feed { Tag = new Tag { Name = "Suunnistus"}, Url = "http://yle.fi/urheilu/rss/uutiset.rss?osasto=suunnistus"},
                    new Feed { Tag = new Tag { Name = "Taitoluistelu"}, Url = "http://yle.fi/urheilu/rss/uutiset.rss?osasto=taitoluistelu"},
                    new Feed { Tag = new Tag { Name = "Tennis"}, Url = "http://yle.fi/urheilu/rss/uutiset.rss?osasto=tennis"},
                    new Feed { Tag = new Tag { Name = "Uinti"}, Url = "http://yle.fi/urheilu/rss/uutiset.rss?osasto=uinti"},
                    new Feed { Tag = new Tag { Name = "Yhdistetty"}, Url = "http://yle.fi/urheilu/rss/uutiset.rss?osasto=yhdistetty"},
                    new Feed { Tag = new Tag { Name = "Yleisurheilu"}, Url = "http://yle.fi/urheilu/rss/uutiset.rss?osasto=yleisurheilu"},
                    new Feed { Tag = new Tag { Name = "Urheilu blogit ja kolumnit"}, Url = "http://yle.fi/urheilu/rss/uutiset.rss?osasto=kolumnit"}
                };
            }
        }
    }
}
