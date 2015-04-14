namespace AE.Funny.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addedexception : DbMigration
    {
        public override void Up()
        {
            AddColumn("funny.Maintenances", "Exception", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("funny.Maintenances", "Exception");
        }
    }
}
