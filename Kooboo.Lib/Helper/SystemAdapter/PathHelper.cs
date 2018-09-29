using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Kooboo.Lib.Helper
{
    public class PathHelper
    {
        public static int GetLastSlash(string path)
        {
            if (RuntimeSystemHelper.IsWindow())
            {
                return path.LastIndexOf('\\');
            }
            return path.LastIndexOf('/');
        }
        public static string GetCaseInsensitiveFile(string root,string fullPath)
        {
            //Kooboo.Data.AppSettings.RootPath
            var folder = new DirectoryInfo(root);
            if (fullPath.IndexOf(root) == 0)
            {
                fullPath = fullPath.Replace(root, "");
            }

            var segments = fullPath.Split('/');
            for (int i = 0; i < segments.Length; i++)
            {
                string segment = segments[i];
                if (i == segments.Length - 1)
                {
                    var files = folder.GetFiles().Where(file =>
                            file.Name.Equals(segment, StringComparison.OrdinalIgnoreCase));
                    if(files==null || files.Count() == 0)
                    {
                        files = folder.GetFiles().Where(file =>
                           file.Name.Split('.')[0].Equals(segment, StringComparison.OrdinalIgnoreCase));
                    }

                    if(files != null && files.Count() > 0)
                    {
                        //default get html
                        var fileinfo = files.FirstOrDefault(file =>
                          file.Name.IndexOf(".html", StringComparison.OrdinalIgnoreCase)>-1);
                        if (fileinfo != null)
                        {
                            segments[i] = fileinfo.Name;
                        }
                        else
                        {
                            segments[i] = files.ToList()[0].Name;
                        }
                    }
                    else
                    {
                        folder = folder.GetDirectories().FirstOrDefault(dir =>
                        dir.Name.Equals(segment, StringComparison.OrdinalIgnoreCase));
                        if (folder != null)
                        {
                            segments[i] = folder.Name;
                        }
                    }
                    
                }
                else
                {
                    folder = folder.GetDirectories().FirstOrDefault(dir =>
                        dir.Name.Equals(segment, StringComparison.OrdinalIgnoreCase));
                    if (folder != null)
                    {
                        segments[i] = folder.Name;
                    }
                    else
                    {
                        break;
                    }

                }
            }

            var reletivePath = string.Join("/", segments);
           
            if (!root.EndsWith("/"))
            {
                root += "/";
            }
            if (reletivePath.StartsWith("/"))
            {
                reletivePath = reletivePath.TrimStart('/');
            }
            string path = root + reletivePath;
            return path;
        }

        public static string CombinePath(string root,string relativePath)
        {
            relativePath = relativePath.TrimStart('/');

            string fullpath = string.Empty;
            //url and linux seperate is / ,but window file seperate is \
            if (RuntimeSystemHelper.IsWindow())
            {
                if (relativePath.StartsWith("\\"))
                {
                    relativePath = relativePath.Substring(1);
                }
                var path = relativePath.Replace("/", "\\");
                fullpath = Path.Combine(root, path);
            }
            else
            {
                fullpath = GetCaseInsensitiveFile(root, relativePath);
            }            
            return fullpath;
        }

        public static string CombineRelativePath(string relativePath,string path)
        {
            if (RuntimeSystemHelper.IsWindow())
            {
                return relativePath + "\\" + path;
            }
            return relativePath + "/" + path;
        }
        public static string JoinPath(string[] segments)
        {
            if (RuntimeSystemHelper.IsWindow())
            {
                return string.Join("\\", segments);
            }

            return string.Join("/", segments);
        }

        public static List<string> GetSegments(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return new List<string>();
            }
            if (RuntimeSystemHelper.IsWindow())
            {
                input = input.Replace('/', '\\');
                input = input.Trim();
                if (input.StartsWith("\\"))
                {
                    input = input.Substring(1);
                }
                return input.Split('\\').ToList();
            }
            else
            {
                input = input.Replace('\\', '/');
                input = input.Trim();
                if (input.StartsWith("/"))
                {
                    input = input.Substring(1);
                }
                return input.Split('/').ToList();
            }
        }
    }
}
