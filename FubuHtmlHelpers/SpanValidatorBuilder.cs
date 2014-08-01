using HtmlTags;
using FubuMVC.Core.UI.Elements;

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
