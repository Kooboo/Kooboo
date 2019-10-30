//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using System.IO;

namespace Kooboo.Sites.Render.Commands
{
    public class DBCommandSourceProvider : ICommandSourceProvider
    {
        public string GetLayout(RenderContext context, string relativeUrl)
        {
            return GetLayoutSourceFromDb(context, relativeUrl);
        }

        public string GetString(RenderContext context, string relativeUrl)
        {
            return GetSourceFromDb(context, relativeUrl);
        }

        private string GetSourceFromDb(RenderContext context, string filename)
        {
            // try get view...
            var view = context.WebSite.SiteDb().Views.GetByNameOrId(filename);
            if (view != null)
            {
                return view.Body;
            }
            if (filename.StartsWith("/"))
            {
                string name = filename.Substring(1);
                view = context.WebSite.SiteDb().Views.GetByNameOrId(name);
                if (view != null)
                {
                    return view.Body;
                }
            }
            var prefixview = GetPrefixViewSource(context, filename);
            if (!string.IsNullOrEmpty(prefixview))
            {
                return prefixview;
            }

            return null;
        }

        private string GetPrefixViewSource(RenderContext context, string filename)
        {
            string lower = filename.ToLower();
            // check prefix
            if (lower.StartsWith("/_view/"))
            {
                lower = lower.Substring("/_view/".Length);
            }
            else if (lower.StartsWith("/view/"))
            {
                lower = lower.Substring("/view/".Length);
            }
            else if (lower.StartsWith("_view/"))
            {
                lower = lower.Substring("_view/".Length);
            }
            else if (lower.StartsWith("view/"))
            {
                lower = lower.Substring("view/".Length);
            }
            var view = context.WebSite.SiteDb().Views.GetByNameOrId(lower);
            return view?.Body;
        }

        private string GetLayoutSourceFromDb(RenderContext context, string layoutRelativeUrl)
        {
            var layout = context.WebSite.SiteDb().Layouts.GetByNameOrId(layoutRelativeUrl);
            if (layout != null)
            {
                return layout.Body;
            }
            if (layoutRelativeUrl.StartsWith("/"))
            {
                string name = layoutRelativeUrl.Substring(1);
                layout = context.WebSite.SiteDb().Layouts.GetByNameOrId(name);
                if (layout != null)
                {
                    return layout.Body;
                }
            }
            var prefixview = GetPrefixLayoutSource(context, layoutRelativeUrl);
            if (!string.IsNullOrEmpty(prefixview))
            {
                return prefixview;
            }

            return null;
        }

        private static string GetPrefixLayoutSource(RenderContext context, string filename)
        {
            string lower = filename.ToLower();
            // check prefix
            if (lower.StartsWith("/_layout/"))
            {
                lower = lower.Substring("/_layout/".Length);
            }
            else if (lower.StartsWith("/layout/"))
            {
                lower = lower.Substring("/layout/".Length);
            }
            else if (lower.StartsWith("_layout/"))
            {
                lower = lower.Substring("_layout/".Length);
            }
            else if (lower.StartsWith("layout/"))
            {
                lower = lower.Substring("layout/".Length);
            }
            var layout = context.WebSite.SiteDb().Layouts.GetByNameOrId(lower);
            return layout?.Body;
        }

        public byte[] GetBinary(RenderContext context, string relativeUrl)
        {
            return null;
        }

        public Stream GetStream(RenderContext context, string relativeUrl)
        {
            return null;
        }
    }
}