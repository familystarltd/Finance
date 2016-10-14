using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Text;

namespace Finance.Web
{
    public static class HtmlExtension
    {
        public static HtmlString Image(this IHtmlHelper html, string alt, int width, int height, byte[] image)
        {
            var img = String.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(image));
            StringBuilder htmlBuilder = new StringBuilder();
            htmlBuilder.Append("<img alt='" + alt + "'");
            htmlBuilder.Append(" src='" + img + "'");
            htmlBuilder.Append(" width='" + width + "px'");
            htmlBuilder.Append(" height='" + height + "px'");
            htmlBuilder.Append(" />");
            return new HtmlString(htmlBuilder.ToString());
        }
        public static HtmlString Image(this IHtmlHelper html, string alt, byte[] image)
        {
            var img = String.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(image));
            StringBuilder htmlBuilder = new StringBuilder();
            htmlBuilder.Append("<img alt='" + alt + "'");
            htmlBuilder.Append(" src='" + img + "'");
            htmlBuilder.Append(" />");
            return new HtmlString(htmlBuilder.ToString());
        }
        public static string ConvertEnumToJson(this IHtmlHelper html, Type type)
        {
            System.Text.StringBuilder JSON = new System.Text.StringBuilder();
            JSON.Append("[");
            foreach (var val in Enum.GetValues(type))
            {
                var name = Enum.GetName(type, val);
                JSON.Append("{");
                JSON.Append("name:");
                JSON.Append('"');
                JSON.Append(name);
                JSON.Append('"');
                JSON.Append(",id:");
                JSON.Append(((int)val).ToString());
                JSON.Append("},");
            }
            JSON.Append("]");
            return JSON.ToString();

        }

    }
}