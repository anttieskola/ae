using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AE.Mpg.Abstract;
using AE.Mpg.Entity;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using AE.WebUI.Controllers;
using System.Web.Mvc;
using AE.Users.Entity;

namespace AE.Test
{
    [TestClass]
    public class WebUI_MpgAdmin
    {
        //[TestMethod]
        //public void Vehicles()
        //{
        //    MpgAdminController controller = new MpgAdminController(mockVehicles.Object, mockFills.Object);
        //    ViewResult res = controller.Vehicles();
        //    // check model
        //    Assert.IsNotNull(res.Model);
        //    // check vehicles
        //    List<Vehicle> list = new List<Vehicle>(res.Model as IEnumerable<Vehicle>);
        //    Assert.AreEqual(vehicles.Count, list.Count);
        //    Assert.AreEqual(vehicle1.Model, list.First().Model);
        //}

        //[TestMethod]
        //public void Fills()
        //{
        //    MpgAdminController controller = new MpgAdminController(mockVehicles.Object, mockFills.Object);
        //    ViewResult res = controller.Fills(null);
        //    // check model
        //    Assert.IsNotNull(res.Model);
        //    // check vehicles
        //    List<Fill> list = new List<Fill>(res.Model as IEnumerable<Fill>);
        //    Assert.AreEqual(fills.Count, list.Count);
        //    Assert.AreEqual(fill1.Amount, list.First().Amount);

        //    controller.Fills(vehicle1.VehicleId);
        //    list = new List<Fill>(res.Model as IEnumerable<Fill>);
        //    Assert.AreEqual(fills.Where(f => f.VehicleId == vehicle1.VehicleId).Count(), list.Count);
        //}

        #region mocks
        private Fuel fuel
        {
            get
            {
                return new Fuel
                {
                    FuelId = 9001,
                    Name = "Electricity"
                };
            }
        }
        private Vehicle vehicle1
        {
            get
            {
                return new Vehicle
                {
                    VehicleId = 1111,
                    Make = "Tesla",
                    Model = "X",
                    ManufacturingYear = 2015,
                    Engine = "DurHur",
                    Fuel = fuel,
                    Private = true
                };
            }
        }
        private Vehicle vehicle2
        {
            get
            {
                return new Vehicle
                {
                    VehicleId = 1111,
                    Make = "Tesla",
                    Model = "S",
                    ManufacturingYear = 2014,
                    Engine = "HurDur",
                    Fuel = fuel,
                    Private = false
                };
            }
        }

        private List<Vehicle> vehicles
        {
            get
            {
                return new List<Vehicle>
                {
                    vehicle1,
                    vehicle2
                };
            }
        }
        private Fill fill1
        {
            get
            {
                return new Fill
                {
                    VehicleId = vehicle1.VehicleId,
                    Vehicle = vehicle1,
                    FillId = 292924,
                    Mileage = 1337,
                };
            }
        }
        private Fill fill2
        {
            get
            {
                return new Fill
                {
                    VehicleId = vehicle1.VehicleId,
                    Vehicle = vehicle1,
                    FillId = 292925,
                    Mileage = 3337,
                };
            }
        }
        private List<Fill> fills
        {
            get
            {
                return new List<Fill> {
                    fill1,
                    fill2
                };
            }
        }
        private Mock<IGenericRepository<Vehicle>> mockVehicles
        {
            get
            {
                Mock<IGenericRepository<Vehicle>> m = new Mock<IGenericRepository<Vehicle>>();
                m.Setup(r => r.Get(
                    It.IsAny<Expression<Func<Vehicle, bool>>>(),
                    It.IsAny<Func<IQueryable<Vehicle>, IOrderedQueryable<Vehicle>>>(),
                    It.IsAny<string>()
                    )).Returns(vehicles);
                return m;
            }
        }
        private Mock<IGenericRepository<Fill>> mockFills
        {
            get
            {
                Mock<IGenericRepository<Fill>> m = new Mock<IGenericRepository<Fill>>();
                m.Setup(r => r.Get(
                    It.IsAny<Expression<Func<Fill, bool>>>(),
                    It.IsAny<Func<IQueryable<Fill>, IOrderedQueryable<Fill>>>(),
                    It.IsAny<string>()
                    )).Returns(fills);
                return m;
            }
        }
        #endregion
    }
}
