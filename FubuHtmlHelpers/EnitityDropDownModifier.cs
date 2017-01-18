using System;
using System.Data.Entity;
using System.Linq;
using HtmlTags;
using HtmlTags.Conventions;
using HtmlTags.Conventions.Elements;

namespace FubuHtmlHelpers
{
    public class EnitityDropDownModifier : IElementModifier
    {
        public bool Matches(ElementRequest token)
        {
            return typeof(Entity).IsAssignableFrom(token.Accessor.PropertyType);
        }

        public void Modify(ElementRequest request)
        {
            request.CurrentTag.RemoveAttr("type");
            request.CurrentTag.TagName("select");
            request.CurrentTag.Append(new HtmlTag("option"));

            var context = request.Get<DbContext>();
            var entities = context.Set(request.Accessor.PropertyType).Cast<Entity>().ToList();
            var value = request.Value<Entity>();

            foreach (var entity in entities)
            {
                var optionTag = new HtmlTag("option").Value(entity.Id.ToString()).Text(entity.DisplayValue);

                if (value?.Id == entity.Id)
                {
                    optionTag.Attr("selected");
                }

                request.CurrentTag.Append(optionTag);
            }
        }
    }

    public abstract class Entity
    {
        public Guid Id { get; set; }
        public abstract string DisplayValue { get; }
    }
}
