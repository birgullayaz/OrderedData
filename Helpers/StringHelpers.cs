using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace OrderedData.Helpers
{
    public static class StringHelpers
    {
        public static IHtmlContent ToUpperCase(this IHtmlHelper helper, string text)
        {
            return new HtmlString(text?.ToUpper() ?? string.Empty);
        }
    }
} 