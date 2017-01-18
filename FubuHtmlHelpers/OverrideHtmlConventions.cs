using System;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Web.Mvc;
using HtmlTags.Conventions;
using HtmlTags.Reflection;
using NodaTime;

namespace FubuHtmlHelpers
{
    public class OverrideHtmlConventions : DefaultHtmlConventions
    {
        public OverrideHtmlConventions()
        {
            Always();
            Html5();
            
            Labels.IfPropertyIs<bool>().ModifyWith(er => er.CurrentTag.Text(er.OriginalTag.Text() + "?"));
        }

        private void Html5()
        {
            Editors.Modifier<EnumDropDownModifier>();
            Editors.Modifier<NullableEnumDropDownModifier>();
            Editors.Modifier<EnitityDropDownModifier>();
            Editors.IfPropertyIs<bool>().ModifyWith(m => m.CurrentTag.Attr("type", "checkbox").Attr("value", "true"));
            Editors.IfPropertyIs<Color>().Attr("type", "color");
            Editors.IfPropertyIs<LocalDate>().Attr("type", "date");
            Editors.IfPropertyIs<LocalTime>().Attr("type", "time");
            Editors.IfPropertyIs<LocalDateTime>().Attr("type", "datetime-local");
            Editors.IfPropertyIs<OffsetDateTime>().Attr("type", "datetime");
            Editors.If(er => er.Accessor.Name.Contains("Email")).Attr("type", "email");
            Editors.IfPropertyIs<Guid>().Attr("type", "hidden");
            Editors.IfPropertyIs<Guid?>().Attr("type", "hidden");
            Editors.IfPropertyHasAttribute<HiddenInputAttribute>().Attr("type", "hidden");
            Editors.IfPropertyIs<decimal?>().ModifyWith(m => m.CurrentTag.Data("pattern", "9{1,9}.99").Data("placeholder", "0.00").Attr("type", "number"));
            Editors.IfPropertyIs<decimal>().ModifyWith(m => m.CurrentTag.Data("pattern", "9{1,9}.99").Data("placeholder", "0.00").Attr("type", "number"));
            Editors.IfPropertyIs<int?>().ModifyWith(m => m.CurrentTag.Data("pattern", "9{1,9}").Data("placeholder", "0").Attr("type", "number"));
            Editors.IfPropertyIs<int>().ModifyWith(m => m.CurrentTag.Data("pattern", "9{1,9}").Data("placeholder", "0").Attr("type", "number"));

            Editors.If(er => er.Accessor.Name.Contains("Password")).Attr("type", "password");
            Editors.If(er =>
            {
                var attr = er.Accessor.GetAttribute<DataTypeAttribute>();
                return attr != null && attr.DataType == DataType.Password;
            }).Attr("type", "password");

            Editors.If(er => er.Accessor.Name.Contains("Phone")).Attr("type", "tel");
            Editors.If(er =>
            {
                var attr = er.Accessor.GetAttribute<DataTypeAttribute>();
                return attr != null && attr.DataType == DataType.PhoneNumber;
            }).Attr("type", "tel");

            Editors.If(er => er.Accessor.Name.Contains("Url")).Attr("type", "url");
            Editors.If(er =>
            {
                var attr = er.Accessor.GetAttribute<DataTypeAttribute>();
                return attr != null && attr.DataType == DataType.Url;
            }).Attr("type", "url");
        }

        private void Always()
        {
            Validators.Always.BuildBy<SpanValidatorBuilder>();
            Editors.Always.AddClass("form-control");
            Labels.Always.AddClass("control-label");
            Labels.Always.AddClass("col-md-2");
            Labels.ModifyForAttribute<DisplayAttribute>((t, a) => t.Text(a.Name));
        }

        protected ElementCategoryExpression Validators
        {
            get
            {
                var builderSet = Library.TagLibrary.Category("Validator").Defaults;
                return new ElementCategoryExpression(builderSet);
            }
        }
    }
}
