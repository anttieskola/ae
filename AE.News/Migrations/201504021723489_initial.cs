namespace AE.News.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "news.Articles",
                c => new
                    {
                        ArticleId = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        Content = c.String(),
                        ImageUrl = c.String(),
                        SourceUrl = c.String(),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ArticleId);
            
            CreateTable(
                "news.Tags",
                c => new
                    {
                        TagId = c.Int(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.TagId)
                .ForeignKey("news.Feeds", t => t.TagId)
                .Index(t => t.TagId);
            
            CreateTable(
                "news.Feeds",
                c => new
                    {
                        FeedId = c.Int(nullable: false, identity: true),
                        Url = c.String(),
                    })
                .PrimaryKey(t => t.FeedId);
            
            CreateTable(
                "news.Maintenances",
                c => new
                    {
                        MaintenanceId = c.Int(nullable: false, identity: true),
                        Success = c.Boolean(nullable: false),
                        Exception = c.String(),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        Inserted = c.Int(nullable: false),
                        Deleted = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MaintenanceId);
            
            CreateTable(
                "news.ArticleTag",
                c => new
                    {
                        ArticleId = c.Int(nullable: false),
                        TagId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ArticleId, t.TagId })
                .ForeignKey("news.Articles", t => t.ArticleId, cascadeDelete: true)
                .ForeignKey("news.Tags", t => t.TagId, cascadeDelete: true)
                .Index(t => t.ArticleId)
                .Index(t => t.TagId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("news.ArticleTag", "TagId", "news.Tags");
            DropForeignKey("news.ArticleTag", "ArticleId", "news.Articles");
            DropForeignKey("news.Tags", "TagId", "news.Feeds");
            DropIndex("news.ArticleTag", new[] { "TagId" });
            DropIndex("news.ArticleTag", new[] { "ArticleId" });
            DropIndex("news.Tags", new[] { "TagId" });
            DropTable("news.ArticleTag");
            DropTable("news.Maintenances");
            DropTable("news.Feeds");
            DropTable("news.Tags");
            DropTable("news.Articles");
        }
    }
}
