using E_Ticaret.WebUI.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Ticaret.WebUI.TagHelpers
{
    [HtmlTargetElement("div",Attributes="page-model")]//div etiketi için kullanılacak dedim.Att. ise verilecek isim
    public class PageLinkTagHelper:TagHelper
    {
        public PageInfo PageModel { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<ul class='pagination'>");

            for (int i = 1; i <= PageModel.TotalPages(); i++)
            {
                stringBuilder.AppendFormat("<li class = 'page-item {0}'>", i == PageModel.CurrentPage ? "active" : "");//seçileni aktif yaptı
                if (string.IsNullOrEmpty(PageModel.CurrentCategory))//seçilen kategoriye göre link oluşturudm
                {
                    stringBuilder.AppendFormat("<a class='page-link' href='/products?page={0}'>{0}</a>",i);//{0} dedim bu query string. sayfa 1 olduğunda oda 1 olur
                }
                else
                {
                    stringBuilder.AppendFormat("<a class='page-link' href='/products/{1}?page={0}'>{0}</a>", i, PageModel.CurrentCategory);//burada {1} kategori 1 den başladı
                }
                stringBuilder.Append("</li>");
            }
            output.Content.SetHtmlContent(stringBuilder.ToString());//burada da set ettik
            base.Process(context, output);
        }
    }
}
