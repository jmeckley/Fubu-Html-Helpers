using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FluentValidation.Mvc;
using FubuHtmlHelpers.StructureMap;
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
                scan.AssemblyContainingType<FubuHtmlTagsRegistry>();
                scan.WithDefaultConventions();
                scan.LookForRegistries();
            }));

            StructureMapDependencyScope = new StructureMapDependencyScope(container);
            DependencyResolver.SetResolver(StructureMapDependencyScope);

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            FluentValidationModelValidatorProvider.Configure();
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