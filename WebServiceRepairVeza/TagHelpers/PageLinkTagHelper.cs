using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using WebService.Models.ViewModels;

namespace WebService.TagHelpers
{
    public class PageLinkTagHelper : TagHelper
    {
        private readonly IUrlHelperFactory urlHelperFactory;
        public PageLinkTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext? ViewContext { get; set; }
        public PageViewModel? PageModel { get; set; }
        public string? PageAction { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (ViewContext != null && PageModel != null)
            {
                IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
                output.TagName = "div";

                TagBuilder tag = new("ul");
                tag.AddCssClass("pagination");

                TagBuilder currentItem = CreateTag(PageModel.PageNumber, urlHelper);

                if (PageModel.HasPreviousPage)
                {
                    for (int i = 1; i < PageModel.PageNumber; i++)
                    {
                        TagBuilder prevItem = CreateTag(i, urlHelper);
                        tag.InnerHtml.AppendHtml(prevItem);
                    }
                }

                tag.InnerHtml.AppendHtml(currentItem);

                if (PageModel.HasNextPage)
                {
                    for (int i = PageModel.PageNumber + 1; i < PageModel.TotalPages + 1; i++)
                    {
                        TagBuilder prevItem = CreateTag(i, urlHelper);
                        tag.InnerHtml.AppendHtml(prevItem);
                    }
                }
                output.Content.AppendHtml(tag);
            }
        }

        TagBuilder CreateTag(int pageNumber, IUrlHelper urlHelper)
        {
            TagBuilder item = new("li");
            TagBuilder link = new("a");
            if (PageModel != null && pageNumber == PageModel.PageNumber)
            {
                item.AddCssClass("active");
            }
            else if (ViewContext != null)
            {
                link.Attributes["href"] = urlHelper.Action(PageAction, new { page = ViewContext.ViewData["pageName"], pageNumber });
            }
            item.AddCssClass("page-item");
            link.AddCssClass("page-link");
            link.InnerHtml.Append(pageNumber.ToString());
            item.InnerHtml.AppendHtml(link);
            return item;
        }
    }
}
