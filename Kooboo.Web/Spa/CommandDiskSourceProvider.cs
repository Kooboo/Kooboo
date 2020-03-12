//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Sites.Render.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VirtualFile;

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
                    _startPageNames = new List<string>();
                    _startPageNames.Add("index");
                    _startPageNames.Add("default");
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

        private string FindFile(string FullPath, List<string> extensions)
        {
            if (System.IO.File.Exists(FullPath))
            {
                return FullPath;
            }

            var fileinfo = new System.IO.FileInfo(FullPath);

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
            return Kooboo.Render.Controller.ModuleFile.FindFile(FullPath);
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

        private string FindFileSearch(RenderContext context, string RelativeUrl, List<string> searchfolders, List<string> extensions)
        {
            RelativeUrl = CleanQuestionMark(RelativeUrl);
            string root = GetRoot(context);
            string route = null;
            if (RelativeUrl.StartsWith("/") || RelativeUrl.StartsWith("\\"))
            {
                RelativeUrl = RelativeUrl.Substring(1);
            }
            route = "/" + RelativeUrl;

            string fullpath = RenderHelper.CombinePath(root, route);
            string Filename = FindFile(fullpath, extensions);
            if ((searchfolders == null) || (!string.IsNullOrEmpty(Filename)))
            {
                return Filename;
            }

            return ExtendViewSearch(root, route, searchfolders, extensions);
        }

        public byte[] GetBinary(RenderContext context, string RelativeUrl)
        {
            string FileName = FindFileSearch(context, RelativeUrl, null, null);

            if (!string.IsNullOrEmpty(FileName))
            {
                return GetBinary(context, option, RelativeUrl, FileName);
                //return System.IO.File.ReadAllBytes(FileName);
            }
            return null;
        }

        private string SearchRoute(RenderContext context, string RelativeUrl)
        {
            string root = option.GetDiskRoot(context);
            if (!string.IsNullOrEmpty(option.StartPath))
            {
                root = RenderHelper.CombinePath(root, option.StartPath);
            }

            string result = null;

            bool isDirectory = false;
            if (RelativeUrl.EndsWith("/"))
            {
                isDirectory = true;
            }
            else
            {
                string diskpath = RenderHelper.CombinePath(root, RelativeUrl);
                isDirectory = System.IO.Directory.Exists(diskpath);
            }

            if (isDirectory)
            {
                foreach (var item in this.StartPageNames)
                {
                    string relativeurl = RenderHelper.CombinePath(RelativeUrl, item);
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
                string fullpath = RenderHelper.CombinePath(root, RelativeUrl);

                result = FindFile(fullpath, option.Extensions);

                if (string.IsNullOrWhiteSpace(result))
                {
                    result = ExtendViewSearch(root, RelativeUrl, option.ViewFolders, option.Extensions);
                }
            }

            return result;
        }

        public string GetString(RenderContext context, string RelativeUrl)
        {
            RelativeUrl = CleanQuestionMark(RelativeUrl);

            string fullfilename = SearchRoute(context, RelativeUrl);

            if (!string.IsNullOrEmpty(fullfilename))
            {
                return GetText(context, option, RelativeUrl, fullfilename);
            }
            return null;
        }

        public string GetLayout(RenderContext context, string RelativeUrl)
        {
            string FileName = FindFileSearch(context, RelativeUrl, option.LayoutFolders, null);
            if (!string.IsNullOrEmpty(FileName))
            {
                return GetText(context, option, RelativeUrl, FileName);
                // return System.IO.File.ReadAllText(FileName);
            }
            return null;
        }

        private string CleanQuestionMark(string RelativeUrl)
        {
            if (!string.IsNullOrEmpty(RelativeUrl))
            {
                int markindex = RelativeUrl.IndexOf("?");

                if (markindex > -1)
                {
                    return RelativeUrl.Substring(0, markindex);
                }
            }
            return RelativeUrl;
        }

        private static byte[] GetBinary(RenderContext context, SpaRenderOption option, string RelativeUrl, string FullFileName)
        {
            byte[] result = null;
#if DEBUG
            {
                result = VirtualResources.ReadAllBytes(FullFileName);
            }
#endif
            if (result == null)
            {
                Guid key = Kooboo.Lib.Security.Hash.ComputeGuidIgnoreCase(RelativeUrl);
                result = Kooboo.Data.Cache.RenderCache.GetBinary(key);
                if (result == null)
                {
                    result = VirtualResources.ReadAllBytes(FullFileName);
                    Kooboo.Data.Cache.RenderCache.SetBinary(key, result);
                }
            }
            return result;

        }

        private static string GetText(RenderContext context, SpaRenderOption option, string RelativeUrl, string FullFileName)
        {
            Guid key = Lib.Security.Hash.ComputeGuidIgnoreCase(FullFileName);
            string text = null;
#if DEBUG
            {
                text = VirtualResources.ReadAllText(FullFileName);
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
                        text = VirtualResources.ReadAllText(FullFileName);
                    }
                    htmlbody = Kooboo.Data.Cache.MultiLingualRender.SetGetHtml(context, key, text);
                }
                return htmlbody;
            }
            else
            {
                if (text == null)
                {
                    text = Kooboo.Data.Cache.RenderCache.GetHtml(key);
                    if (text == null)
                    {
                        text = VirtualResources.ReadAllText(FullFileName);
                    }
                    Kooboo.Data.Cache.RenderCache.SetHtml(key, text);
                }
                return text;
            }
        }

        public Stream GetStream(RenderContext context, string RelativeUrl)
        {
            var binary = GetBinary(context, RelativeUrl);
            if (binary != null)
            {
                return new System.IO.MemoryStream(binary);
            }
            return null;
        }
    }



}
