using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using FubuCore.Reflection;
using FubuMVC.Core.UI.Elements;
using HtmlTags;
using HtmlTags.Conventions;
using MediatR;
using Microsoft.Web.Mvc;

namespace FubuHtmlHelpers
{
    public static class FubuAspNetTagExtensions
    {
        public static HtmlTag Link<TController>(this HtmlHelper helper, Expression<Action<TController>> action, string linkText)
            where TController : Controller
        {
            var url = LinkBuilder.BuildUrlFromExpression(helper.ViewContext.RequestContext, RouteTable.Routes, action);
            return new HtmlTag("a").Text(linkText).Attr("href", url);
        }

        public static HtmlTag ButtonLink<TController>(this HtmlHelper helper, Expression<Action<TController>> action, string linkText)
            where TController : Controller
        {
            return helper.Link(action, linkText).AddClass("btn");
        }

        public static HtmlTag Input<T>(this HtmlHelper<T> helper,
            Expression<Func<T, object>> expression)
            where T : class
        {
            var generator = GetGenerator<T>();

            return generator.InputFor(expression, model: helper.ViewData.Model);
        }

        public static HtmlTag Display<T>(this HtmlHelper<T> helper,
            Expression<Func<T, object>> expression)
            where T : class
        {
            var generator = GetGenerator<T>();

            return generator.DisplayFor(expression, model: helper.ViewData.Model);
        }

        public static HtmlTag Label<T>(this HtmlHelper<T> helper,
            Expression<Func<T, object>> expression)
            where T : class
        {
            var generator = GetGenerator<T>();

            return generator.LabelFor(expression, model: helper.ViewData.Model);
        }

        public static async Task<HtmlTag> QueryDropDown<T, TItem, TQuery>(this HtmlHelper<T> htmlHelper, Expression<Func<T, TItem>> expression, TQuery query, Func<TItem, string> displaySelector, Func<TItem, object> valueSelector)
            where TQuery : IRequest<IEnumerable<TItem>>
        {
            var expressionText = ExpressionHelper.GetExpressionText(expression);
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var selectedItem = (TItem)metadata.Model;

            var mediator = DependencyResolver.Current.GetService<IMediator>();
            var items = await mediator.Send(query);
            var select = new SelectTag(t =>
            {
                t.Option("", string.Empty);
                foreach (var item in items)
                {
                    var htmlTag = t.Option(displaySelector(item), valueSelector(item));
                    if (item.Equals(selectedItem) == false) continue;
                    
                    htmlTag.Attr("selected");
                }

                t.Id(expressionText).Attr("name", expressionText);
            });

            return select;
        }

        public static HtmlTag Validator<T>(this HtmlHelper<T> helper, Expression<Func<T, object>> expression) where T : class
        {
            // MVC code don't ask me I just copied
            var expressionText = ExpressionHelper.GetExpressionText(expression);
            string fullHtmlFieldName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionText);

            if (!helper.ViewData.ModelState.ContainsKey(fullHtmlFieldName))
            {
                return new NoTag();
            }

            var  modelState = helper.ViewData.ModelState[fullHtmlFieldName];
            var  modelErrorCollection = modelState == null ? null : modelState.Errors;
            var  error = modelErrorCollection == null || modelErrorCollection.Any() == false ? null : modelErrorCollection.FirstOrDefault(m => string.IsNullOrEmpty(m.ErrorMessage) == false) ?? modelErrorCollection[0];
            if (error == null)
            {
                return new NoTag();
            }
            // End of MVC code

            var tagGeneratorFactory = DependencyResolver.Current.GetService<ITagGeneratorFactory>();
            var tagGenerator = tagGeneratorFactory.GeneratorFor<ElementRequest>();
            var request = new ElementRequest(expression.ToAccessor())
            {
                Model = helper.ViewData.Model
            };

            var tag = tagGenerator.Build(request, "Validator");

            tag.Text(error.ErrorMessage);

            return tag;
        }

        public static HtmlTag Submit(this HtmlHelper helper, string text = "Submit")
        {
            return new HtmlTag("input").Attr("type", "submit").Attr("value", text).AddClasses("btn", "btn-primary");
        }

        public static MvcHtmlString ValidationErrorsSummary(this HtmlHelper helper, string message = "")
        {
            return helper.ValidationSummary(message, new { @class = "text-danger" });
        }

        private static IElementGenerator<T> GetGenerator<T>() where T : class
        {
            var generator = DependencyResolver.Current.GetService<IElementGenerator<T>>();
            return generator;
        }
    }
}
