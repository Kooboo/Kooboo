//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Kooboo.Extensions;
using System.Linq;

namespace Kooboo.Data.Context
{
    public class RequestManager
    {
        // get the value in query string or form only... 
        public static string GetHttpValue(HttpRequest request, string name)
        {
            string value = request.QueryString.Get(name);
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }

            value = request.Forms.Get(name);
            if (!string.IsNullOrEmpty(value))
            {
                return value;
            }
            return request.Headers.Get(name);
        }

        private static string TryGetValue(HttpRequest request, string name,bool needDecode=true)
        {
            string Value = request.QueryString.Get(name);
            if (!string.IsNullOrEmpty(Value))
            {
                if(needDecode)
                    Value = System.Net.WebUtility.UrlDecode(Value);
                return Value;
            }

            Value = request.Forms.Get(name);
            if (!string.IsNullOrEmpty(Value))
            {
                return Value;
            }

            Value = request.Headers.Get(name);
            if (!string.IsNullOrEmpty(Value))
            {
                return Value;
            }

            if (request.Cookies.ContainsKey(name))
            {
                return request.Cookies[name];
            }

            if (!string.IsNullOrEmpty(request.Body))
            {
                string body = request.Body;
                if (Kooboo.Lib.Helper.JsonHelper.IsJson(body))
                {
                    try
                    {
                        var converted = Lib.Helper.JsonHelper.DeserializeObject(body);

                        if (converted is JArray)
                        {
                            var array = (JArray)converted;
                            foreach (var item in array)
                            {
                                if (item is JObject)
                                {
                                    var itemobject = (JObject)item;
                                    if (itemobject != null)
                                    {
                                        foreach (var itemproperty in itemobject.Properties())
                                        {
                                            if (itemproperty.Name.ToLower() == name.ToLower() && !string.IsNullOrEmpty(itemproperty.Value.ToString()))
                                            {
                                                return itemproperty.Value.ToString();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (converted is JObject)
                        {
                            var data = (JObject)converted;

                            if (data != null)
                            {
                                foreach (var item in data.Properties())
                                {
                                    if (item.Name.ToLower() == name.ToLower() && !string.IsNullOrEmpty(item.Value.ToString()))
                                    {
                                        return item.Value.ToString();
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // throw;
                    }
                }
            }

            if (request.Model != null)
            {
                try
                {
                    var value = Kooboo.Lib.Reflection.Dynamic.GetObjectMember(request.Model, name);
                    if (value != null)
                    {
                        return value.ToString();
                    }
                }
                catch (Exception)
                {
                    //throw;
                }
            }
            return Value;
        }

        public static string GetValue(HttpRequest request, string key,bool needDecode=true)
        {
            return TryGetValue(request, key,needDecode);
        }

        public static string GetValue(HttpRequest request, params string[] names)
        {
            for (int i = 0; i < names.Length; i++)
            {
                string name = names[i];
                var value = GetValue(request, name);
                if (!string.IsNullOrEmpty(value))
                {
                    return value;
                }
            }
            return null;
        }

        public static string GetSetCulture(WebSite WebSite, RenderContext context)
        {
            if (WebSite == null)
            {
                return null;
            }

            if (!WebSite.EnableMultilingual)
            {
                return WebSite.DefaultCulture; 
            }

            var request = context.Request;

            if (!string.IsNullOrWhiteSpace(request.Culture))
            {
                SetCultureCookie(context, request.Culture);
                return request.Culture;
            }
            string culture = request.GetValue("lang", "language");

            if (!string.IsNullOrEmpty(culture) && WebSite.Culture.ContainsKeyIgnoreCase(culture))
            {
                return culture;
            }

            if (WebSite.EnableSitePath)
            {
                if (context.Request.RawRelativeUrl.Length > 3)
                {
                    int NextSlash = request.RawRelativeUrl.IndexOf("/", 1);
                    string path = string.Empty;
                    if (NextSlash > 0)
                    {
                        path = request.RawRelativeUrl.Substring(1, NextSlash - 1);
                    }
                    else
                    {
                        path = request.RawRelativeUrl.Substring(1);
                    }

                    if (!string.IsNullOrEmpty(path))
                    {
                        string lowerpath = path.ToLower();
                        foreach (var item in WebSite.SitePath)
                        {
                            if (!string.IsNullOrEmpty(item.Value))
                            {
                                if (item.Value.ToLower() == lowerpath)
                                {
                                    request.RelativeUrl = request.RawRelativeUrl.Substring(NextSlash);

                                    SetCultureCookie(context, item.Key);
                                    return item.Key;
                                }
                            }
                        }

                    }

                }
            }

            if (context.User !=null)
            {
                if (context.User.Language !=null)
                {
                    var userlang = context.User.Language; 
                    if (WebSite.Culture.ContainsKey(userlang))
                    {
                        SetCultureCookie(context, userlang);
                        return userlang; 
                    } 
                } 
            }

            if (context.Request.Cookies.ContainsKey("_kooboo_culture"))
            {
                var cookieculture = context.Request.Cookies.GetValue("_kooboo_culture"); 

                if (WebSite.Culture.ContainsKey(cookieculture))
                {
                    return cookieculture; 
                }

            }

            // Auto Detect culture .
            if (WebSite.AutoDetectCulture)
            {
                string detected = DetectCulture(WebSite, context.Request);
                SetCultureCookie(context, detected);
                return detected; 
            }
            else
            {
                return WebSite.DefaultCulture;
            }
        }

        public static void SetCultureCookie(RenderContext context, string culture)
        {
            if (context.Request.Cookies.ContainsKey("_kooboo_culture"))
            {
                string value = context.Request.Cookies.GetValue("_kooboo_culture");
                if (value != null && value != culture)
                {
                    context.Response.AppendCookie("_kooboo_culture", culture, 5);
                }
            }
            else
            {
                context.Response.AppendCookie("_kooboo_culture", culture, 5);
            }
        }

        public static void ParseQueryString(WebSite website, HttpRequest request)
        {
            string relativeUrl = request.RawRelativeUrl;
            bool hasquery = false;

            string strpageinline = request.QueryString.Get(Constants.Site.PageInlineParameter);

            if (!string.IsNullOrEmpty(strpageinline))
            {
                string value = strpageinline.ToLower();

                if (value == Constants.Site.RequestChannel.Design)
                {
                    request.Channel = RequestChannel.InlineDesign;
                }
                else if (value == Constants.Site.RequestChannel.Draft)
                {
                    request.Channel = RequestChannel.Draft;
                }

                request.QueryString.Remove(Constants.Site.PageInlineParameter);
                hasquery = true;
            }

            if (request.QueryString.AllKeys.Contains("kooboodebug"))
            {
                request.Channel = RequestChannel.InlineDesign;    
                request.QueryString.Remove("kooboodebug");
                hasquery = true;
            }
                
            // alternative view. 
            string stralterview = request.QueryString.Get(Constants.Site.AlternativeViewQueryName);

            if (!string.IsNullOrEmpty(stralterview))
            {
                string values = stralterview;

                string[] valuearray = values.Split(',');

                foreach (var item in valuearray)
                {
                    int intvalue = 0;

                    if (int.TryParse(item, out intvalue))
                    {
                        if (intvalue != 0)
                        {
                            if (request.AltervativeViews == null)
                            {
                                request.AltervativeViews = new HashSet<int>();
                            }

                            request.AltervativeViews.Add(intvalue);
                        }
                    }
                }
            }

            if (hasquery)
            {
                // rebuild relative url. 
                Dictionary<string, string> querystring = new Dictionary<string, string>();
                foreach (var item in request.QueryString.Keys)
                {
                    if (item != null)
                    {
                        string itemvalue = request.QueryString.Get(item.ToString());
                        querystring.Add(item.ToString(), itemvalue);
                    }
                }
                string newrelative = request.Path;
                newrelative = Lib.Helper.UrlHelper.AppendQueryString(newrelative, querystring);
                relativeUrl = newrelative;
            }
            request.RawRelativeUrl = relativeUrl;
            request.RelativeUrl = relativeUrl;
            PraseSitePath(website, request);
        }

        public static void PraseSitePath(WebSite website, HttpRequest request)
        {
            string relativeUrl = request.RawRelativeUrl;
            if (website.EnableSitePath)
            {
                if (relativeUrl.Length >= 3)
                {
                    int NextSlash = relativeUrl.IndexOf("/", 1);
                    if (NextSlash == -1)
                    {
                        NextSlash = relativeUrl.IndexOf("?", 1); 
                    }
                    string path = string.Empty;
                    if (NextSlash > 0)
                    {
                        path = relativeUrl.Substring(1, NextSlash - 1);
                    }
                    else
                    {
                        path = relativeUrl.Substring(1);
                    }
                    if (!string.IsNullOrEmpty(path))
                    {
                        string lowerpath = path.ToLower();

                        foreach (var item in website.SitePath)
                        {
                            if (!string.IsNullOrEmpty(item.Value))
                            {
                                if (item.Value.ToLower() == lowerpath)
                                {
                                    request.SitePath = path;
                                    request.Culture = item.Key;
                                    if (NextSlash > -1)
                                    {
                                        request.RelativeUrl = relativeUrl.Substring(NextSlash);
                                        if (request.RelativeUrl.StartsWith("?"))
                                        {
                                            request.RelativeUrl = "/" + request.RelativeUrl; 
                                        }
                                    }
                                    else
                                    {
                                        request.RelativeUrl = "/";
                                    }
                                    break;
                                }
                            }
                        }


                    }

                }
            }
        }


        public static string DetectCulture(WebSite website, HttpRequest request)
        {
            if (website.EnableMultilingual)
            {
                string culture = null;

                var header = request.Headers.Get("Accept-Language");

                culture = DectecCultureByAcceptLanguage(website, header);

                if (culture != null)
                {
                    return culture;
                }

                var ip = request.IP;
                var ipcountry = Kooboo.Data.GeoLocation.IPLocation.GetIpCountry(ip);
                if (ipcountry != null)
                {
                    culture = DetectCultureByCountryCode(website, ipcountry.CountryCode);
                    if (culture != null)
                    {
                        return culture;
                    }
                }
            }

            return website.DefaultCulture;
        }



        public static string DetectCultureByCountryCode(WebSite website, string countrycode)
        {

            var langlist = Language.LanguageSetting.GetRegionLangCodes(countrycode);

            if (langlist != null && langlist.Any())
            {
                foreach (var item in langlist)
                {
                    if (website.Culture.ContainsKeyIgnoreCase(item))
                    {
                        return item.ToLower();
                    }
                }
            }
            return null;
        }

        public static string DectecCultureByAcceptLanguage(WebSite website, string acceptLanguageHeader)
        {
            if (acceptLanguageHeader == null)
            {
                return null; 
            }

            string lowerdefault = website.DefaultCulture.ToLower();

            string[] parts = acceptLanguageHeader.Split(Language.LanguageSetting.LangSep, StringSplitOptions.RemoveEmptyEntries);

            /// bool hasdefault = false; 
            foreach (var item in parts)
            {
                if (!string.IsNullOrEmpty(item) && item.Length >= 2)
                {
                    string lang = item.ToLower().Trim();
                    if (lang.Length > 2)
                    {
                        lang = lang.Substring(0, 2);
                    }

                    if (lang != lowerdefault && website.Culture.ContainsKeyIgnoreCase(lang))
                    {
                        return lang;
                    }
                }
            }

            //if (hasdefault)
            //{
            //    return lowerdefault;
            //}
            return null;
        }



    }
}
