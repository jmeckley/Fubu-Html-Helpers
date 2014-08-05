using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Castle.Windsor;
using Castle.Windsor.Installer;
using WebApp.DependencyResolution;

namespace WebApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public MvcApplication()
        {
            this.EndRequest += ReleaseViews;
        }

        protected void Application_Start()
        {
            new WindsorContainer().Install(FromAssembly.This());

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        // there is probably a better way to do this.
        protected void ReleaseViews(object sender, System.EventArgs e)
        {
            var views = System.Web.HttpContext.Current.Items[WindsorViewPageActivator.key] as System.Collections.Generic.IEnumerable<object>;
            if (views == null) return;

            var kernel = DependencyResolver.Current.GetService<Castle.MicroKernel.IKernel>();
            foreach(var view in views)
            {
                kernel.ReleaseComponent(view);
            }
        }
    }
}
