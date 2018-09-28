using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Sites.Models;
using System;
using System.Linq;

namespace Kooboo.Sites.Helper
{
    public class MenuHelper
    {
        public const string MarkAnchorText = "{anchortext}";

        public const string MarkHref = "{href}";
         
        public const string MarkSubItems = "{items}";
         
        public const string MarkActiveClassReplacer = "{activeclass}";

        public const string MarkParentId = "{parentid}";

        public const string MarkCurrentId = "{currentid}";

        public static string DefaultSubItemContainer = $"<ul class=\"menu\">{MenuHelper.MarkSubItems}</ul>";

        public static string DefaultSubItemTemplate = $"<li><a href=\"{MenuHelper.MarkHref}\">{MenuHelper.MarkAnchorText}</a>{MenuHelper.MarkSubItems}</li>";

        public static bool IsActive(Menu CurrentMenu, RenderContext context)
        {
            if (context == null || CurrentMenu == null)
            {
                return false;
            }

            var url = context.Request.RelativeUrl.ToLower();

            if (!string.IsNullOrEmpty(CurrentMenu.Url) && CurrentMenu.Url.ToLower() == url)
            {
                return true;
            }
            foreach (var item in CurrentMenu.children)
            {
                if (IsActive(item, context))
                {
                    return true;
                }
            }
            return false;
        }

        internal static string PraseSelfTemplate(Menu menu)
        {
            if (!string.IsNullOrEmpty(menu.Template))
            {
                return menu.Template;
            }
            string template = null;

            Menu currentParent = menu.Parent;
            while (currentParent != null)
            {
                if (!string.IsNullOrEmpty(currentParent.SubItemTemplate))
                {
                    template = currentParent.SubItemTemplate;
                    break;
                }
                else
                {
                    foreach (var item in currentParent.children)
                    {
                        if (!string.IsNullOrEmpty(item.Template))
                        {
                            template = item.Template;
                            break;
                        }

                    }
                }
                currentParent = currentParent.Parent;
            }

            if (string.IsNullOrEmpty(template))
            {
                template = MenuHelper.DefaultSubItemTemplate;
            }

            menu.Template = template;
            return menu.Template;
        }

        internal static string PraseSubItemContainer(Menu menu)
        {
            if (!string.IsNullOrEmpty(menu.SubItemContainer))
            {
                return menu.SubItemContainer;
            }

            string subitemcontainer = null;

            Menu currentParent = menu.Parent;
            while (currentParent != null)
            {
                if (!string.IsNullOrEmpty(currentParent.SubItemContainer))
                {
                    subitemcontainer = currentParent.SubItemContainer;
                    break;
                }
                else
                {
                    foreach (var item in currentParent.children)
                    {
                        if (!string.IsNullOrEmpty(item.SubItemContainer))
                        {
                            subitemcontainer = item.SubItemContainer;
                            break;
                        }

                    }
                }

                currentParent = currentParent.Parent;
            }

            menu.SubItemContainer = subitemcontainer;

            return subitemcontainer;
        }
         
        private static string getMenuUrl(Menu menu, RenderContext context)
        {
            if (menu == null)
            {
                return null;  
            }

            string url = menu.Url;
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }
            if (context != null && context.WebSite.EnableSitePath)
            {
                return Lib.Helper.UrlHelper.Combine("/" + context.Culture, url);
            }
            else
            {
                return url;
            }
        }

        /// <summary>
        /// Render current menu and output the string. 
        /// </summary>
        /// <returns></returns>
        public static string Render(Menu Menu, Render.FrontContext context = null)
        {
            string template = null;
            if (!string.IsNullOrEmpty(Menu.Url) && !string.IsNullOrEmpty(Menu.Name))
            {
                template = MenuHelper.PraseSelfTemplate(Menu);
                template = template.Replace(MenuHelper.MarkHref, Menu.Url);
                template = template.Replace(MenuHelper.MarkAnchorText, Menu.Name);
                if (Menu.RenderId)
                {
                    string parentid = null; if (Menu.Parent != null)
                    { parentid = Menu.Parent.Id.ToString(); }
                    if (!string.IsNullOrEmpty(parentid))
                    {
                        template = template.Replace(MenuHelper.MarkParentId, parentid);
                    }
                    template = template.Replace(MenuHelper.MarkCurrentId, Menu.Id.ToString());
                }
                if (Menu.HasActiveClass)
                {
                    string activeclassname = string.Empty;
                    if (MenuHelper.IsActive(Menu, context.RenderContext))
                    {
                        activeclassname = Menu.ActiveClass;
                    }
                    template = template.Replace(MenuHelper.MarkActiveClassReplacer, activeclassname);
                }

            }
            string submenustring = string.Empty;

            foreach (var item in Menu.children)
            {
                if (item.Parent == null)
                {
                    item.Parent = Menu;
                }
                string rendermenu = Render(item, context);
                submenustring += rendermenu;
            }
            string subitemcontainer = MenuHelper.PraseSubItemContainer(Menu);

            if (!string.IsNullOrEmpty(subitemcontainer) && subitemcontainer.Contains(MenuHelper.MarkSubItems) && !string.IsNullOrEmpty(submenustring))
            {
                submenustring = subitemcontainer.Replace(MenuHelper.MarkSubItems, submenustring);
            }

            if (!string.IsNullOrEmpty(template) && template.Contains(MenuHelper.MarkSubItems))
            {
                return template.Replace(MenuHelper.MarkSubItems, submenustring);
            }
            else
            {
                if (string.IsNullOrEmpty(template))
                {
                    return submenustring;
                }
                else
                {
                    return template;
                }
            }
        }

