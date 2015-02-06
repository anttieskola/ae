using AE.Mpg.Entity;
using AE.Mpg.Dal;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Collections.Generic;
using System;

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
            var petrol = new Fuel { Name = "Petrol" };
            if (!db.Fuels.Any(f => f.Name == "Petrol"))
            {
                db.Fuels.AddRange(new List<Fuel>
                    {
                        petrol,
                        new Fuel { Name = "Diesel"},
                        new Fuel { Name = "Autogas"},
                        new Fuel { Name = "Electricity"}
                    });
            }
            else
            {
                petrol = db.Fuels.First(f => f.Name == "Petrol");
            }

            // my car
            var myCar = new Vehicle
            {
                Make = "Ford",
                ManufacturingYear = 2014,
                Model = "Fiesta",
                Engine = "1.0 Ecoboost 100HP",
                Fuel = petrol,
                UnitOfDistance = UnitOfDistance.Kilometers
            };

            if (!db.Cars.Any(c => c.Make == "Ford" && c.Model == "Fiesta" && c.Engine == "1.0 Ecoboost 100HP"))
            {
                db.Cars.Add(myCar);
            }
            else
            {
                myCar = db.Cars.First(c => c.Make == "Ford" && c.Model == "Fiesta" && c.Engine == "1.0 Ecoboost 100HP");
            }

            // my first ten fill ups
            List<Fill> fills = new List<Fill>
            {
                new Fill
                {
                    Vehicle = myCar,
                    VehicleId = myCar.VehicleId,
                    Mileage = 29,
                    Amount = 0,
                    Date = new DateTime(2014, 8, 1),
                    Price = 0F
                },
                new Fill
                {
                    Vehicle = myCar,
                    VehicleId = myCar.VehicleId,
                    Mileage = 515,
                    Amount = 29.3F,
                    Date = new DateTime(2014, 8, 8),
                    Price = 48.02F
                },
                new Fill
                {
                    Vehicle = myCar,
                    VehicleId = myCar.VehicleId,
                    Mileage = 870,
                    Amount = 21.5F,
                    Date = new DateTime(2014, 8, 15),
                    Price = 35.23F
                },
                new Fill
                {
                    Vehicle = myCar,
                    VehicleId = myCar.VehicleId,
                    Mileage = 1398,
                    Amount = 28.74F,
                    Date = new DateTime(2014, 8, 31),
                    Price = 44.69F
                },
                new Fill
                {
                    Vehicle = myCar,
                    VehicleId = myCar.VehicleId,
                    Mileage = 1987,
                    Amount = 33.09F,
                    Date = new DateTime(2014, 9, 15),
                    Price = 52.08F
                },
                new Fill
                {
                    Vehicle = myCar,
                    VehicleId = myCar.VehicleId,
                    Mileage = 2629,
                    Amount = 37.30F,
                    Date = new DateTime(2014, 9, 29),
                    Price = 57.22F
                },
                new Fill
                {
                    Vehicle = myCar,
                    VehicleId = myCar.VehicleId,
                    Mileage = 2899,
                    Amount = 15.99F,
                    Date = new DateTime(2014, 10, 10),
                    Price = 26.04F
                },
                new Fill
                {
                    Vehicle = myCar,
                    VehicleId = myCar.VehicleId,
                    Mileage = 3222,
                    Amount = 19.43F,
                    Date = new DateTime(2014, 11, 30),
                    Price = 27.10F
                }
            };
            if (!db.Fills.Any(f => f.VehicleId == myCar.VehicleId))
            {
                db.Fills.AddRange(fills);
            }

            // save
            db.SaveChanges();
        }
    }
}
