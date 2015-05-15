using System;
using Microsoft.Practices.Unity;
using AE.Mpg.Abstract;
using AE.Mpg.Dal;
using AE.Mpg.Entity;
using AE.Snipplets.Dal;
using AE.EF.Abstract;
using AE.Funny.Dal;
using AE.News.Dal;

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
        /// <param name="c">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer c)
        {
            // TODO: Register your types here
            c.RegisterType<Controllers.View.AccountController>(new InjectionConstructor()); // tell unity to use constructor without parameters
            // mpg
            c.RegisterType<IGenericRepository<Vehicle>, GenericRepository<Vehicle>>();
            c.RegisterType<IGenericRepository<Fill>, GenericRepository<Fill>>();
            c.RegisterType<IGenericRepository<Fuel>, GenericRepository<Fuel>>();
            // news
            c.RegisterType(typeof(Controllers.Api.NewsController), new InjectionConstructor(typeof(NewsRepository)));
            c.RegisterType(typeof(Controllers.View.NewsController), new InjectionConstructor(typeof(NewsRepository)));
            c.RegisterType(typeof(Controllers.View.NewsAdminController), new InjectionConstructor(typeof(NewsRepository)));
            // snipplet
            c.RegisterType(typeof(Controllers.Api.CSharpController), new InjectionConstructor(typeof(SnippletRepository)));
            c.RegisterType(typeof(Controllers.View.CSharpController), new InjectionConstructor(typeof(SnippletRepository)));
            // funny
            c.RegisterType(typeof(Controllers.Api.FunnyController), new InjectionConstructor(typeof(FunnyRepository)));
            c.RegisterType(typeof(Controllers.View.FunnyController), new InjectionConstructor(typeof(FunnyRepository)));
        }
    }
}