        public static string Render(Menu Menu, RenderContext context, int levels = 999)
        {
            string template = null;

            string menuname = string.Empty;
            if (context != null)
            {
                menuname = GetName(Menu, context.Culture);
            }
            else
            {
                if (!string.IsNullOrEmpty(Menu.Name))
                {
                    menuname = Menu.Name;
                }
                else
                {
                    if (Menu.Values != null && Menu.Values.Count() > 0)
                    {
                        menuname = Menu.Values.First().Value;
                    }
                }
            }
             
            if (!string.IsNullOrEmpty(Menu.Url) && !string.IsNullOrEmpty(menuname))
            {
                string url = getMenuUrl(Menu, context);

                template = MenuHelper.PraseSelfTemplate(Menu);
                template = template.Replace(MenuHelper.MarkHref, url);
                template = template.Replace(MenuHelper.MarkAnchorText, menuname);
                if (Menu.RenderId)
                {
                    string parentid = null; if (Menu.Parent != null)
                    { parentid = Menu.Parent.Id.ToString(); }
                    if (!string.IsNullOrEmpty(parentid))
                    {
                        template = template.Replace(MenuHelper.MarkParentId, parentid);
                    }
                    template = template.Replace(MenuHelper.MarkCurrentId, Menu.Id.ToString());
                }
                if (Menu.HasActiveClass)
                {
                    string activeclassname = string.Empty;
                    if (MenuHelper.IsActive(Menu, context))
                    {
                        activeclassname = Menu.ActiveClass;
                    }
                    template = template.Replace(MenuHelper.MarkActiveClassReplacer, activeclassname);
                }

            }
            string submenustring = string.Empty;

            if (levels > 0)
            {
                foreach (var item in Menu.children)
                {
                    if (item.Parent == null)
                    {
                        item.Parent = Menu;
                    }
                    string rendermenu = Render(item, context, levels - 1);
                    submenustring += rendermenu;
                }
            }

            string subitemcontainer = MenuHelper.PraseSubItemContainer(Menu);

            if (!string.IsNullOrEmpty(subitemcontainer) && subitemcontainer.Contains(MenuHelper.MarkSubItems) && !string.IsNullOrEmpty(submenustring))
            {
                submenustring = subitemcontainer.Replace(MenuHelper.MarkSubItems, submenustring);
            }

            if (!string.IsNullOrEmpty(template) && template.Contains(MenuHelper.MarkSubItems))
            {
                return template.Replace(MenuHelper.MarkSubItems, submenustring);
            }
            else
            {
                if (string.IsNullOrEmpty(template))
                {
                    return submenustring;
                }
                else
                {
                    return template;
                }
            }
        }
         
        // find insert position to merge into class names of the template. 
        public static int FindClassInsertPosition(string template)
        {
            // find the right place to insert 
            int index = template.IndexOf(" class", StringComparison.OrdinalIgnoreCase);

            if (index == -1)
            {
                return -1;
            }

            int totallen = template.Length;

            int EqualMark = template.IndexOf("=", index);

            if (EqualMark == -1)
            {
                return -1;
            }

            for (int i = EqualMark + 1; i < totallen; i++)
            {
                var currentchar = template[i];
                if (!Lib.Helper.CharHelper.isSpaceCharacters(currentchar))
                {
                    if (currentchar == '"' || currentchar == '\'')
                    {
                        return i + 1;
                    }
                    else
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        public static string GetName(Menu menu, string culture)
        {
            if (menu == null)
            {
                return null;
            }

            if (menu.Values != null && menu.Values.Count > 0 && menu.Values.ContainsKey(culture))
            {
                return menu.Values[culture];
            }

            if (!string.IsNullOrEmpty(menu.Name))
            {
                return menu.Name;
            }
            if (menu.Values.Count > 0)
            {
                return menu.Values.First().Value;
            }
            return null;
        }
    }
} 