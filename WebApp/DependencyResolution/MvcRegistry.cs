using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Castle.Core.Configuration;
using System.Web.Mvc.Async;

namespace WebApp.DependencyResolution
{
    public class MvcRegistry : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container
                .AddFacility<MvcFacility>()
                .Register(
                    Component.For<HttpRequest>().UsingFactoryMethod(_ => HttpContext.Current.Request).LifestylePerWebRequest(),
                    Component.For<HttpContext>().UsingFactoryMethod(_ => HttpContext.Current).LifestylePerWebRequest(),
                    Component.For<HttpRequestBase>().ImplementedBy<HttpRequestWrapper>().LifestylePerWebRequest(),
                    Component.For<HttpContextBase>().ImplementedBy<HttpContextWrapper>().LifestylePerWebRequest(),
                    Component.For<RequestContext>().UsingFactoryMethod(x=> x.Resolve<HttpContextBase>().Request.RequestContext),
                    Component.For<IControllerFactory>().ImplementedBy<WindsorControllerFactory>().LifestyleTransient(),
                    Component.For<ITempDataProviderFactory>().ImplementedBy<WindsorTempDataProviderFactory>().LifestyleTransient(),
                    Component.For<ITempDataProvider>().ImplementedBy<SessionStateTempDataProvider>().LifestyleTransient(),
                    Component.For<IAsyncActionInvokerFactory>().ImplementedBy<WindsorAsyncActionInvokerFactory>().LifestyleTransient(),
                    Component.For<IAsyncActionInvoker>().ImplementedBy<AsyncControllerActionInvoker>().LifestyleTransient(),
                    Component.For<IViewPageActivator>().ImplementedBy<WindsorViewPageActivator>().LifestyleTransient(),
                    Component.For<ModelMetadataProvider>().ImplementedBy<DataAnnotationsModelMetadataProvider>().LifestyleTransient(),
                    Classes
                        .FromThisAssembly()
                        .BasedOn<Controller>()
                        .LifestyleTransient()
                        .WithService.Self()
                        .Configure(x => x.Named(x.Implementation.Name.Replace("Controller", ""))));
        }
    }

    public class MvcFacility : IFacility
    {
        public void Init(IKernel kernel, IConfiguration facilityConfig)
        {
            System.Web.Mvc.DependencyResolver.SetResolver(new WindsorDependencyResolver(kernel));
        }

        public void Terminate()
        {
        }
    }
}