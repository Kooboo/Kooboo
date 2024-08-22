//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Kooboo.Lib.Helper
{
    public class UrlHelper
    {

        public static bool IsValidUrl(string input, bool RequireAbsolute = false)
        {
            //  RFC 3986 also specifies some unreserved characters, which can always be used simply to represent data without any encoding: 
            //abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 -._~
            // Finally, the % character itself is allowed for percent - encodings.  
            // That leaves only the following ASCII characters that are forbidden from appearing in a URL:  
            // The control characters(chars 0 - 1F and 7F), including new line, tab, and carriage return.
            // "<>\^`{|}

            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            for (int i = 0; i < input.Length; i++)
            {
                var currentchar = input[i];
                if (currentchar == '<' || currentchar == '>' || currentchar == '^' || currentchar == '`')
                {
                    return false;
                }
            }

            if (RequireAbsolute)
            {
                string lower = input.ToLower();
                if (lower.StartsWith("https://") || lower.StartsWith("http://"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }


        public static bool IsImage(string url)
        {
            var type = IOHelper.MimeType(url);
            if (string.IsNullOrEmpty(type))
            {
                return false;
            }

            type = type.ToLower();
            return type.Contains("image");
        }

        //the link like: //kooboo.com/abc.png
        public static bool IsRelativeExternal(string link)
        {
            if (link.StartsWith("//") && link.Length > 3)
            {
                var index = link.IndexOf("/", 3);
                if (index > -1)
                {
                    int len = index - 2;
                    if (len > 0)
                    {
                        string domain = link.Substring(2, index - 2);

                        return Kooboo.Lib.Domain.DomainService.IsValidDomain(domain);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// combine base url with sub url. 
        /// baseurl can be absolute url or a relative url.
        /// Warning, this combine only work within Kooboo which we require every url to start with /
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="subUrl"></param>
        /// <returns></returns>
        public static string Combine(string baseUrl, string subUrl, bool RemoveFragment = true)
        {
            if (subUrl == null)
            {
                return baseUrl;
            }
            subUrl = subUrl.Trim();

            if (string.IsNullOrEmpty(subUrl))
            {
                return baseUrl;
            }
            if ((subUrl.ToLower().StartsWith("http://") || subUrl.ToLower().StartsWith("https://")) || string.IsNullOrEmpty(baseUrl) || Kooboo.Lib.Utilities.DataUriService.isDataUri(subUrl))
            {
                return subUrl;
            }

            if (IsRelativeExternal(subUrl))
            {
                if (baseUrl != null)
                {
                    baseUrl = baseUrl.ToLower();
                    if (baseUrl.StartsWith("http://"))
                    {
                        return "http:" + subUrl;
                    }
                    else
                    {
                        return "https:" + subUrl;
                    }
                }
                return subUrl;
            }

            if (baseUrl.ToLower().StartsWith("http"))
            {
                Uri uri = new Uri(baseUrl);

                try
                {
                    uri = new Uri(uri, subUrl);
                }
                catch (Exception)
                {
                }

                return uri.OriginalString;
            }
            else
            {
                string fullurl = Combine("http://www.kooboofakedomaintemp.com", baseUrl);
                string combined = Combine(fullurl, subUrl);

                if (baseUrl.StartsWith(@"\") || baseUrl.StartsWith(@"/"))
                {
                    return UrlHelper.RelativePath(combined, RemoveFragment);
                }
                else
                {
                    return UrlHelper.RelativePath(combined, RemoveFragment).Substring(1);
                }
            }
        }

        /// <summary>
        /// This requires that the absoluteUrl must start with http. 
        /// </summary>
        /// <param name="absoluteUrl"></param>
        /// <param name="isSameHost"></param>
        /// <returns></returns>
        public static string RelativeHostPath(string absoluteUrl, bool isSameHost)
        {
            if (isSameHost)
            {
                return UrlHelper.RelativePath(absoluteUrl);
            }
            else
            {
                if (absoluteUrl.ToLower().StartsWith("http"))
                {
                    string domain = Kooboo.Lib.Helper.UrlHelper.UriHost(absoluteUrl, true);

                    string relativeUrl = UrlHelper.RelativePath(absoluteUrl);
                    if (domain != null)
                    {
                        return "/" + domain + relativeUrl;
                    }
                    else
                    {
                        return relativeUrl;
                    }

                }
                else
                {
                    return UrlHelper.RelativePath(absoluteUrl);
                }
            }
        }

        /// <summary>
        /// make the full url with  http:// into relative url like /content/mycontent
        /// </summary>
        /// <param name="fullurl"></param>
        /// <param name="domain"></param>
        /// <param name="sitepath"></param>
        /// <returns></returns>
        public static string RelativePath(string fullurl, bool RemoveFragment = true)
        {
            if (IsRelativeExternal(fullurl))
            {
                return fullurl;
            }

            if (fullurl.ToLower().StartsWith("http"))
            {
                Uri url = new Uri(fullurl);
                return RelativePath(url, RemoveFragment);
            }
            else
            {
                if (!fullurl.StartsWith("/"))
                {
                    fullurl = "/" + fullurl;
                }
                return fullurl;
            }
        }


        public static string RemoveQueryStringKey(string url, string keyToRemove)
        {
            if (url == null)
            {
                return null;
            }
            url = url.Trim();

            if (url.Contains("?"))
            {

                bool absUrl = url.ToLower().Contains("http://") || url.ToLower().Contains("https://");

                if (!absUrl)
                {
                    if (url.StartsWith("/"))
                    {
                        url = "http://www.kooboo.com" + url;
                    }
                    else
                    {
                        url = "http://www.kooboo.com/" + url;
                    }
                }

                string result = null;

                UriBuilder uriBuilder = new UriBuilder(url);
                NameValueCollection queryParameters = HttpUtility.ParseQueryString(uriBuilder.Query);

                // Remove the query parameter if it exists
                if (queryParameters.AllKeys.Contains(keyToRemove))
                {
                    queryParameters.Remove(keyToRemove);

                    // Reconstruct the URL without the removed query parameter
                    uriBuilder.Query = queryParameters.ToString();
                    result = uriBuilder.Uri.AbsoluteUri;
                }
                else
                {
                    result = url;
                }

                if (!absUrl)
                {
                    return RelativePath(result);
                }
                else
                {
                    return result;
                }

            }

            return url;
        }



        public static string RelativePath(Uri uri, bool RemoveFragment = true)
        {
            string localPath = uri.LocalPath + uri.Query;

            if (!RemoveFragment)
            {
                localPath += uri.Fragment;
            }

            if (string.IsNullOrEmpty(localPath))
            {
                localPath = "/";
            }
            //  return RemoveFragment(RemoveSessionId(localpath));

            return RemoveSessionId(localPath);

        }

        /// <summary>
        /// remove the session id from url input. 
        /// </summary>
        /// <param name="urlinput"></param>
        /// <returns></returns>
        public static string RemoveSessionId(string urlinput)
        {
            int sessionindex = urlinput.IndexOf("session", StringComparison.OrdinalIgnoreCase);

            if (sessionindex > 0)
            {
                int previousmarker;
                previousmarker = urlinput.LastIndexOf("?", sessionindex);
                int previousAnd = urlinput.LastIndexOf("&", sessionindex);
                if (previousAnd > previousmarker)
                {
                    previousmarker = previousAnd;
                }

                if (previousmarker == -1)
                {
                    return urlinput;
                }

                int nextAnd = urlinput.IndexOf("&", sessionindex);

                if (nextAnd > 0)
                {
                    return urlinput.Substring(0, previousmarker + 1) + urlinput.Substring(nextAnd + 1);
                }
                else
                {
                    return urlinput.Substring(0, previousmarker);
                }


            }
            else
            {
                return urlinput;
            }
        }


        /// <summary>
        /// remove links like http://kooboo.com/aaaa.aspx#localsection
        /// </summary>
        /// <param name="urlinput"></param>
        /// <returns></returns>
        public static string RemoveFragment(string urlinput)
        {
            int localIndex = urlinput.LastIndexOf("#");
            if (localIndex >= 0)
            {
                if (localIndex > 0)
                {
                    return urlinput.Substring(0, localIndex);
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return urlinput;
            }
        }

        /// <summary>
        /// get the domain/host of current full url. 
        /// if start with www. remove the www.
        /// </summary>
        /// <param name="fullurl">the full url that contains http:</param>
        /// <returns></returns>
        private static string HostWithoutWWW(string fullurl)
        {
            // fullurl = fullurl.ToLower(); already done before call. 

            if (!fullurl.StartsWith("http"))
            {
                if (fullurl.StartsWith("www."))
                {
                    fullurl = fullurl.Substring(4);
                }

                int slashindex = fullurl.IndexOf("/");
                if (slashindex > 0)
                {
                    return fullurl.Substring(0, slashindex);
                }
                else
                {
                    return fullurl;
                }

            }

            if (Uri.IsWellFormedUriString(fullurl, UriKind.RelativeOrAbsolute))
            {
                Uri url = new Uri(fullurl);
                string host = url.Host.ToLower();
                //with or without www should be treated the same.
                if (host.StartsWith("www."))
                {
                    host = host.Substring(4);
                }
                return host;
            }
            else
            {
                try
                {
                    Uri url = new Uri(fullurl);
                    string host = url.Host.ToLower();
                    //with or without www should be treated the same.
                    if (host.StartsWith("www."))
                    {
                        host = host.Substring(4);
                    }
                    return host;
                }
                catch (Exception)
                {

                }
            }

            return null;

        }




        /// <summary>
        /// the original host reture by Uri class includes www
        /// </summary>
        /// <param name="fullUrl"></param>
        /// <returns></returns>
        public static string UriHost(string fullUrl, bool keepWWW)
        {
            if (fullUrl == null)
            {
                return null;
            }

            fullUrl = fullUrl.ToLower().Replace("\\", "/");

            if (!keepWWW)
            {
                return HostWithoutWWW(fullUrl);
            }

            if (!fullUrl.StartsWith("http"))
            {
                int slashIndex = fullUrl.IndexOf("/");
                if (slashIndex > 0)
                {
                    return fullUrl.Substring(0, slashIndex);
                }
                else
                {
                    return fullUrl;
                }
            }

            if (Uri.IsWellFormedUriString(fullUrl, UriKind.RelativeOrAbsolute))
            {
                Uri url = new Uri(fullUrl);
                return url.Host.ToLower();
            }
            else
            {
                try
                {
                    Uri url = new Uri(fullUrl);
                    return url.Host.ToLower();
                }
                catch (Exception)
                {
                }
            }

            return null;
        }


        public static string GetHost(string Url)
        {
            if (string.IsNullOrWhiteSpace(Url))
            {
                return null;
            }

            var lower = Url.ToLower();

            if (lower.Length <= 10)
            {
                return lower;
            }

            if (lower.StartsWith("https://"))
            {
                var rest = lower.Substring(8);
                var index = rest.IndexOf("/");
                var slashIndex = rest.IndexOf("\\");

                if (slashIndex > 0 && index > slashIndex)
                {
                    index = slashIndex;
                }

                if (index > 0)
                {
                    return rest.Substring(0, index);
                }
                else
                {
                    return rest;
                }

            }
            else if (lower.Contains("http://"))
            {
                var rest = lower.Substring(7);
                var index = rest.IndexOf("/");
                var slashIndex = rest.IndexOf("\\");

                if (index > slashIndex && slashIndex > 0)
                {
                    index = slashIndex;
                }

                if (index > 0)
                {
                    return rest.Substring(0, index);
                }
                else
                {
                    return rest;
                }
            }
            else
            {
                return null;
            }

        }



        /// <summary>
        /// check whether the two url are from the same host or not. 
        /// This is mostly used when we are downloading content. 
        /// </summary>
        /// <param name="UrlOrHostX"></param>
        /// <param name="UrlOrHostY"></param>
        /// <returns></returns>
        public static bool isSameHost(string BaseUrlOrHost, string TargetUrlOrHost)
        {

            if (string.IsNullOrEmpty(BaseUrlOrHost) && string.IsNullOrEmpty(TargetUrlOrHost))
            {
                return false;
            }

            if (string.IsNullOrEmpty(BaseUrlOrHost) || string.IsNullOrEmpty(TargetUrlOrHost))
            {
                return true;
            }

            return UrlHelper.UriHost(BaseUrlOrHost, false) == UrlHelper.UriHost(TargetUrlOrHost, false);

        }

        /// <summary>
        /// get the file name from url. 
        /// </summary>
        /// <param name="fullUrl"></param>
        /// <returns></returns>
        public static string FileName(string fullUrl)
        {
            if (string.IsNullOrEmpty(fullUrl))
            {
                return string.Empty;
            }
            return System.IO.Path.GetFileName(fullUrl.Split('?')[0]);
        }

        public static string FileExtension(string fullUrl)
        {
            if (String.IsNullOrWhiteSpace(fullUrl))
            {
                return String.Empty;
            }

            if (fullUrl.IndexOf("?") > -1)
            {
                var index = fullUrl.IndexOf("?");
                fullUrl = fullUrl.Substring(0, index);
            }

            fullUrl = fullUrl.Trim('\\', '/');

            return System.IO.Path.GetExtension(fullUrl.ToLower());
        }

        public static string GetImageExtensionFromMine(string input)
        {
            input = input.ToLower();

            if (input.Contains("png"))
            {
                return ".png";
            }
            else if (input.Contains("gif"))
            {
                return ".gif";
            }
            else if (input.Contains("jpg"))
            {
                return ".jpg";
            }
            else if (input.Contains("jpeg"))
            {
                return ".jpeg";
            }
            else if (input.Contains("svg"))
            {
                return ".svg";
            }
            else
            {
                return ".none";
            }
        }
        /// <summary>
        /// change http://koobo.com/somepage to /somepage. 
        /// used to add download content to make sure the next link is continue within current domain. 
        /// TODO: should handle https as well. 
        /// </summary>
        /// <param name="pageSource"></param>
        /// <param name="fullURL">The url of current request. </param>
        /// <returns></returns>
        public static string ClearUrlAndEncoding(string fullURL, string pageSource)
        {
            string CorrectURL = ReplaceUrlsWithRelative(fullURL, pageSource);

            return ReplaceMetaSecurityPolicy(CorrectURL);
        }

        /// <summary>
        /// Replace the href inside the url with relative url. 
        /// </summary>
        /// <param name="FullUrl"></param>
        /// <param name="PageSource"></param>
        /// <returns></returns>
        public static string ReplaceUrlsWithRelative(string FullUrl, string PageSource)
        {

            string httpdomain = UriHost(FullUrl, true);

            string urlpattern = "<a.*?href\\s*=\\s*(\"(?<strUrl>\\S.*?)\"|'(?<strUrl>\\S.*?)'|(?<strUrl>.*?)[\\s\\>])";

            return Regex.Replace(PageSource, urlpattern, o => replaceLink(o, httpdomain, FullUrl), RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

        }

        public static string ReplaceMetaSecurityPolicy(string input)
        {
            string metacharset = """
            <meta\s*http-equiv=["']\s*Content-Security-Policy\s*["']\s*content\s*=\s*["']\s*upgrade-insecure-requests\s*["']\s*\/?\s*>
            """;
            return Regex.Replace(input, metacharset, "", RegexOptions.IgnoreCase);
        }

        private static string replaceLink(Match match, string httpDomain, string originalFullUrl)
        {
            string url = match.Groups["strUrl"].Value;

            string FullUrl = Combine(originalFullUrl, url);

            if (isSameHost(FullUrl, originalFullUrl))
            {
                string newURL = RelativePath(FullUrl);

                return match.Value.Replace(url, newURL);
            }
            else
            {
                return match.Value;
            }
        }

        public static List<string> getSegments(string input, bool toLower = true)
        {
            string[] segments;
            if (RuntimeSystemHelper.IsWindow())
            {
                if (toLower)
                {
                    input = input.Replace("/", "\\").ToLower();
                }
                else
                {
                    input = input.Replace("/", "\\");
                }

                segments = input.Split('\\');
            }
            else
            {
                if (toLower)
                {
                    input = input.Replace("\\", "/").ToLower();
                }
                else
                {
                    input = input.Replace("\\", "/");
                }

                segments = input.Split('/');
            }

            List<string> stringList = new List<string>();

            foreach (var item in segments)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    stringList.Add(item);
                }
            }

            return stringList;
        }

        /// <summary>
        /// return the file name to be used for this page. 
        /// </summary>
        /// <param name="fullurl"></param>
        /// <returns></returns>
        public static string GetPageName(string fullurl)
        {

            if (string.IsNullOrEmpty(fullurl))
            {
                return null;
            }

            string path = System.IO.Path.GetFileName(fullurl);

            if (string.IsNullOrEmpty(path))
            {
                int lastslash = fullurl.LastIndexOf("/");

                if (lastslash > -1)
                {
                    path = fullurl.Substring(lastslash);
                }
            }

            if (string.IsNullOrEmpty(path))
            {
                path = fullurl;
            }

            int LastQuestionMark = path.LastIndexOf("?");

            if (LastQuestionMark > 0)
            {
                return path.Substring(0, LastQuestionMark);
            }
            else
            {
                return path;
            }

        }

        /// <summary>
        /// return the file name to be used for this page. 
        /// </summary>
        /// <param name="fullurl"></param>
        /// <returns></returns>
        public static string GetNameFromUrl(string fullurl)
        {
            if (string.IsNullOrEmpty(fullurl))
            {
                return null;
            }

            string path = System.IO.Path.GetFileName(fullurl);

            if (string.IsNullOrEmpty(path))
            {
                int lastsLash = fullurl.LastIndexOf("/");

                if (lastsLash > -1)
                {
                    path = fullurl.Substring(lastsLash);
                }
            }

            if (path == "/")
            {
                path = "_folderroot";
            }

            if (string.IsNullOrEmpty(path))
            {
                path = fullurl;
            }

            int LastQuestionMark = path.LastIndexOf("?");

            if (LastQuestionMark > 0)
            {
                path = path.Substring(0, LastQuestionMark);
            }
            return path;
        }

        public static UrlFileType GetFileType(string url, string ContentType = "")
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return UrlFileType.Unknow;
            }
            string cleanURL = url.ToLower();
            int QuestionMark = url.IndexOf("?");
            if (QuestionMark > 0)
            {
                cleanURL = url.Substring(0, QuestionMark);
            }
            cleanURL = cleanURL.Trim('\t', '\r', '\n');
            var extension = string.Empty;
            try
            {
                if (Uri.TryCreate(cleanURL, UriKind.Absolute, out var uri))
                {
                    if (uri.PathAndQuery == "/") return UrlFileType.PageOrView;
                }

                extension = System.IO.Path.GetExtension(cleanURL);

            }
            catch (Exception)
            {

            }

            if (!string.IsNullOrWhiteSpace(extension))
            {
                if (extension == ".js")
                {
                    return UrlFileType.JavaScript;
                }
                else if (extension == ".css" || extension == ".scss")
                {
                    return UrlFileType.Style;
                }

                if (extension == ".jpg" || extension == ".ico" || extension == ".gif" || extension == ".bmp" || extension == ".png" || extension == ".jpeg" || extension == ".svg" || extension == ".webp" || extension == ".avif" || extension == ".apng ")
                {
                    return UrlFileType.Image;
                }

                if (extension == ".aspx" || extension == ".axd" || extension == ".asx" || extension == ".ashx" || extension == ".asmx" || extension == ".asp" || extension == ".cfm" || extension == ".yaws" || extension == ".html" || extension == ".htm" || extension == ".shtml" || extension == ".xhtml" || extension == ".jhtml" || extension == ".cshtml")
                {
                    return UrlFileType.PageOrView;
                }

                if (extension == ".jsp" || extension == ".jspx" || extension == ".wss" || extension == ".do" || extension == ".action" || extension == ".pl" || extension == ".php" || extension == ".php3" || extension == ".php4" || extension == ".phtml" || extension == ".py" || extension == ".cgi" || extension == ".dll" || extension == ".rb" || extension == ".rhtml")
                {
                    return UrlFileType.PageOrView;
                }

                if (extension == ".swf" || extension == ".flv" || extension == ".mid" || extension == ".midi" || extension == ".mp3" || extension == ".mpg" || extension == ".mpeg" || extension == ".mov" || extension == ".rar" || extension == ".zip" || extension == ".7z" || extension == ".wav" || extension == ".tiff" || extension == ".map" || extension == ".json" || extension == ".xml")
                {
                    return UrlFileType.File;
                }

                if (extension == ".doc" || extension == ".docx" || extension == ".ppt" || extension == ".pptx" || extension == ".xls" || extension == ".xlsx" || extension == ".pdf" || extension == ".pps" || extension == ".txt")
                {
                    return UrlFileType.File;
                }

                if (extension == ".less" || extension == ".sass" || extension == ".scss")
                {
                    return UrlFileType.Style;
                }

                if (extension == ".ts" || extension == ".coffee")
                {
                    return UrlFileType.JavaScript;
                }

            }

            if (string.IsNullOrEmpty(ContentType))
            {
                ContentType = IOHelper.MimeType(url);
            }

            if (!string.IsNullOrWhiteSpace(ContentType))
            {
                ContentType = ContentType.ToLower();

                if (ContentType.Contains("image"))
                {
                    return UrlFileType.Image;
                }

                if (ContentType.Contains("text"))
                {
                    if (ContentType.Contains("css") || ContentType.Contains("style"))
                    {
                        return UrlFileType.Style;
                    }

                    if (ContentType.Contains("script"))
                    {
                        return UrlFileType.JavaScript;
                    }
                    else
                    {
                        return UrlFileType.PageOrView;
                    }
                }
                else if (ContentType.Contains("application"))
                {
                    return UrlFileType.File;
                }
                else if (ContentType.Contains("font"))
                {
                    return UrlFileType.File;
                }
                else if (ContentType.Contains("video"))
                {
                    return UrlFileType.File;
                }
                else if (ContentType.Contains("audio"))
                {
                    return UrlFileType.File;
                }
                else if (ContentType.StartsWith("x-"))
                {
                    return UrlFileType.File;
                }
            }

            if (string.IsNullOrEmpty(extension))
            {
                return UrlFileType.PageOrView;
            }

            string MockName = "kooboofake" + extension;

            var mineType = IOHelper.MimeType(MockName);

            if (mineType.StartsWith("application"))
            {
                return UrlFileType.File;
            }
            else if (mineType.StartsWith("image"))
            {
                return UrlFileType.Image;
            }
            else if (mineType.StartsWith("text"))
            {

                if (mineType.Contains("css"))
                {
                    return UrlFileType.Style;
                }
                else if (mineType.Contains("javascript"))
                {
                    return UrlFileType.JavaScript;
                }
                else
                {
                    return UrlFileType.PageOrView;
                }
            }


            return UrlFileType.PageOrView;

        }

        /// <summary>
        /// replace the disk back slash with url forward slash. 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="withRoot"></param>
        /// <returns></returns>
        public static string ReplaceBackSlash(string input, bool withRoot = false)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            string newValue = input.Replace("\\", "/");

            if (withRoot && !newValue.StartsWith("/"))
            {
                newValue = "/" + newValue;
            }
            return newValue;
        }

        public static string AppendQueryString(string baseUrl, Dictionary<string, string> Parameters)
        {
            if (string.IsNullOrEmpty(baseUrl) || Parameters == null || Parameters.Count == 0)
            {
                return baseUrl;
            }

            string local = null;
            int markLocal = baseUrl.IndexOf("#");
            if (markLocal > -1)
            {
                local = baseUrl.Substring(markLocal);
                baseUrl = baseUrl.Substring(0, markLocal);
            }

            int markIndex = baseUrl.IndexOf("?");
            string queryPart = "";
            string url = baseUrl;
            if (markIndex > 0)
            {
                url = baseUrl.Substring(0, markIndex);
                queryPart = baseUrl.Substring(markIndex + 1);
            }

            var queryStrings = System.Web.HttpUtility.ParseQueryString(queryPart);

            var keys = queryStrings.Keys;
            if (keys != null && keys.Count > 0)
            {
                for (int i = 0; i < keys.Count; i++)
                {
                    var key = keys[i];
                    string value = queryStrings.Get(key);
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (key != null && !Parameters.ContainsKey(key))
                        {
                            Parameters[key] = value;
                        }
                    }
                }
            }

            bool first = true;

            foreach (var item in Parameters)
            {
                string para = item.Key;
                if (para == string.Empty) continue;

                if (!string.IsNullOrEmpty(item.Value))
                {
                    para += "=" + System.Web.HttpUtility.UrlEncode(item.Value);
                }
                if (first)
                {
                    url = url + "?" + para;
                    first = false;
                }
                else
                {
                    url = url + "&" + para;
                }
            }
            if (local != null)
            {
                url += local;
            }
            return url;
        }

        public static string AppendQueryString(string baseurl, string name, string value)
        {
            Dictionary<string, string> query = new Dictionary<string, string>();
            query.Add(name, value);

            return AppendQueryString(baseurl, query);
        }

        public static string BuildQueryString(IEnumerable<KeyValuePair<string, string>> arguments)
        {
            return string.Join("&", arguments.Select(it => $"{it.Key}={PercentEncode(it.Value?.Trim() ?? string.Empty)}"));
        }

        public static bool IsSameUrl(string left, string right)
        {
            if (left == default && right == default) return true;
            if (left == default || right == default) return false;
            left = left.ToLower().Trim();
            var queryStringStart = left.IndexOf('?');
            if (queryStringStart > -1) left = left[..queryStringStart];
            queryStringStart = right.IndexOf('?');
            if (queryStringStart > -1) right = right[..queryStringStart];
            return left == right;
        }

        public static string ToPrUrl(string url)
        {
            if (url.StartsWith("https:", StringComparison.CurrentCultureIgnoreCase))
            {
                return url[6..];
            }

            if (url.StartsWith("http:", StringComparison.CurrentCultureIgnoreCase))
            {
                return url[5..];
            }

            return url;
        }

        public static string GetEncodedLocation(string location, bool onlyHttp = true)
        {
            if (string.IsNullOrEmpty(location))
                return location;
            var builder = new StringBuilder();
            Uri uri;
            // /admin/sites will be parse to uri.schema= file in linux
            if (Uri.TryCreate(location, UriKind.Absolute, out uri))
            {
                if (!string.IsNullOrEmpty(uri.Scheme))
                {
                    void UseBaseUrl()
                    {
                        var baseUrl = uri.Scheme + "://" + uri.Authority;
                        builder.Append(baseUrl);
                        location = location.Replace(baseUrl, "");
                    }

                    if (onlyHttp)
                    {
                        if (uri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase) || uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
                        {
                            UseBaseUrl();
                        }
                    }
                    else
                    {
                        UseBaseUrl();
                    }
                }
            }

            var queryString = string.Empty;

            int questionMark = location.IndexOf("?");
            if (questionMark > -1)
            {
                queryString = location.Substring(questionMark);
                location = location.Substring(0, questionMark);

            }
            var segments = location.Split('/');

            for (var i = 0; i < segments.Length; i++)
            {
                var seg = segments[i];
                builder.Append(System.Net.WebUtility.UrlEncode(seg));
                if (segments.Length - 1 != i)
                {
                    builder.Append("/");
                }

            }
            if (!string.IsNullOrEmpty(queryString))
            {
                builder.Append(queryString);
            }
            return builder.ToString();
        }

        public enum UrlFileType
        {
            Unknow = 0,
            Image = 1,
            JavaScript = 2,
            Style = 3,
            File = 4,
            PageOrView = 5
        }

        public static string GetEncodePath(string path)
        {
            List<string> list = new List<string>();
            string[] array = path.Split('/');
            foreach (string value in array)
            {
                list.Add(PercentEncode(value));
            }

            return string.Join("/", list);
        }

        public static string PercentEncode(string raw)
        {
            if (raw == null)
            {
                return null;
            }

            StringBuilder stringBuilder = new StringBuilder();
            string text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
            byte[] bytes = Encoding.UTF8.GetBytes(raw);
            for (int i = 0; i < bytes.Length; i++)
            {
                char c = (char)bytes[i];
                if (text.IndexOf(c) >= 0)
                {
                    stringBuilder.Append(c);
                    continue;
                }

                stringBuilder.Append("%").Append(string.Format(CultureInfo.InvariantCulture, "{0:X2}", (int)c));
            }

            return stringBuilder.ToString().Replace("+", "%20").Replace("*", "%2A")
                .Replace("%7E", "~");
        }
    }
}
