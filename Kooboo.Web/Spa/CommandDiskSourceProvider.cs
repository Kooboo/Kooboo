//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Sites.Render.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kooboo.Web.Spa
{
    public class CommandDiskSourceProvider : ICommandSourceProvider
    {
        public CommandDiskSourceProvider()
        { }

        public CommandDiskSourceProvider(SpaRenderOption option)
        {
            this.option = option;
        }

        public SpaRenderOption option { get; set; }

        private List<string> _startPageNames;

        private List<string> StartPageNames
        {
            get
            {
                if (_startPageNames == null)
                {
                    _startPageNames = new List<string> {"index", "default"};
                }
                return _startPageNames;
            }
        }

        private string GetRoot(RenderContext context)
        {
            if (option != null)
            {
                string root = this.option.GetDiskRoot(context);
                if (!string.IsNullOrEmpty(option.StartPath))
                {
                    root = RenderHelper.CombinePath(root, option.StartPath);
                }
                return root;
            }

            return null;
        }

        private string FindFile(string fullPath, List<string> extensions)
        {
            if (System.IO.File.Exists(fullPath))
            {
                return fullPath;
            }

            var fileinfo = new System.IO.FileInfo(fullPath);

            if (fileinfo.Directory.Exists)
            {
                var dir = fileinfo.Directory;
                if (!string.IsNullOrEmpty(fileinfo.Name))
                {
                    if (extensions != null)
                    {
                        foreach (var extension in extensions)
                        {
                            var files = dir.GetFiles(fileinfo.Name + "." + extension.ToLower(), SearchOption.TopDirectoryOnly);
                            if (files != null && files.Any())
                            {
                                return files[0].FullName;
                            }
                        }
                    }

                    var files2 = dir.GetFiles(fileinfo.Name + ".*", SearchOption.TopDirectoryOnly);
                    if (files2 != null && files2.Any())
                    {
                        return files2[0].FullName;
                    }
                }
            }
            return Kooboo.Render.Controller.ModuleFile.FindFile(fullPath);
        }

        private string ExtendViewSearch(string root, string relative, List<string> searchfolders, List<string> extensions)
        {
            foreach (var item in searchfolders)
            {
                string viewrelative = "/" + item + relative;
                viewrelative = RenderHelper.CombinePath(root, viewrelative);
                var result = FindFile(viewrelative, extensions);
                if (!string.IsNullOrEmpty(result))
                {
                    return result;
                }
            }
            return null;
        }

        private string FindFileSearch(RenderContext context, string relativeUrl, List<string> searchfolders, List<string> extensions)
        {
            relativeUrl = CleanQuestionMark(relativeUrl);
            string root = GetRoot(context);
            string route = null;
            if (relativeUrl.StartsWith("/") || relativeUrl.StartsWith("\\"))
            {
                relativeUrl = relativeUrl.Substring(1);
            }
            route = "/" + relativeUrl;

            string fullpath = RenderHelper.CombinePath(root, route);
            string filename = FindFile(fullpath, extensions);
            if ((searchfolders == null) || (!string.IsNullOrEmpty(filename)))
            {
                return filename;
            }

            return ExtendViewSearch(root, route, searchfolders, extensions);
        }

        public byte[] GetBinary(RenderContext context, string relativeUrl)
        {
            string fileName = FindFileSearch(context, relativeUrl, null, null);

            return !string.IsNullOrEmpty(fileName) ? GetBinary(context, option, relativeUrl, fileName) : null;
        }

        private string SearchRoute(RenderContext context, string relativeUrl)
        {
            string root = option.GetDiskRoot(context);
            if (!string.IsNullOrEmpty(option.StartPath))
            {
                root = RenderHelper.CombinePath(root, option.StartPath);
            }

            string result = null;

            bool isDirectory = false;
            if (relativeUrl.EndsWith("/"))
            {
                isDirectory = true;
            }
            else
            {
                string diskpath = RenderHelper.CombinePath(root, relativeUrl);
                isDirectory = System.IO.Directory.Exists(diskpath);
            }

            if (isDirectory)
            {
                foreach (var item in this.StartPageNames)
                {
                    string relativeurl = RenderHelper.CombinePath(relativeUrl, item);
                    string fullpath = RenderHelper.CombinePath(root, relativeurl);

                    result = FindFile(fullpath, option.Extensions);

                    if (string.IsNullOrWhiteSpace(result))
                    {
                        result = ExtendViewSearch(root, relativeurl, option.ViewFolders, option.Extensions);
                    }
                    if (!string.IsNullOrEmpty(result))
                    { return result; }
                }
            }
            else
            {
                string fullpath = RenderHelper.CombinePath(root, relativeUrl);

                result = FindFile(fullpath, option.Extensions);

                if (string.IsNullOrWhiteSpace(result))
                {
                    result = ExtendViewSearch(root, relativeUrl, option.ViewFolders, option.Extensions);
                }
            }

            return result;
        }

        public string GetString(RenderContext context, string relativeUrl)
        {
            relativeUrl = CleanQuestionMark(relativeUrl);

            string fullfilename = SearchRoute(context, relativeUrl);

            if (!string.IsNullOrEmpty(fullfilename))
            {
                return GetText(context, option, relativeUrl, fullfilename);
            }
            return null;
        }

        public string GetLayout(RenderContext context, string relativeUrl)
        {
            string fileName = FindFileSearch(context, relativeUrl, option.LayoutFolders, null);
            return !string.IsNullOrEmpty(fileName) ? GetText(context, option, relativeUrl, fileName) : null;
        }

        private string CleanQuestionMark(string relativeUrl)
        {
            if (!string.IsNullOrEmpty(relativeUrl))
            {
                int markindex = relativeUrl.IndexOf("?");

                if (markindex > -1)
                {
                    return relativeUrl.Substring(0, markindex);
                }
            }
            return relativeUrl;
        }

        private static byte[] GetBinary(RenderContext context, SpaRenderOption option, string relativeUrl, string fullFileName)
        {
            byte[] result = null;
#if DEBUG
            {
                result = System.IO.File.ReadAllBytes(fullFileName);
            }
#endif
            if (result == null)
            {
                Guid key = Kooboo.Lib.Security.Hash.ComputeGuidIgnoreCase(relativeUrl);
                result = Kooboo.Data.Cache.RenderCache.GetBinary(key);
                if (result == null)
                {
                    result = System.IO.File.ReadAllBytes(fullFileName);
                    Kooboo.Data.Cache.RenderCache.SetBinary(key, result);
                }
            }
            return result;
        }

        private static string GetText(RenderContext context, SpaRenderOption option, string relativeUrl, string fullFileName)
        {
            Guid key = Lib.Security.Hash.ComputeGuidIgnoreCase(fullFileName);
            string text = null;
#if DEBUG
            {
                text = System.IO.File.ReadAllText(fullFileName);
                key = Lib.Security.Hash.ComputeGuidIgnoreCase(text);
            }
#endif

            if (option.EnableMultilingual)
            {
                string htmlbody = Kooboo.Data.Cache.MultiLingualRender.GetHtml(context, key);
                if (htmlbody == null)
                {
                    if (text == null)
                    {
                        text = File.ReadAllText(fullFileName);
                    }
                    htmlbody = Kooboo.Data.Cache.MultiLingualRender.SetGetHtml(context, key, text);
                }
                return htmlbody;
            }
            else
            {
                if (text == null)
                {
                    text = Kooboo.Data.Cache.RenderCache.GetHtml(key) ?? File.ReadAllText(fullFileName);
                    Kooboo.Data.Cache.RenderCache.SetHtml(key, text);
                }
                return text;
            }
        }

        public Stream GetStream(RenderContext context, string relativeUrl)
        {
            var binary = GetBinary(context, relativeUrl);
            if (binary != null)
            {
                return new System.IO.MemoryStream(binary);
            }
            return null;
        }
    }
}