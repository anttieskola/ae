namespace AE.Mpg.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedUnitOfDistanceAndFillDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("mpg.Vehicles", "UnitOfDistance", c => c.Int(nullable: false));
            AddColumn("mpg.Fills", "Date", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("mpg.Fills", "Date");
            DropColumn("mpg.Vehicles", "UnitOfDistance");
        }
    }
}
