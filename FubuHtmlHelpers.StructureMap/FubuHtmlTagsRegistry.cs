using System.Linq;
using System.Web;
using FubuCore;
using FubuCore.Binding.Values;
using FubuMVC.Core.Http.AspNet;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Elements;
using HtmlTags.Conventions;
using StructureMap;

namespace FubuHtmlHelpers.StructureMap
{
    public class FubuHtmlTagsRegistry
        : Registry
    {
        public FubuHtmlTagsRegistry()
        {
            For<DefaultHtmlConventions>().Use(new DefaultHtmlConventions());
            For<DefaultHtmlConventions>().Use(new OverrideHtmlConventions());
            For<HtmlConventionLibrary>().Singleton().Use(ctx => BuildLibrary(ctx));
            
            For<IValueSource>().AddInstances(c => c.Type<RequestPropertyValueSource>());
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

        private HtmlConventionLibrary BuildLibrary(IContext context)
        {
            return context
                .GetAllInstances<DefaultHtmlConventions>()
                .Aggregate(new HtmlConventionLibrary(), (library, convention) =>
                {
                    library.Import(convention.Library);
                    return library;
                });
        }
    }
}