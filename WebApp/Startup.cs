using System.Web.Http;
using System.Web.Mvc;
using Microsoft.Owin;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Owin;
using StructureMap;
using WebActivatorEx;
using WebApp;
using WebApp.DependencyResolution;

[assembly: OwinStartup(typeof(Startup))]
[assembly: PreApplicationStartMethod(typeof(Startup), "Start")]
[assembly: ApplicationShutdownMethod(typeof(Startup), "End")]

namespace WebApp
{
    public partial class Startup
    {
        public static StructureMapDependencyScope StructureMapDependencyScope { get; set; }

        public void Configuration(IAppBuilder app)
        {
            //var config = new HttpConfiguration();
            //app.UseWebApi(config);

            var container = new Container(c => c.Scan(scan =>
            {
                scan.TheCallingAssembly();
                scan.WithDefaultConventions();
                scan.LookForRegistries();
            }));

            StructureMapDependencyScope = new StructureMapDependencyScope(container);
            DependencyResolver.SetResolver(StructureMapDependencyScope);
        }

        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(StructureMapScopeModule));
        }

        public static void End()
        {
            StructureMapDependencyScope?.Dispose();
        }
    }
}