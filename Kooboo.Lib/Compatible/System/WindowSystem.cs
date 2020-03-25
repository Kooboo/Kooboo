//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

namespace Kooboo.Lib.Compatible
{
    public class WindowSystem : ISystem
    {
        public static List<string> TryPath { get; set; }

        static WindowSystem()
        {
            TryPath = new List<string>
            {
                @"..\..\..\..\Github\Kooboo.Web",
                @"..\..\..\Github\Kooboo.Web",
                @"..\..\..\..\Kooboo.Web",
                @"..\..\..\Kooboo.Web",
                @"..\..\",
                @"..\..\..\",
                @"..\..\..\..\",
                @"..\..\..\..\Kooboo\Kooboo.Web"
            };
        }

        public int GetLastSlash(string path)
        {
            return path.LastIndexOf('\\');
        }

        public string GetSlash()
        {
            return "\\";
        }

        #region
        public string CombinePath(string root, string relativePath)
        {
            relativePath = relativePath.TrimStart('/');

            string fullpath = string.Empty;

            if (!root.EndsWith("\\"))
            {
                root += "\\";
            }
            if (relativePath.StartsWith("\\"))
            {
                relativePath = relativePath.Substring(1);
            }
            if (string.IsNullOrEmpty(relativePath))
            {
                return root;
            }
            var path = relativePath.Replace("/", "\\");
            fullpath = Path.Combine(root, path);

            return fullpath;
        }
        //todo move 
        public string CombineRelativePath(string relativePath, string path)
        {
            var slash = GetSlash();
            return relativePath + slash + path;
        }

        //todo move
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

            input = input.Replace('/', '\\');
            input = input.Trim();
            if (input.StartsWith("\\"))
            {
                input = input.Substring(1);
            }
            return input.Split('\\').ToList();
        }
        #endregion

        #region port
        public int GetPort(int port)
        {
            while (Lib.Helper.NetworkHelper.IsPortInUse(port) && port < 65535)
            {
                port += 1;
            }
            return port;
        }

        public bool IsPortInUsed(int port)
        {
            if (Lib.Helper.NetworkHelper.IsPortInUse(port))
            {
                return true;
            }
            return false;
        }
        #endregion

        public List<string> GetTryPaths() => TryPath;

        public string GetUpgradeUrl(string convertApiUrl)
        {
            return convertApiUrl + "/_api/converter/WindowServerPackage";
        }

    }
}
