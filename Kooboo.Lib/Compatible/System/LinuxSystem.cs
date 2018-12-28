using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.IO;

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

        public int GetEndLine(string source, int index)
        {
            //linux source.IndexOf("\n", index) will be -1;
            //only source.IndexOf("\r\n", index) can get the index
            return source.IndexOf("\r\n", index) + 1;
        }

        #region path
        public string CombinePath(string root, string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return root;

            relativePath = relativePath.TrimStart('/');
            //linux file is case sensitive
            var fullpath = GetCaseInsensitiveFile(root, relativePath);
            return fullpath;
        }

        public string CombineRelativePath(string relativePath, string path)
        {
            var slash = GetSlash();
            return relativePath + slash + path;
        }

        public string GetCaseInsensitiveFile(string root, string relativePath)
        {
            var folder = new DirectoryInfo(root);
            if (relativePath.IndexOf(root,StringComparison.OrdinalIgnoreCase) == 0)
            {
                relativePath = relativePath.ToLower().Replace(root.ToLower(), "");
            }
            relativePath = relativePath.Replace("\\", "/");

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
                                    file.Name.Equals(segment+".html", StringComparison.OrdinalIgnoreCase));
                            if (fileInfo != null)
                            {
                                segment = fileInfo.Name;
                            }
                        }
                    }
                }
                segments[i] = segment;
                //if (i == segments.Length - 1)
                //{
                //    var files = folder.GetFiles().Where(file =>
                //            file.Name.Equals(segment, StringComparison.OrdinalIgnoreCase));
                //    if (files == null || files.Count() == 0)
                //    {
                //        files = folder.GetFiles().Where(file =>
                //           file.Name.Split('.')[0].Equals(segment, StringComparison.OrdinalIgnoreCase));
                //    }

                //    if (files != null && files.Count() > 0)
                //    {
                //        //default get html
                //        var fileinfo = files.FirstOrDefault(file =>
                //          file.Name.IndexOf(".html", StringComparison.OrdinalIgnoreCase) > -1);
                //        if (fileinfo != null)
                //        {
                //            segments[i] = fileinfo.Name;
                //        }
                //        else
                //        {
                //            segments[i] = files.ToList()[0].Name;
                //        }
                //    }
                //    else
                //    {
                //        folder = folder.GetDirectories().FirstOrDefault(dir =>
                //        dir.Name.Equals(segment, StringComparison.OrdinalIgnoreCase));
                //        if (folder != null)
                //        {
                //            segments[i] = folder.Name;
                //        }
                //    }

                //}
                //else
                //{
                //    folder = folder.GetDirectories().FirstOrDefault(dir =>
                //        dir.Name.Equals(segment, StringComparison.OrdinalIgnoreCase));
                //    if (folder != null)
                //    {
                //        segments[i] = folder.Name;
                //    }
                //    else
                //    {
                //        break;
                //    }

                //}
            }

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
            string path = root + reletivePath;
            return path;
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

        #region port
        public int GetPort(int port)
        {
            return port;
        }

        public bool IsPortInUsed(int port)
        {
            //when linux or macos port is closed,it still have active tcp.
            //so only bind to the defaultPort.
            //NetworkHelper.IsPortInUse will get wrong result
            return false;
        }
        #endregion

        public List<string> GetTryPaths()
        {
            List<string> trypaths = new List<string>();
            trypaths.Add(@"../../../Github/Kooboo.Web");
            trypaths.Add(@"../");
            trypaths.Add(@"../../");
            trypaths.Add(@"../../../");
            return trypaths;
        }

        public string GetUpgradeUrl(string convertApiUrl)
        {
            return convertApiUrl + "/_api/converter/LinuxServerPackage";
        }

    }
}
