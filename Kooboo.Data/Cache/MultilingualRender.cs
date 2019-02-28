//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Language;
using System;
using System.Collections.Generic;


namespace Kooboo.Data.Cache
{
    public static class MultiLingualRender
    {
        private static object _locker = new object();

        private static object _htmlcachelock = new object(); 

        private static Dictionary<Guid, List<LanguageTask>> Cache = new Dictionary<Guid, List<LanguageTask>>();

        private static Dictionary<string, Dictionary<Guid, string>> HtmlCache = new Dictionary<string, Dictionary<Guid, string>>(StringComparer.OrdinalIgnoreCase); 

        private static Dictionary<Guid, string> GetLangHtmlCache(string lang)
        {
            if (!HtmlCache.ContainsKey(lang))
            {
                lock(_htmlcachelock)
                {
                    if (!HtmlCache.ContainsKey(lang))
                    { 
                        Dictionary<Guid, string> langcache = new Dictionary<Guid, string>();
                        HtmlCache[lang] = langcache; 
                    }  
                } 
            }
            return HtmlCache[lang]; 
        }

        
        public static string GetHtml(RenderContext context, Guid Key)
        {
            var lang = LanguageSetting.GetCmsLangCode(context);
            var cache = GetLangHtmlCache(lang); 
            if (cache.ContainsKey(Key))
            {
                return cache[Key]; 
            }
            return null; 
        }
        public static string SetGetHtml(RenderContext context, Guid Key, string source)
        {
            var lang = LanguageSetting.GetCmsLangCode(context);
            var cache = GetLangHtmlCache(lang);

            string langhtml = null;

            if (!cache.ContainsKey(Key))
            {
                lock (_locker)
                {
                    var tasks = LanguageTaskHelper.ParseDom(source);
                    langhtml = LanguageTaskHelper.Render(tasks, lang);
                    cache[Key] = langhtml;
                }
            }
            else
            {
                langhtml = cache[Key]; 
            }
            return langhtml; 
        }
        
        #region "Javascript"
        // js only allow one js file to have language. 

        private static object _jscachelock = new object();
        private static Dictionary<string, byte[]> jsCache = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
      
        public static byte[] GetJs(RenderContext context)
        {
            string lang = LanguageSetting.GetCmsLangCode(context);

            if (!jsCache.ContainsKey(lang))
            {
                return null; 
            } 
            return jsCache[lang];
        }

        public static byte[] SetGetJs(RenderContext context, byte[] binary)
        {
            string lang = LanguageSetting.GetCmsLangCode(context);
         
            if (!jsCache.ContainsKey(lang))
            {
                lock(_jscachelock)
                {
                    if (!jsCache.ContainsKey(lang))
                    {
                        string text = System.Text.Encoding.UTF8.GetString(binary);
                        var tasks = LanguageTaskHelper.ParseJs(text);
                        string langjs = LanguageTaskHelper.Render(tasks, lang);
                        jsCache[lang] = System.Text.Encoding.UTF8.GetBytes(langjs);
                    }
                } 
            }
            return jsCache[lang];
        }
 
        #endregion 
    }
}
