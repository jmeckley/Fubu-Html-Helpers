using System.Linq;
using System.Web;
using HtmlTags.Conventions;
using HtmlTags.Conventions.Elements;
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
            
            For<HttpRequestBase>().Use(c => c.GetInstance<HttpRequestWrapper>());
            For<HttpContextBase>().Use(c => c.GetInstance<HttpContextWrapper>());

            For<HttpRequest>().Use(() => HttpContext.Current.Request);
            For<HttpContext>().Use(() => HttpContext.Current);

            For<IElementNamingConvention>().Use<DotNotationElementNamingConvention>();
            For<ITagGenerator>().Use<TagGenerator>();
        }

        private HtmlConventionLibrary BuildLibrary(IContext context)
        {
            return context
                .GetAllInstances<DefaultHtmlConventions>()
                .Aggregate(new HtmlConventionLibrary(), (library, convention) =>
                {
                    convention.Apply(library);
                    return library;
                });
        }
    }
}