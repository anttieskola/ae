using AE.News.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AE.News.Dal
{
    public class NewsContext : DbContext
    {
        public NewsContext()
            : base("DbConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // db schema
            modelBuilder.HasDefaultSchema("news");
            // Feed has tag related to it
            modelBuilder.Entity<Feed>().HasRequired(f => f.Tag).WithRequiredPrincipal(t => t.Feed);
            // Article always has collection of tags and each tag has collection of
            // articles it can navigate back, so easy to search.
            // "many to many"
            modelBuilder.Entity<Article>().HasMany(a => a.Tags).WithMany(t => t.Articles).Map(x => x.MapLeftKey("ArticleId").MapRightKey("TagId").ToTable("ArticleTag"));

            // todo
            base.OnModelCreating(modelBuilder);
        }

        // "tables"
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Feed> Feeds { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Maintenance> Maintenances { get; set; }
    }
}
