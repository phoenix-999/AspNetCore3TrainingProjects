using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TagHelpersApp.TagHelpers
{
    [HtmlTargetElement(Attributes = "ancor")]
    public class DivAncorTagHelper : TagHelper
    {
        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "h2";
            output.Attributes.RemoveAll("ancor");
            return Task.CompletedTask;
        }
    }
}
