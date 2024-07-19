//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Lib;
using Kooboo.Render.Response;

namespace Kooboo.Render
{
    public class RenderOption
    {

        public Action<Kooboo.Data.Context.RenderContext, RenderRespnose> Log { get; set; }

        private Func<Kooboo.Data.Context.RenderContext, string> _GetDbPath;
        public Func<Kooboo.Data.Context.RenderContext, string> GetDbPath
        {
            get
            {
                return _GetDbPath == null ? DefaultOptions.DefaultGetDbPath : _GetDbPath;
            }
            set
            {
                _GetDbPath = value;
            }
        }

        private Func<Kooboo.Data.Context.RenderContext, string> _GetRoot;

        public Func<Kooboo.Data.Context.RenderContext, string> GetDiskRoot
        {
            get
            {
                return _GetRoot == null ? DefaultOptions.DefaultGetRoot : _GetRoot;
            }
            set
            {
                _GetRoot = value;
            }
        }

        private Func<Kooboo.Data.Context.RenderContext, RenderOption, bool> _ShouldTryHandle;

        public Func<Kooboo.Data.Context.RenderContext, RenderOption, bool> ShouldTryHandle
        {
            get
            {
                return _ShouldTryHandle == null ? DefaultOptions.DefaultTryShouldHandle : _ShouldTryHandle;
            }
            set
            {
                _ShouldTryHandle = value;
            }
        }

        public Func<Data.Context.RenderContext, string, ResponseBase> Render { get; set; }

        private string _layoutfolder;
        public string LayoutFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_layoutfolder))
                {
                    _layoutfolder = GetAppSetting("LayoutFolder");
                    if (string.IsNullOrWhiteSpace(_layoutfolder))
                    {
                        _layoutfolder = DefaultOptions.DefaultLayoutFolder;
                    }
                }
                return _layoutfolder;
            }
            set
            {
                _layoutfolder = value;
            }
        }
        private string _Extension;
        public string Extension
        {
            get
            {
                if (string.IsNullOrEmpty(_Extension))
                {
                    _Extension = GetAppSetting("Extension");
                    if (string.IsNullOrEmpty(_Extension))
                    {
                        _Extension = DefaultOptions.DefaultExtension;
                    }
                }
                return _Extension;
            }
            set
            {
                _Extension = value;
            }
        }

        private List<string> _Extensions;
        internal List<string> Extensions
        {
            get
            {
                if (_Extensions == null)
                {
                    _Extensions = new List<string>();
                    foreach (var item in Extension.Split(',').ToList())
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            _Extensions.Add(item.Trim());
                        }
                    }
                }
                return _Extensions;
            }
        }

        private string _ViewFolder;
        public string ViewFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_ViewFolder))
                {
                    _ViewFolder = GetAppSetting("ViewFolder");
                    if (string.IsNullOrWhiteSpace(_ViewFolder))
                    {
                        _ViewFolder = DefaultOptions.DefaultViewFolder;
                    }
                }
                return _ViewFolder;
            }
            set
            {
                _ViewFolder = value;
            }
        }

        private List<string> _ViewFolders;
        internal List<string> ViewFolders
        {
            get
            {
                if (_ViewFolders == null)
                {
                    _ViewFolders = new List<string>();
                    foreach (var item in ViewFolder.Split(',').ToList())
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            _ViewFolders.Add(item.Trim());
                        }
                    }
                }
                return _ViewFolders;
            }
        }
        private List<string> _layoutFolders;
        internal List<string> LayoutFolders
        {
            get
            {
                if (_layoutFolders == null)
                {
                    _layoutFolders = new List<string>();
                    foreach (var item in LayoutFolder.Split(',').ToList())
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            _layoutFolders.Add(item.Trim());
                        }
                    }
                }
                return _layoutFolders;
            }
        }

        private string _startPath;
        // the prefix path that should be ignored.  
        public string StartPath
        {
            get
            {
                return _startPath;
            }
            set
            {
                _startPath = value;
                if (!string.IsNullOrEmpty(_startPath) && !_startPath.StartsWith("/"))
                {
                    _startPath = "/" + _startPath;
                }
            }
        }

        private string GetAppSetting(string Name)
        {
            return AppSettingsUtility.Get(Name);
        }

        public bool RequireUser { get; set; }

        public List<string> RequireUserIgnorePath { get; set; } = new List<string>();

        public string LoginPage { get; set; }

        public string PageAfterLogin { get; set; }

        // all content should be rendered within one special, isolated from other websites. 
        public bool RequireSpeicalSite { get; set; }

        public Dictionary<string, object> InitData { get; set; }

        public bool EnableMultilingual { get; set; }

        public bool EnableRenderCache { get; set; }

        public string MultilingualJsFile { get; set; }

        public bool HtmlRaw { get; set; }

    }

    public static class DefaultOptions
    {
        public static string DefaultLayoutFolder = "_layout";

        public static string DefaultViewFolder = "_view";

        public static string DefaultExtension = "html,cshtml";

        public static string DefaultGetRoot(Kooboo.Data.Context.RenderContext request)
        {
            string ExecutingFolder = AppDomain.CurrentDomain.BaseDirectory;
            return System.IO.Path.Combine(ExecutingFolder, @"..\");
        }

        public static string DefaultGetDbPath(Kooboo.Data.Context.RenderContext request)
        {
            string ExecutingFolder = AppDomain.CurrentDomain.BaseDirectory;
            return System.IO.Path.Combine(ExecutingFolder, @"..\_renderdata");
        }

        public static bool DefaultTryShouldHandle(Kooboo.Data.Context.RenderContext context, RenderOption Options)
        {
            if (string.IsNullOrEmpty(Options.StartPath))
            {
                return true;
            }

            string RelativeUrl = context.Request.RawRelativeUrl;

            if (RelativeUrl.ToLower().StartsWith(Options.StartPath.ToLower()))
            {
                return true;
            }
            return false;
        }
    }
}
