using System;
using FubuMVC.Core.UI;
using FubuMVC.Core.UI.Elements;
using HtmlTags;

namespace FubuHtmlHelpers
{
    public class EnumDropDownModifier 
        : IElementModifier
    {
        public virtual bool Matches(ElementRequest token)
        {
            return token.Accessor.PropertyType.IsEnum;
        }

        public void Modify(ElementRequest request)
        {
            var enumType = GetEnumType(request);

            request.CurrentTag.RemoveAttr("type");
            request.CurrentTag.TagName("select");
            request.CurrentTag.Append(new HtmlTag("option"));
            foreach (var value in Enum.GetValues(enumType))
            {
                var optionTag = new HtmlTag("option").Value(value.ToString()).Text(Enum.GetName(enumType, value));
                if (value.ToString() == request.RawValue?.ToString())
                {
                    optionTag.Attr("selected", true);
                }
                request.CurrentTag.Append(optionTag);
            }
        }

        protected virtual Type GetEnumType(ElementRequest request)
        {
            return request.Accessor.PropertyType;
        }
    }
}
