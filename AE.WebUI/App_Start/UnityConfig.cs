using System;
using Microsoft.Practices.Unity;
using AE.Mpg.Abstract;
using AE.Mpg.Dal;
using AE.Mpg.Entity;
using AE.WebUI.Controllers.View;
using AE.Snipplets.Dal;
using AE.EF.Abstract;

namespace AE.WebUI.App_Start
{
    /// <summary>
    /// Unity configuration
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // TODO: Register your types here
            container.RegisterType<AccountController>(new InjectionConstructor()); // tell unity to use constructor without parameters
            // mpg injection
            container.RegisterType<IGenericRepository<Vehicle>, GenericRepository<Vehicle>>();
            container.RegisterType<IGenericRepository<Fill>, GenericRepository<Fill>>();
            container.RegisterType<IGenericRepository<Fuel>, GenericRepository<Fuel>>();
            // snipplet injection
            container.RegisterType<IBasicRepository, SnippletRepository>();
        }
    }
}
