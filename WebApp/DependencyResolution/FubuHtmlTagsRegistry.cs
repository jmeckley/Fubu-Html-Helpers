using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using FubuCore;
using FubuCore.Binding.Values;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Elements;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using FubuHtmlHelpers;
using HtmlTags.Conventions;

namespace WebApp.DependencyResolution
{
    public class FubuHtmlTagsRegistry : IWindsorInstaller
    {
        public void Install(Castle.Windsor.IWindsorContainer container, IConfigurationStore store)
        {
            var htmlConventionLibrary = new HtmlConventionLibrary();
            htmlConventionLibrary.Import(new DefaultHtmlConventions().Library);
            var conventions = new OverrideHtmlConventions();

            htmlConventionLibrary.Import(conventions.Library);

            //container.Kernel.Resolver.AddSubResolver(new Castle.MicroKernel.Resolvers.SpecializedResolvers.CollectionResolver(container.Kernel, true));

            container
                .Register(
                Component.For<HtmlConventionLibrary>().Instance(htmlConventionLibrary),
                Component.For<ITagRequestActivator>().ImplementedBy<ElementRequestActivator>().Named("ElementRequestActivator"),
                Component.For<ITagRequestActivator>().ImplementedBy<ServiceLocatorTagRequestActivator>().Named("ServiceLocatorTagRequestActivator"),
                Component.For<IEnumerable<ITagRequestActivator>>().UsingFactoryMethod(x => x.ResolveAll<ITagRequestActivator>())
                /*
                Component.For<IValueSource>().ImplementedBy<RequestPropertyValueSource>().Named("RequestPropertyValueSource"),
                Component.For<ITypeResolverStrategy>().ImplementedBy<TypeResolver.DefaultStrategy>().Named("TypeResolver.DefaultStrategy"),
                Component.For<IElementNamingConvention>().ImplementedBy<DotNotationElementNamingConvention>().Named("DotNotationElementNamingConvention"),
                Component.For(typeof(ITagGenerator<>)).ImplementedBy(typeof(TagGenerator<>)).Named("TagGenerator"),
                Component.For(typeof(IElementGenerator<>)).ImplementedBy(typeof(ElementGenerator<>)).Named("ElementGenerator")
                */
            );
        }
    }
}