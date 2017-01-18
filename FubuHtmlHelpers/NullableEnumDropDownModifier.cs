using System;
using HtmlTags.Conventions;

namespace FubuHtmlHelpers
{
    public class NullableEnumDropDownModifier
        : EnumDropDownModifier
    {
        public override bool Matches(ElementRequest token)
        {
            var underlyingType = Nullable.GetUnderlyingType(token.Accessor.PropertyType);
            return (underlyingType != null) && underlyingType.IsEnum;
        }

        protected override Type GetEnumType(ElementRequest request)
        {
            return Nullable.GetUnderlyingType(request.Accessor.PropertyType);
        }
    }
}