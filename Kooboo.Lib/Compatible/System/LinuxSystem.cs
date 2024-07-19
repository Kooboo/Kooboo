//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kooboo.Lib.Compatible
{
    public class LinuxSystem : ISystem
    {
        public int GetLastSlash(string path)
        {
            return path.LastIndexOf('/');
        }

        public string GetSlash()
        {
            return "/";
        }

        #region path
        public string CombinePath(string root, string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return root;

            relativePath = relativePath.TrimStart('/');
            //linux file is case sensitive

            var folder = new DirectoryInfo(root);
            if (relativePath.IndexOf(root, StringComparison.OrdinalIgnoreCase) == 0)
            {
                relativePath = relativePath.ToLower().Replace(root.ToLower(), "");
            }
            relativePath = relativePath.Replace("\\", "/");

            #region get new segments
            var segments = relativePath.Split('/');

            for (int i = 0; i < segments.Length; i++)
            {
                string segment = segments[i];

                if (i != segments.Length - 1)
                {
                    folder = folder.GetDirectories().FirstOrDefault(dir =>
                        dir.Name.Equals(segment, StringComparison.OrdinalIgnoreCase));
                    if (folder != null)
                    {
                        segment = folder.Name;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    var extension = Path.GetExtension(segment);

                    if (!string.IsNullOrEmpty(extension))
                    {
                        var fileInfo = folder.GetFiles().FirstOrDefault(file =>
                           file.Name.Equals(segment, StringComparison.OrdinalIgnoreCase));
                        if (fileInfo != null)
                        {
                            segment = fileInfo.Name;
                        }
                    }
                    else
                    {
                        var newFolder = folder.GetDirectories().FirstOrDefault(dir =>
                            dir.Name.Equals(segment, StringComparison.OrdinalIgnoreCase));
                        if (newFolder != null)
                        {
                            folder = newFolder;
                            segment = folder.Name;
                        }
                        else
                        {
                            var fileInfo = folder.GetFiles().FirstOrDefault(file =>
                                    file.Name.Equals(segment + ".html", StringComparison.OrdinalIgnoreCase));
                            if (fileInfo != null)
                            {
                                segment = fileInfo.Name;
                            }
                        }
                    }
                }
                segments[i] = segment;
            }
            #endregion
            var reletivePath = string.Join("/", segments);
            root = root.Replace("\\", "/");
            if (!root.EndsWith("/"))
            {
                root += "/";
            }
            if (reletivePath.StartsWith("/"))
            {
                reletivePath = reletivePath.TrimStart('/');
            }
            return root + reletivePath;
        }

        public string CombineRelativePath(string relativePath, string path)
        {
            var slash = GetSlash();
            return relativePath + slash + path;
        }

        public string JoinPath(string[] segments)
        {
            var slash = GetSlash();
            return string.Join(slash, segments);
        }

        public List<string> GetSegments(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return new List<string>();
            }
            input = input.Replace('\\', '/');
            input = input.Trim();
            if (input.StartsWith("/"))
            {
                input = input.Substring(1);
            }
            return input.Split('/').ToList();
        }
        #endregion

        public string GetUpgradeUrl(string convertApiUrl)
        {
            return convertApiUrl + "/_api/converter/LinuxServerPackage";
        }

    }
}
