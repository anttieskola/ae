using AE.Mpg.Entity;
using AE.Mpg.Dal;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Collections.Generic;

namespace AE.Mpg.Migrations
{
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

    internal sealed class Configuration : DbMigrationsConfiguration<MpgContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(MpgContext db)
        {
            // fuel types
            Fuel petrol = new Fuel { Name = "Petrol" };
            if (db.Fuels.Count() == 0)
            {
                db.Fuels.AddRange(new List<Fuel>
                    {
                        petrol,
                        new Fuel { Name = "Diesel"},
                        new Fuel { Name = "Autogas"},
                        new Fuel { Name = "Electricity"}
                    });
            }

            // my car
            Vehicle myCar = new Vehicle
            {
                Make = "Ford",
                ManufacturingYear = 2014,
                Model = "Fiesta",
                Engine = "1.0 3Cyl Ecoboost 100HP",
                Fuel = petrol,
            };

            if (db.Cars.Count() == 0)
            {
                db.Cars.Add(myCar);
            }

            // my fills
            List<Fill> fills = new List<Fill>
            {
                new Fill
                {
                    Vehicle = myCar,
                    VehicleId = myCar.VehicleId,
                    Mileage = 1000,
                    Amount = 40,
                    Price = 55.04F
                },
                new Fill
                {
                    Vehicle = myCar,
                    VehicleId = myCar.VehicleId,
                    Mileage = 2000,
                    Amount = 40,
                    Price = null
                },
                new Fill
                {
                    Vehicle = myCar,
                    VehicleId = myCar.VehicleId,
                    Mileage = 4000,
                    Amount = 40,
                    Price = 53.03F
                },
                new Fill
                {
                    Vehicle = myCar,
                    VehicleId = myCar.VehicleId,
                    Mileage = 7000,
                    Amount = 40,
                    Price = 49.92F
                }
            };
            if (db.Fills.Count() == 0)
            {
                db.Fills.AddRange(fills);
            }

            db.SaveChanges();
        }
    }
}
