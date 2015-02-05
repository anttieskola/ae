namespace AE.Mpg.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "mpg.Vehicles",
                c => new
                    {
                        VehicleId = c.Int(nullable: false, identity: true),
                        Make = c.String(),
                        ManufacturingYear = c.Int(nullable: false),
                        Model = c.String(),
                        Engine = c.String(),
                        FuelId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.VehicleId)
                .ForeignKey("mpg.Fuels", t => t.FuelId, cascadeDelete: true)
                .Index(t => t.FuelId);
            
            CreateTable(
                "mpg.Fuels",
                c => new
                    {
                        FuelId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.FuelId);
            
            CreateTable(
                "mpg.Fills",
                c => new
                    {
                        FillId = c.Int(nullable: false, identity: true),
                        Mileage = c.Single(nullable: false),
                        Amount = c.Single(nullable: false),
                        Price = c.Single(),
                        VehicleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FillId)
                .ForeignKey("mpg.Vehicles", t => t.VehicleId, cascadeDelete: true)
                .Index(t => t.VehicleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("mpg.Fills", "VehicleId", "mpg.Vehicles");
            DropForeignKey("mpg.Vehicles", "FuelId", "mpg.Fuels");
            DropIndex("mpg.Fills", new[] { "VehicleId" });
            DropIndex("mpg.Vehicles", new[] { "FuelId" });
            DropTable("mpg.Fills");
            DropTable("mpg.Fuels");
            DropTable("mpg.Vehicles");
        }
    }
}
