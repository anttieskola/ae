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
                        Private = c.Boolean(nullable: false),
                        AspNetUserId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.VehicleId)
                .ForeignKey("mpg.Fuels", t => t.FuelId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.AspNetUserId)
                .Index(t => t.FuelId)
                .Index(t => t.AspNetUserId);
            
            CreateTable(
                "mpg.Fuels",
                c => new
                    {
                        FuelId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.FuelId);
            
            //CreateTable(
            //    "dbo.AspNetUsers",
            //    c => new
            //        {
            //            Id = c.String(nullable: false, maxLength: 128),
            //            Email = c.String(),
            //            EmailConfirmed = c.Boolean(nullable: false),
            //            PasswordHash = c.String(),
            //            SecurityStamp = c.String(),
            //            PhoneNumber = c.String(),
            //            PhoneNumberConfirmed = c.Boolean(nullable: false),
            //            TwoFactorEnabled = c.Boolean(nullable: false),
            //            LockoutEndDateUtc = c.DateTime(),
            //            LockoutEnabled = c.Boolean(nullable: false),
            //            AccessFailedCount = c.Int(nullable: false),
            //            UserName = c.String(),
            //        })
            //    .PrimaryKey(t => t.Id);
            
            CreateTable(
                "mpg.Fills",
                c => new
                    {
                        FillId = c.Int(nullable: false, identity: true),
                        Mileage = c.Single(nullable: false),
                        Amount = c.Single(nullable: false),
                        VehicleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.FillId)
                .ForeignKey("mpg.Vehicles", t => t.VehicleId, cascadeDelete: true)
                .Index(t => t.VehicleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("mpg.Fills", "VehicleId", "mpg.Vehicles");
            DropForeignKey("mpg.Vehicles", "AspNetUserId", "dbo.AspNetUsers");
            DropForeignKey("mpg.Vehicles", "FuelId", "mpg.Fuels");
            DropIndex("mpg.Fills", new[] { "VehicleId" });
            DropIndex("mpg.Vehicles", new[] { "AspNetUserId" });
            DropIndex("mpg.Vehicles", new[] { "FuelId" });
            DropTable("mpg.Fills");
            //DropTable("dbo.AspNetUsers");
            DropTable("mpg.Fuels");
            DropTable("mpg.Vehicles");
        }
    }
}
