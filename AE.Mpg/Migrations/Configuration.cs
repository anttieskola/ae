using AE.Mpg.Entity;
using AE.Mpg.Dal;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Collections.Generic;

namespace AE.Mpg.Migrations
{
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
                Engine = "100HP Ecoboost",
                Fuel = petrol,
                Private = false
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
                    Amount = 40
                },
                new Fill
                {
                    Vehicle = myCar,
                    VehicleId = myCar.VehicleId,
                    Mileage = 2000,
                    Amount = 40
                },
                new Fill
                {
                    Vehicle = myCar,
                    VehicleId = myCar.VehicleId,
                    Mileage = 4000,
                    Amount = 40
                },
                new Fill
                {
                    Vehicle = myCar,
                    VehicleId = myCar.VehicleId,
                    Mileage = 7000,
                    Amount = 40
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
