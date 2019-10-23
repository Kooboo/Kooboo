//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Lib.Helper;
using System;

namespace Kooboo.Render
{
    public class RenderHelper
    {
        public static UrlFileType GetFileType(string url, string contentType = "")
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return UrlFileType.Unknow;
            }
            string cleanurl = url.ToLower();
            int questionMark = url.IndexOf("?");
            if (questionMark > 0)
            {
                cleanurl = url.Substring(0, questionMark);
            }
            cleanurl = cleanurl.Trim('\t', '\r', '\n');
            string extension = System.IO.Path.GetExtension(cleanurl);

            if (!string.IsNullOrWhiteSpace(extension))
            {
                if (extension == ".js")
                {
                    return UrlFileType.JavaScript;
                }
                else if (extension == ".css")
                {
                    return UrlFileType.Style;
                }

                if (extension == ".jpg" || extension == ".ico" || extension == ".gif" || extension == ".bmp" || extension == ".png" || extension == ".jpeg" || extension == ".svg")
                {
                    return UrlFileType.Image;
                }

                if (extension == ".aspx" || extension == ".axd" || extension == ".asx" || extension == ".ashx" || extension == ".asmx" || extension == ".asp" || extension == ".cfm" || extension == ".yaws" || extension == ".html" || extension == ".htm" || extension == ".shtml" || extension == ".xhtml" || extension == ".jhtml" || extension == ".cshtml")
                {
                    return UrlFileType.Html;
                }

                if (extension == ".jsp" || extension == ".jspx" || extension == ".wss" || extension == ".do" || extension == ".action" || extension == ".pl" || extension == ".php" || extension == ".php3" || extension == ".php4" || extension == ".phtml" || extension == ".py" || extension == ".cgi" || extension == ".dll" || extension == ".rb" || extension == ".rhtml" || extension == ".txt")
                {
                    return UrlFileType.Html;
                }

                if (extension == ".swf" || extension == ".flv" || extension == ".mid" || extension == ".midi" || extension == ".mp3" || extension == ".mpg" || extension == ".mpeg" || extension == ".mov" || extension == ".rar" || extension == ".zip" || extension == ".7zip" || extension == ".wav" || extension == ".tiff")
                {
                    return UrlFileType.File;
                }

                if (extension == ".doc" || extension == ".docx" || extension == ".ppt" || extension == ".pptx" || extension == ".xls" || extension == ".xlsx" || extension == ".pdf" || extension == ".pps")
                {
                    return UrlFileType.File;
                }
            }

            if (!string.IsNullOrWhiteSpace(contentType))
            {
                contentType = contentType.ToLower();

                if (contentType.Contains("image"))
                {
                    return UrlFileType.Image;
                }

                if (contentType.Contains("text"))
                {
                    if (contentType.Contains("css") || contentType.Contains("style"))
                    {
                        return UrlFileType.Style;
                    }

                    return contentType.Contains("script") ? UrlFileType.JavaScript : UrlFileType.Html;
                }
                else if (contentType.Contains("application"))
                {
                    return UrlFileType.File;
                }
            }

            if (string.IsNullOrEmpty(extension))
            {
                return UrlFileType.Html;
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
                    return UrlFileType.Html;
                }
            }

            return UrlFileType.File;
        }

        public static string CombinePath(string root, string relativeUrl)
        {
            return Kooboo.Lib.Compatible.CompatibleManager.Instance.System.CombinePath(root, relativeUrl);
        }

        public static string GetRelativeUrl(Uri absoluteUri, RenderOption option)
        {
            string rawRelativeUrl = Kooboo.Lib.Helper.UrlHelper.RelativePath(absoluteUri);
            return GetRelativeUrl(rawRelativeUrl, option);
        }

        public static string GetRelativeUrl(string rawRelativeUrl, RenderOption option)
        {
            string relativeUrl = RemoveQuestionMark(rawRelativeUrl);
            if (!string.IsNullOrEmpty(option.StartPath))
            {
                if (relativeUrl.ToLower().StartsWith(option.StartPath))
                {
                    relativeUrl = relativeUrl.Substring(option.StartPath.Length);
                }
            }
            return relativeUrl;
        }

        public static string RemoveQuestionMark(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            int mark = input.IndexOf("?");
            return mark > 0 ? input.Substring(0, mark) : input;
        }
    }

    public enum UrlFileType
    {
        Unknow = 0,
        Image = 1,
        JavaScript = 2,
        Style = 3,
        File = 4,
        Html = 5
    }
}