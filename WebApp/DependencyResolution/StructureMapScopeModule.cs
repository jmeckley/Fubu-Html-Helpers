using System.Web;

namespace WebApp.DependencyResolution
{
    public class StructureMapScopeModule
        : IHttpModule
    {
        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += (sender, e) => Startup.StructureMapDependencyScope.CreateNestedContainer();
            context.EndRequest += (sender, e) =>
            {
                //HttpContextLifecycle.DisposeAndClearAll();
                Startup.StructureMapDependencyScope.DisposeNestedContainer();
            };
        }
    }
}