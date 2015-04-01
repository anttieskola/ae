namespace AE.Snipplets.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "snipplet.Snipplets",
                c => new
                    {
                        SnippletId = c.Int(nullable: false, identity: true),
                        Headline = c.String(),
                        Content = c.String(),
                    })
                .PrimaryKey(t => t.SnippletId);
            
            CreateTable(
                "snipplet.Tags",
                c => new
                    {
                        TagId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.TagId);
            
            CreateTable(
                "snipplet.SnippletTag",
                c => new
                    {
                        SnippletId = c.Int(nullable: false),
                        TagId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.SnippletId, t.TagId })
                .ForeignKey("snipplet.Snipplets", t => t.SnippletId, cascadeDelete: true)
                .ForeignKey("snipplet.Tags", t => t.TagId, cascadeDelete: true)
                .Index(t => t.SnippletId)
                .Index(t => t.TagId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("snipplet.SnippletTag", "TagId", "snipplet.Tags");
            DropForeignKey("snipplet.SnippletTag", "SnippletId", "snipplet.Snipplets");
            DropIndex("snipplet.SnippletTag", new[] { "TagId" });
            DropIndex("snipplet.SnippletTag", new[] { "SnippletId" });
            DropTable("snipplet.SnippletTag");
            DropTable("snipplet.Tags");
            DropTable("snipplet.Snipplets");
        }
    }
}
