using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using HtmlTags;

namespace FubuHtmlHelpers
{
    public static class FubuAspNetBlockExtensions
    {
        public static HtmlTag InputBlock<T>(this HtmlHelper<T> helper, Expression<Func<T, object>> expression, Action<HtmlTag> inputModifier = null, Action<HtmlTag> validatorModifier = null)
            where T : class
        {
            inputModifier = inputModifier ?? (_ => { });
            validatorModifier = validatorModifier ?? (_ => { });

            var divTag = new HtmlTag("div");
            divTag.AddClass("col-md-10");

            var inputTag = helper.Input(expression);
            inputModifier(inputTag);

            var validatorTag = helper.Validator(expression);
            validatorModifier(validatorTag);

            divTag.Append(inputTag);
            divTag.Append(validatorTag);

            return divTag;
        }

        public static HtmlTag FormBlock<T>(this HtmlHelper<T> helper, Expression<Func<T, object>> expression, Action<HtmlTag> labelModifier = null, Action<HtmlTag> inputBlockModifier = null, Action<HtmlTag> inputModifier = null, Action<HtmlTag> validatorModifier = null)
            where T : class
        {
            labelModifier = labelModifier ?? (_ => { });
            inputBlockModifier = inputBlockModifier ?? (_ => { });

            var divTag = new HtmlTag("div");
            divTag.AddClass("form-group");
            if (helper.GetErrors(expression).Any())
            {
                divTag.AddClass("has-error");
            }

            var labelTag = helper.Label(expression);
            labelModifier(labelTag);

            var inputBlockTag = helper.InputBlock(expression, inputModifier, validatorModifier);
            inputBlockModifier(inputBlockTag);

            divTag.Append(labelTag);
            divTag.Append(inputBlockTag);

            return divTag;
        }

        public static HtmlTag SubmitBlock(this HtmlHelper helper, string text = "Submit")
        {
            var divTag = new HtmlTag("div").AddClass("form-group");
            var columnTag = new HtmlTag("div").AddClasses("col-md-offset-2", "col-md-10");

            columnTag.Append(helper.Submit(text));
            divTag.Append(columnTag);

            return divTag;
        }

        public static HtmlTag ActionBlock(this HtmlHelper helper, params HtmlTag[] tags)
        {
            var divTag = new HtmlTag("div").AddClass("form-group");
            var columnTag = new HtmlTag("div").AddClasses("col-md-offset-2", "col-md-10");

            Array.ForEach(tags, tag => columnTag.Append(tag));
            divTag.Append(columnTag);

            return divTag;
        }


        public static HtmlTag ValidationSummaryBlock(this HtmlHelper helper, string message = "")
        {
            var divTag = new HtmlTag("div").AddClass("form-group");
            var columnTag = new HtmlTag("div").AddClasses("col-md-offset-2", "col-md-10");
            
            columnTag.AppendHtml(helper.ValidationErrorsSummary(message).ToString());
            divTag.Append(columnTag);

            return divTag;
        }
    }
}
