using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using HtmlTags;
using HtmlTags.Conventions;
using HtmlTags.Conventions.Elements;
using HtmlTags.Reflection;
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
            return helper.Link(action, linkText).AddClass("btn").Attr("role", "button");
        }

        public static HtmlTag Input<T>(this HtmlHelper<T> helper, Expression<Func<T, object>> expression)
            where T : class
        {
            var generator = GetGenerator<T>();
            var clienId = helper.ClientIdFor(expression);

            return generator.InputFor(expression, model: helper.ViewData.Model).Id(clienId);
        }

        public static HtmlTag Display<T>(this HtmlHelper<T> helper, Expression<Func<T, object>> expression)
            where T : class
        {
            var generator = GetGenerator<T>();

            return generator.DisplayFor(expression, model: helper.ViewData.Model);
        }

        public static HtmlTag Label<T>(this HtmlHelper<T> helper, Expression<Func<T, object>> expression)
            where T : class
        {
            var generator = GetGenerator<T>();
            var clientId = helper.ClientIdFor(expression);

            return generator.LabelFor(expression, model: helper.ViewData.Model).Attr("for", clientId);
        }

        public static async Task<HtmlTag> QueryDropDown<T, TItem, TQuery>(this HtmlHelper<T> htmlHelper, Expression<Func<T, TItem>> expression, TQuery query, Func<TItem, string> displaySelector, Func<TItem, object> valueSelector)
            where TQuery : IRequest<IEnumerable<TItem>>
        {
            var expressionText = ExpressionHelper.GetExpressionText(expression);
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var selectedItem = (TItem)metadata.Model;

            var mediator = Get<IMediator>();
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

        public static void ConvertToDropDown<TItem, TQuery>(this HtmlTag tag, TItem value, IEnumerable<TQuery> items, Func<TQuery, string> displaySelector = null, Func<TQuery, string> valueSelector = null)
        {
            displaySelector = displaySelector ?? (_ => _.ToString());
            valueSelector = valueSelector ?? (_ => _.ToString());

            tag.RemoveAttr("type").TagName("select").Append(new HtmlTag("option"));
            foreach (var item in items)
            {
                var option = new HtmlTag("option").Text(displaySelector(item)).Value(valueSelector(item));
                if (valueSelector(item).Equals(value))
                {
                    option.Attr("selected");
                }
                tag.Append(option);
            }
        }

        public static HtmlTag Validator<T>(this HtmlHelper<T> helper, Expression<Func<T, object>> expression) where T : class
        {
            var error = helper.GetErrors(expression);
            if (error.Any() == false) return new NoTag();

            var errorMessage = error.FirstOrDefault()?.ErrorMessage;
            if(string.IsNullOrEmpty(errorMessage)) return new NoTag();

            var request = new ElementRequest(expression.ToAccessor()) {Model = helper.ViewData.Model};

            return GetGenerator<T>().TagFor(request, "Validator").Text(errorMessage);
        }

        public static ModelErrorCollection GetErrors<T>(this HtmlHelper<T> helper, Expression<Func<T, object>> expression) where T : class
        {
            // MVC code don't ask me I just copied
            var expressionText = GetExpressionText(expression);
            var fullHtmlFieldName = helper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(expressionText);
            var modelStateDictionary = helper.ViewData.ModelState;
            if (modelStateDictionary.ContainsKey(fullHtmlFieldName) == false) return new ModelErrorCollection();

            return  modelStateDictionary[fullHtmlFieldName]?.Errors ?? new ModelErrorCollection();
        }

        public static string ClientIdFor<T>(this HtmlHelper<T> helper, Expression<Func<T, object>> expression)
            where T : class
        {
            return GetExpressionText(expression).Replace('.', '_').Replace('[', '_').Replace(']', '_');
        }

        public static HtmlTag Submit(this HtmlHelper helper, string text = "Submit")
        {
            return new HtmlTag("input").Attr("type", "submit").Attr("value", text).AddClasses("btn", "btn-primary");
        }

        public static HtmlTag Reset<TController>(this HtmlHelper helper, Expression<Action<TController>> action, string text = "Reset")
            where TController : Controller
        {
            return helper.ButtonLink(action, text).AddClasses("btn-default");
        }

        public static MvcHtmlString ValidationErrorsSummary(this HtmlHelper helper, string message = "")
        {
            return helper.ValidationSummary(message, new { @class = "text-danger" });
        }

        private static T Get<T>()
        {
            return DependencyResolver.Current.GetService<T>();
        }

        private static IElementGenerator<T> GetGenerator<T>() where T : class
        {
            return Get<HtmlConventionLibrary>().GeneratorFor<T>();
        }

        private static string GetExpressionText<T>(Expression<Func<T, object>> expression)
            where T : class
        {
            if (expression.Body.NodeType == ExpressionType.Convert || expression.Body.NodeType == ExpressionType.ConvertChecked)
            {
                var lambda = Expression.Lambda(((UnaryExpression)expression.Body).Operand, expression.Parameters);
                return ExpressionHelper.GetExpressionText(lambda);
            }
            return ExpressionHelper.GetExpressionText(expression);
        }
    }
}
