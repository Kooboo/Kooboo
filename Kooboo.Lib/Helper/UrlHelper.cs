//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

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
            if (link.StartsWith("//"))
            {
                var index = link.IndexOf("/", 3);
                if (index > -1)
                {
                    int len = index - 2;
                    if (len > 0)
                    {
                        string domain = link.Substring(2, index - 2);

                        return IsValidDomain(domain);
                    }
                }
            }
            return false;
        }

        public static bool IsValidDomain(string domain)
        {
            if (domain == null)
            {
                return false;
            }

            int index = domain.LastIndexOf(".");
            if (index == -1 || index == 0)
            {
                return false;
            }

            var lastpart = domain.Substring(index, domain.Length - index);
            if (string.IsNullOrEmpty(lastpart))
            {
                return false;
            }

            if (lastpart.Length < 3)
            {
                return false;
            }

            for (int i = 0; i < lastpart.Length; i++)
            {
                if (!Helper.CharHelper.IsAscii(lastpart[i]) && lastpart[i] != '.')
                {
                    return false;
                }
            }

            //for (int i = 0; i < domain.Length; i++)
            //{
            //    var current = domain[i];
            //    if (!Helper.CharHelper.isAlphanumeric(current))
            //    {
            //        if (current == '-' || current == '-' || current == '.')
            //        {
            //            continue;
            //        }
            //    }
            //}

            return true;
            //A complete domain name must have one or more subdomain names and one top-level domain name.
            //A complete domain name must use dots(.) to separate domain names.
            //Domain names must use only alphanumeric characters and dashes(-).
            //Domain names must not begin or end with dashes(-).
            //Domain names must mot have more than 63 characters.
            //The top-level domain name must be one of the predefined top - level domain names, like (com), (org), or (ca)
        }

        /// <summary>
        /// combine base url with sub url. 
        /// baseurl can be absolute url or a relative url.
        /// Warning, this combine only work within Kooboo which we require every url to start with /
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="subUrl"></param>
        /// <returns></returns>
        public static string Combine(string baseUrl, string subUrl)
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
            if (subUrl.ToLower().StartsWith("http") || string.IsNullOrEmpty(baseUrl) || Kooboo.Lib.Utilities.DataUriService.isDataUri(subUrl))
            {
                return subUrl;
            }

            if (IsRelativeExternal(subUrl))
            {
                return subUrl; 
            }

            if (baseUrl.ToLower().StartsWith("http"))
            {
                Uri baseuri = new Uri(baseUrl);
                Uri cssuri = new Uri(baseuri, subUrl);
                return cssuri.OriginalString;
            }
            else
            {
                string fullurl = Combine("http://www.kooboofakedomaintemp.com", baseUrl);
                string combined = Combine(fullurl, subUrl);

                if (baseUrl.StartsWith(@"\") || baseUrl.StartsWith(@"/"))
                {
                    return UrlHelper.RelativePath(combined);
                }
                else
                {
                    return UrlHelper.RelativePath(combined).Substring(1);
                }
            }
        }

        /// <summary>
        /// This requires that the absoluteUrl must start with http. 
        /// </summary>
        /// <param name="absoluteUrl"></param>
        /// <param name="isSameHost"></param>
        /// <returns></returns>
        public static string RelativePath(string absoluteUrl, bool isSameHost)
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
        public static string RelativePath(string fullurl)
        {
            if (IsRelativeExternal(fullurl))
            {
                return fullurl; 
            }

            if (fullurl.ToLower().StartsWith("http"))
            {
                Uri url = new Uri(fullurl);
                return RelativePath(url);
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

        public static string RelativePath(Uri uri)
        {
            string localpath = uri.LocalPath + uri.Query;

            if (string.IsNullOrEmpty(localpath))
            {
                localpath = "/";
            }
            return RemoveLocalLink(RemoveSessionId(localpath));
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
        public static string RemoveLocalLink(string urlinput)
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

            return null;

        }




        /// <summary>
        /// the original host reture by Uri class includes www
        /// </summary>
        /// <param name="fullurl"></param>
        /// <returns></returns>
        public static string UriHost(string fullurl, bool keepwww)
        {
            if (fullurl == null)
            {
                return null;
            }

            fullurl = fullurl.ToLower().Replace("\\", "/");

            if (!keepwww)
            {
                return HostWithoutWWW(fullurl);
            }

            if (!fullurl.StartsWith("http"))
            {
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
                return url.Host.ToLower();
            }
            else
            {
                try
                {
                    Uri url = new Uri(fullurl);
                    return url.Host.ToLower();
                }
                catch (Exception)
                {
                }
            }

            return null;
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
        /// <param name="fullurl"></param>
        /// <returns></returns>
        public static string FileName(string fullurl)
        {
            if (string.IsNullOrEmpty(fullurl))
            {
                return string.Empty;
            }
            return System.IO.Path.GetFileName(fullurl.Split('?')[0]);
        }

        public static string FileExtension(string fullurl)
        {
            if (String.IsNullOrWhiteSpace(fullurl))
            {
                return String.Empty;
            }

            if (fullurl.IndexOf("?") > -1)
            {
                var index = fullurl.IndexOf("?");
                fullurl = fullurl.Substring(0, index);
            }

            fullurl = fullurl.Trim('\\', '/');
            //fullurl = fullurl.Remove('\n');
            //fullurl = fullurl.Remove('\r');

            return System.IO.Path.GetExtension(fullurl.ToLower());
        }

        public static string GetImageExtensionFromMine(string input)
        {
            input = input.ToLower();

            if (input.Contains("png"))
            {
                return "png";
            }
            else if (input.Contains("gif"))
            {
                return "gif";
            }
            else if (input.Contains("jpg"))
            {
                return "jpg";
            }
            else if (input.Contains("jpeg"))
            {
                return "jpeg";
            }
            else if (input.Contains("svg"))
            {
                return "svg";
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
        /// <param name="pagesource"></param>
        /// <param name="fullurl">The url of current request. </param>
        /// <returns></returns>
        public static string ClearUrlAndEncoding(string fullurl, string pagesource)
        {
            string righturl = ReplaceUrlsWithRelative(fullurl, pagesource);

            return ReplaceMetaCharSet(righturl);
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

            return Regex.Replace(PageSource, urlpattern, o => replacelink(o, httpdomain, FullUrl), RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

        }

        public static string ReplaceMetaCharSet(string input)
        {
            return input;
            // string metacharset = "<meta.*?charset.*?>";
            //return Regex.Replace(input, metacharset, "", RegexOptions.IgnoreCase);
        }

        private static string replacelink(Match match, string httpdomain, string originalFullUrl)
        {
            string url = match.Groups["strUrl"].Value;

            string FullUrl = Combine(originalFullUrl, url);

            if (isSameHost(FullUrl, originalFullUrl))
            {
                string newurl = RelativePath(FullUrl);

                return match.Value.Replace(url, newurl);
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

            List<string> stringlist = new List<string>();

            foreach (var item in segments)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    stringlist.Add(item);
                }
            }

            return stringlist;
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
                int lastslash = fullurl.LastIndexOf("/");

                if (lastslash > -1)
                {
                    path = fullurl.Substring(lastslash);
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
            string cleanurl = url.ToLower();
            int QuestionMark = url.IndexOf("?");
            if (QuestionMark > 0)
            {
                cleanurl = url.Substring(0, QuestionMark);
            }
            cleanurl = cleanurl.Trim('\t', '\r', '\n');
            var extension = string.Empty;
            try
            {
                extension = System.IO.Path.GetExtension(cleanurl);
            }
            catch (Exception ex)
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

                if (extension == ".jpg" || extension == ".ico" || extension == ".gif" || extension == ".bmp" || extension == ".png" || extension == ".jpeg" || extension == ".svg")
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

                if (extension == ".swf" || extension == ".flv" || extension == ".mid" || extension == ".midi" || extension == ".mp3" || extension == ".mpg" || extension == ".mpeg" || extension == ".mov" || extension == ".rar" || extension == ".zip" || extension == ".7z" || extension == ".wav" || extension == ".tiff")
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

            string fakename = "kooboofake" + extension;

            var minetype = IOHelper.MimeType(fakename);

            if (minetype.StartsWith("application"))
            {
                return UrlFileType.File;
            }
            else if (minetype.StartsWith("image"))
            {
                return UrlFileType.Image;
            }
            else if (minetype.StartsWith("text"))
            {

                if (minetype.Contains("css"))
                {
                    return UrlFileType.Style;
                }
                else if (minetype.Contains("javascript"))
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
            string newvalue = input.Replace("\\", "/");

            if (withRoot && !newvalue.StartsWith("/"))
            {
                newvalue = "/" + newvalue;
            }
            return newvalue;
        }

        public static string AppendQueryString(string baseUrl, Dictionary<string, string> Parameters)
        {
            if (string.IsNullOrEmpty(baseUrl) || Parameters == null || Parameters.Count == 0)
            {
                return baseUrl;
            }

            string local = null;
            int marklocal = baseUrl.IndexOf("#");
            if (marklocal > -1)
            {
                local = baseUrl.Substring(marklocal);
                baseUrl = baseUrl.Substring(0, marklocal);
            }

            int markindex = baseUrl.IndexOf("?");
            string querypart = "";
            string url = baseUrl;
            if (markindex > 0)
            {
                url = baseUrl.Substring(0, markindex);
                querypart = baseUrl.Substring(markindex + 1);
            }

            var queryStrings = System.Web.HttpUtility.ParseQueryString(querypart);

            var keys = queryStrings.Keys;
            if (keys != null && keys.Count > 0)
            {
                for (int i = 0; i < keys.Count; i++)
                {
                    var key = keys[i];
                    string value = queryStrings.Get(key);
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (!Parameters.ContainsKey(key))
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


        public static string GetEncodedLocation(string location)
        {
            if (string.IsNullOrEmpty(location))
                return location;
            var builder = new StringBuilder();
            Uri uri;
            // /admin/sites will be parse to uri.schema= file in linux
            if (Uri.TryCreate(location, UriKind.Absolute, out uri))
            {
                if (!string.IsNullOrEmpty(uri.Scheme) &&
                    (uri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase) ||
                    uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase)))
                {
                    var baseUrl = uri.Scheme + "://" + uri.Authority;
                    builder.Append(baseUrl);
                    location = location.Replace(baseUrl, "");
                }
            }

            var queryString = string.Empty;

            int questionmark = location.IndexOf("?");
            if (questionmark > -1)
            {
                queryString = location.Substring(questionmark);
                location = location.Substring(0, questionmark);

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
    }
}
