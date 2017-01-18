using System;
using HtmlTags.Conventions;
using HtmlTags.Reflection;

namespace FubuHtmlHelpers
{
    public static class ElementCategoryExpressionExtensions
    {
        public static ElementActionExpression HasAttributeValue<TAttribute>(this ElementCategoryExpression expression, Func<TAttribute, bool> matches)
            where TAttribute : Attribute
        {
            return expression.If(er =>
            {
                var attr = er.Accessor.GetAttribute<TAttribute>();
                return attr != null && matches(attr);
            });
        }
    }
}
