using FubuCore;
using FubuCore.Binding.InMemory;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.UI.Security;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using HtmlTags.Conventions;
using FubuMVC.Core.Http.AspNet;
using System.Collections.Generic;
using FubuCore.Logging;

namespace WebApp.DependencyResolution
{
    public class FubuRegistry : IWindsorInstaller
    {
        public void Install(Castle.Windsor.IWindsorContainer container, IConfigurationStore store)
        {
            
            container.Register(Classes
                                .FromAssemblyContaining<IFubuRequest>()
                                .Pick()
                                .WithServiceDefaultInterfaces()
                                .Configure(c => c.LifestyleTransient()),
                               Classes
                                .FromAssemblyContaining<ITypeResolver>()
                                .Pick()
                                .WithServiceDefaultInterfaces()
                                .Configure(c => c.LifestyleTransient()),
                               Classes
                                .FromAssemblyContaining<ITagGeneratorFactory>()
                                .Pick()
                                .WithServiceDefaultInterfaces()
                                .Unless(t => t == typeof(HtmlConventionLibrary))
                                .Configure(c => c.LifestyleTransient()),
                               Classes
                                .FromAssemblyContaining<IFieldAccessService>()
                                .Pick()
                                .WithServiceDefaultInterfaces()
                                .Configure(c => c.LifestyleTransient()),
                               Component.For<IServiceLocator>().ImplementedBy<WindsorServiceLocator>().LifestyleTransient(),
                               Component.For<AspNetCurrentHttpRequest>().ImplementedBy<AspNetCurrentHttpRequest>().LifestyleTransient().Named("AspNetCurrentHttpRequest"),
                               Component.For<IEnumerable<ILogListener>>().Instance(new ILogListener[0]),
                               Component.For<IEnumerable<ILogModifier>>().Instance(new ILogModifier[0])
            );
        }
    }
}