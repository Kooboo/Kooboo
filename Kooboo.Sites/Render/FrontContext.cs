//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Routing;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Render
{
    public class FrontContext
    {
        public FrontContext()
        {
            Items = new Dictionary<string, object>();
            this.RenderContext.SetItem<FrontContext>(this);
            this.StartTime = DateTime.UtcNow;
        }

        private RenderContext _rendercontext;

        public RenderContext RenderContext
        {
            get
            {
                if (_rendercontext == null)
                {
                    _rendercontext = new RenderContext();
                }
                return _rendercontext;
            }
            set { _rendercontext = value; }
        }

        public DateTime StartTime { get; set; }

        public IDictionary<string, object> Items { get; set; }

        private WebSite _website;

        public WebSite WebSite
        {
            get
            {
                if (_website == null)
                {
                    if (_sitedb != null)
                    {
                        _website = _sitedb.WebSite;
                    }
                    else if (_rendercontext != null && _rendercontext.WebSite != null)
                    {
                        _website = _rendercontext.WebSite;
                    }
                }
                return _website;
            }
            set
            {
                _website = value;
                this.RenderContext.WebSite = _website;
            }
        }

        private SiteDb _sitedb;

        public SiteDb SiteDb
        {
            get
            {
                if (_sitedb == null && WebSite != null)
                {
                    _sitedb = this.WebSite.SiteDb();
                }
                return _sitedb;
            }
            set
            {
                _sitedb = value;
            }
        }

        private Route _route;

        public Route Route
        {
            get
            {
                return _route;
            }
            set
            {
                _route = value;
                if (_route != null && _route.Parameters.Count > 0)
                {
                    this.RenderContext.DataContext.Push(_route.Parameters);
                }
            }
        }

        private Page _page;

        public Page Page
        {
            get { return _page; }
            set
            {
                _page = value;
                if (_page != null)
                {
                    bool multilingual = this.RenderContext != null && this.RenderContext.WebSite != null && this.RenderContext.WebSite.EnableMultilingual;
                    this.RenderContext.HeaderBindings = Service.HtmlHeadService.GetHeaderBinding(_page, this.RenderContext.Culture, multilingual);
                    this.Log.PageName = _page.Name;
                }
            }
        }

        private List<ViewDataMethod> _viewDataMethod;

        public List<ViewDataMethod> ViewDataMethods
        {
            get { return _viewDataMethod ?? (_viewDataMethod = new List<ViewDataMethod>()); }
            set
            {
                _viewDataMethod = value;
            }
        }

        private List<View> _views;

        public List<View> Views
        {
            get { return _views ?? (_views = new List<View>()); }
            set
            {
                _views = value;
            }
        }

        public View ExecutingView { get; set; }

        public HashSet<int> AltervativeViews { get; set; }

        private VisitorLog _log;

        /// <summary>
        /// The log of visitor activities.
        /// </summary>
        public VisitorLog Log
        {
            get { return _log ?? (_log = new VisitorLog()); }

            set
            {
                _log = value;
            }
        }

        public void AddLogEntry(string name, string value, DateTime startTime, Int16 statusCode, string detail = null)
        {
            if (this.RenderContext.Request.Channel == RequestChannel.Default && this.WebSite.EnableVisitorLog)
            {
                this.Log.AddEntry(name, value, startTime, DateTime.UtcNow, statusCode, detail);
            }
        }
    }
}