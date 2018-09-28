using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Sites.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Data.Models;
using Kooboo.Data.Extensions;
using Kooboo.Dom; 

namespace Kooboo.Sites.Service
{
   public class Suggestion
    {

        /// <summary>
        /// Send in a dom string and page information and return with a suggestion menu json... 
        /// </summary>
        /// <param name="website"></param>
        /// <param name="PageId"></param>
        /// <param name="domString"></param>
        /// <returns></returns>
        public static Menu SuggestMenu(WebSite website, Guid PageId, string domString)
        {
            var page = website.SiteDb().Pages.Get(PageId);
            if (page == null)
            {
                return null;
            }

            return SuggestMenu(website, page.Body, domString); 
        }

        public static Menu SuggestMenu(WebSite website, string PageHtmlSource, string SelectedDomString)
        {
            var dom = Kooboo.Dom.DomParser.CreateDom(PageHtmlSource);

            var alllinks = dom.Links.item;

            var PageLinkGroupby = Kooboo.Sites.Service.MenuService.GroupBy(alllinks);

            var menulinks = FindMenus(PageLinkGroupby, SelectedDomString);

            return Kooboo.Sites.Service.MenuService.ConvertToMenu(menulinks, website); 
        }

        public static List<Element> FindMenus(List<List<Element>> groupby, string domstring)
        {

            List<string> links = new List<string>();

            var doc = Kooboo.Dom.DomParser.CreateDom(domstring);

            foreach (var item in doc.Links.item)
            {
                string href = item.getAttribute("href");

                if (!string.IsNullOrEmpty(href))
                {
                    links.Add(href); 
                }
            }

            foreach (var items in groupby)
            {
                foreach (var item in items)
                {
                    string href = item.getAttribute("href");

                    if (links.Contains(href))
                    {
                        return items; 
                    }
                }                
            }

            return null; 
        }

       /// <summary>
       /// This is only for the first to replace the html source with menu information... 
       /// </summary>
       /// <param name="website"></param>
       /// <param name="PageId"></param>
       /// <param name="menu"></param>
        public static void ReplaceMenu(WebSite website, Guid PageId, Menu menu)
        {
            var page = website.SiteDb().Pages.Get(PageId);

            var dom = Kooboo.Dom.DomParser.CreateDom(page.Body);

            var elements = dom.getElementsByTagName("menu");

            if (elements != null)
            {
                foreach (var item in elements.item)
                {
                    if (!string.IsNullOrEmpty(item.id))
                    {
                        Guid menuid;
                        System.Guid.TryParse(item.id, out menuid); 
                        if (menuid == menu.Id)
                        { return;  }
                    }
                }
            }

            int startindex = menu.tempdata.StartIndex;
            int endindex = menu.tempdata.EndIndex;

            if (endindex > page.Body.Length)
            { return;  }

            var startelement = dom.getElementByIndex(startindex, true);
            var endelement = dom.getElementByIndex(endindex, false);

            string newhtml;
            newhtml = dom.HtmlSource.Substring(0, startindex);
            newhtml += "<menu id='"+menu.Id + "'></menu>";
            newhtml += dom.HtmlSource.Substring(endindex + 1);

            page.Body = newhtml;

            website.SiteDb().Pages.AddOrUpdate(page); 

        }

    }
}
