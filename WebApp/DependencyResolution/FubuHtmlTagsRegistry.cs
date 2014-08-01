using System.Web;
using FubuCore;
using FubuCore.Binding.Values;
using FubuMVC.Core.Http.AspNet;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Elements;
using StructureMap.Configuration.DSL;
using FubuHtmlHelpers;
using HtmlTags.Conventions;

namespace WebApp.DependencyResolution
{
    public class FubuHtmlTagsRegistry : Registry
    {
        public FubuHtmlTagsRegistry()
        {
            var htmlConventionLibrary = new HtmlConventionLibrary();
            htmlConventionLibrary.Import(new DefaultHtmlConventions().Library);
            var conventions = new OverrideHtmlConventions();

            htmlConventionLibrary.Import(conventions.Library);
            For<HtmlConventionLibrary>().Use(htmlConventionLibrary);

            For<IValueSource>().AddInstances(c =>
            {
                c.Type<RequestPropertyValueSource>();
            });
            For<ITagRequestActivator>().AddInstances(c =>
            {
                c.Type<ElementRequestActivator>();
                c.Type<ServiceLocatorTagRequestActivator>();
            });
            For<HttpRequestBase>().Use(c => c.GetInstance<HttpRequestWrapper>());
            For<HttpContextBase>().Use(c => c.GetInstance<HttpContextWrapper>());

            For<HttpRequest>().Use(() => HttpContext.Current.Request);
            For<HttpContext>().Use(() => HttpContext.Current);

            For<ITypeResolverStrategy>().Use<TypeResolver.DefaultStrategy>();
            For<IElementNamingConvention>().Use<DotNotationElementNamingConvention>();
            For(typeof(ITagGenerator<>)).Use(typeof(TagGenerator<>));
            For(typeof(IElementGenerator<>)).Use(typeof(ElementGenerator<>));
        }
    }
}