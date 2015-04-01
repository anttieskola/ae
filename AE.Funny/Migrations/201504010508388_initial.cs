namespace AE.Funny.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "funny.Maintenances",
                c => new
                    {
                        MaintenanceId = c.Int(nullable: false, identity: true),
                        Success = c.Boolean(nullable: false),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        Inserted = c.Int(nullable: false),
                        Deleted = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MaintenanceId);
            
            CreateTable(
                "funny.Posts",
                c => new
                    {
                        PostId = c.Int(nullable: false, identity: true),
                        Created = c.DateTime(nullable: false),
                        RedditId = c.String(),
                        Title = c.String(),
                        ImageUrl = c.String(),
                    })
                .PrimaryKey(t => t.PostId);
            
            CreateTable(
                "funny.Comments",
                c => new
                    {
                        CommentId = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Post_PostId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CommentId)
                .ForeignKey("funny.Posts", t => t.Post_PostId, cascadeDelete: true)
                .Index(t => t.Post_PostId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("funny.Comments", "Post_PostId", "funny.Posts");
            DropIndex("funny.Comments", new[] { "Post_PostId" });
            DropTable("funny.Comments");
            DropTable("funny.Posts");
            DropTable("funny.Maintenances");
        }
    }
}
