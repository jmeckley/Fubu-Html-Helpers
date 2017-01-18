using HtmlTags;
using HtmlTags.Conventions;
using HtmlTags.Conventions.Elements;

namespace FubuHtmlHelpers
{
    public class SpanValidatorBuilder : IElementBuilder
    {
        public HtmlTag Build(ElementRequest request)
        {
            return new HtmlTag("span")
                .AddClass("field-validation-error")
                .AddClass("text-danger")
                .Data("valmsg-for", request.ElementId);
        }
    }
}
