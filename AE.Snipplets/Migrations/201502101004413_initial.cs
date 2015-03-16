namespace AE.Snipplets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "csharp.Snipplets",
                c => new
                    {
                        SnippletId = c.Int(nullable: false, identity: true),
                        Headline = c.String(),
                        Content = c.String(),
                    })
                .PrimaryKey(t => t.SnippletId);
            
            CreateTable(
                "csharp.Tags",
                c => new
                    {
                        TagId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.TagId);
            
            CreateTable(
                "csharp.SnippletTag",
                c => new
                    {
                        SnippletId = c.Int(nullable: false),
                        TagId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SnippletId, t.TagId })
                .ForeignKey("csharp.Snipplets", t => t.SnippletId, cascadeDelete: true)
                .ForeignKey("csharp.Tags", t => t.TagId, cascadeDelete: true)
                .Index(t => t.SnippletId)
                .Index(t => t.TagId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("csharp.SnippletTag", "TagId", "csharp.Tags");
            DropForeignKey("csharp.SnippletTag", "SnippletId", "csharp.Snipplets");
            DropIndex("csharp.SnippletTag", new[] { "TagId" });
            DropIndex("csharp.SnippletTag", new[] { "SnippletId" });
            DropTable("csharp.SnippletTag");
            DropTable("csharp.Tags");
            DropTable("csharp.Snipplets");
        }
    }
}
