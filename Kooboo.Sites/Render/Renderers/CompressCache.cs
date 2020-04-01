using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Render.Renderers
{
    public static class CompressCache
    {
        private static Dictionary<Guid, CompressObject> cache = new Dictionary<Guid, CompressObject>();

        public static void Set(Guid Id, long version, string content)
        {
            cache[Id] = new CompressObject() { version = version, Content = content };
        }

        public static string Get(Guid Id, long version, string content, CompressType doctype)
        {
            if (Id == default(Guid) || version == 0 || string.IsNullOrWhiteSpace(content))
            {
                return null;
            }

            if (cache.ContainsKey(Id))
            {
                var item = cache[Id];
                if (item.version == version)
                {
                    if (item.Content != null)
                    {
                        return item.Content;
                    }
                }
            }

            string result = null;

            switch (doctype)
            {
                case CompressType.js:
                    NUglify.JavaScript.CodeSettings setting = new NUglify.JavaScript.CodeSettings();
                    var js = NUglify.Uglify.Js(content, setting);
                    if (!js.HasErrors)
                    {
                        result = js.Code;
                    }
                    break;
                case CompressType.css:

                    var css = NUglify.Uglify.Css(content);
                    if (!css.HasErrors)
                    {
                        result = css.Code;
                    }
                    break;

                case CompressType.html:

                    var html = NUglify.Uglify.Html(content);
                    if (!html.HasErrors)
                    {
                        result = html.Code;
                    }
                    break;
                default:
                    break;
            }

            // set item. 

            Set(Id, version, result);

            if (result == null)
            {
                return content;
            }
            return result;
        }
 
    }

    public enum CompressType
    {
        js = 1,
        css = 2,
        html = 3
    }

    public class CompressObject
    {
        public long version { get; set; }

        public string Content { get; set; }
    }
}
