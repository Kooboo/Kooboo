//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Render.Response;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Render
{
    public class RenderOption
    {
        public Action<Kooboo.Data.Context.RenderContext, RenderRespnose> Log { get; set; }

        private Func<Kooboo.Data.Context.RenderContext, string> _getDbPath;

        public Func<Kooboo.Data.Context.RenderContext, string> GetDbPath
        {
            get
            {
                return _getDbPath ?? DefaultOptions.DefaultGetDbPath;
            }
            set
            {
                _getDbPath = value;
            }
        }

        private Func<Kooboo.Data.Context.RenderContext, string> _getRoot;

        public Func<Kooboo.Data.Context.RenderContext, string> GetDiskRoot
        {
            get
            {
                return _getRoot ?? DefaultOptions.DefaultGetRoot;
            }
            set
            {
                _getRoot = value;
            }
        }

        private Func<Kooboo.Data.Context.RenderContext, RenderOption, bool> _shouldTryHandle;

        public Func<Kooboo.Data.Context.RenderContext, RenderOption, bool> ShouldTryHandle
        {
            get
            {
                return _shouldTryHandle ?? DefaultOptions.DefaultTryShouldHandle;
            }
            set
            {
                _shouldTryHandle = value;
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

        private string _extension;

        public string Extension
        {
            get
            {
                if (string.IsNullOrEmpty(_extension))
                {
                    _extension = GetAppSetting("Extension");
                    if (string.IsNullOrEmpty(_extension))
                    {
                        _extension = DefaultOptions.DefaultExtension;
                    }
                }
                return _extension;
            }
            set
            {
                _extension = value;
            }
        }

        private List<string> _extensions;

        internal List<string> Extensions
        {
            get
            {
                if (_extensions == null)
                {
                    _extensions = new List<string>();
                    foreach (var item in Extension.Split(',').ToList())
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            _extensions.Add(item.Trim());
                        }
                    }
                }
                return _extensions;
            }
        }

        private string _viewFolder;

        public string ViewFolder
        {
            get
            {
                if (string.IsNullOrEmpty(_viewFolder))
                {
                    _viewFolder = GetAppSetting("ViewFolder");
                    if (string.IsNullOrWhiteSpace(_viewFolder))
                    {
                        _viewFolder = DefaultOptions.DefaultViewFolder;
                    }
                }
                return _viewFolder;
            }
            set
            {
                _viewFolder = value;
            }
        }

        private List<string> _viewFolders;

        internal List<string> ViewFolders
        {
            get
            {
                if (_viewFolders == null)
                {
                    _viewFolders = new List<string>();
                    foreach (var item in ViewFolder.Split(',').ToList())
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            _viewFolders.Add(item.Trim());
                        }
                    }
                }
                return _viewFolders;
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

        private string GetAppSetting(string name)
        {
            return System.Configuration.ConfigurationManager.AppSettings.Get(name);
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
    }

    public static class DefaultOptions
    {
        public static string DefaultLayoutFolder = "_layout";

        public static string DefaultViewFolder = "_view";

        public static string DefaultExtension = "html,cshtml";

        public static string DefaultGetRoot(Kooboo.Data.Context.RenderContext request)
        {
            string executingFolder = AppDomain.CurrentDomain.BaseDirectory;
            return System.IO.Path.Combine(executingFolder, @"..\");
        }

        public static string DefaultGetDbPath(Kooboo.Data.Context.RenderContext request)
        {
            string executingFolder = AppDomain.CurrentDomain.BaseDirectory;
            return System.IO.Path.Combine(executingFolder, @"..\_renderdata");
        }

        public static bool DefaultTryShouldHandle(Kooboo.Data.Context.RenderContext context, RenderOption options)
        {
            if (string.IsNullOrEmpty(options.StartPath))
            {
                return true;
            }

            string relativeUrl = context.Request.RawRelativeUrl;

            return relativeUrl.ToLower().StartsWith(options.StartPath.ToLower());
        }
    }
}