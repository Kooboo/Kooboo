//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Globalization;
using Kooboo.Data.Models;
using Kooboo.Sites.Models;
using Kooboo.Sites.Routing;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Sites.Repository;
using Kooboo.Sites.Extensions;
using Kooboo.Data.Context;

namespace Kooboo.Sites.Render
{
    public class FrontContext
    {
        public FrontContext(RenderContext renderContext)
        { 
           this.Items = new Dictionary<string, object>();
            this.RenderContext = renderContext; 
            this.RenderContext.SetItem<FrontContext>(this);
            this.StartTime = DateTime.UtcNow; 
        }

        public FrontContext()
        {
            this.Items = new Dictionary<string, object>();
            this.RenderContext = new RenderContext();
            this.RenderContext.SetItem<FrontContext>(this);
            this.StartTime = DateTime.UtcNow; 
        }
         
        public RenderContext RenderContext
        {
            get;set;
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
                   else  if (RenderContext != null && RenderContext.WebSite != null)
                    {
                        _website = RenderContext.WebSite;
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
                    bool multilingual = false; 
                    if (this.RenderContext != null && this.RenderContext.WebSite !=null && this.RenderContext.WebSite.EnableMultilingual)
                    {
                        multilingual = true; 
                    }
                    this.RenderContext.HeaderBindings = Service.HtmlHeadService.GetHeaderBinding(_page, this.RenderContext.Culture, multilingual); 
                    this.Log.PageName = _page.Name; 
                }
            }
        }

        private List<ViewDataMethod> _ViewDataMethod;

        public List<ViewDataMethod> ViewDataMethods
        {
            get
            {
                if (_ViewDataMethod == null)
                {
                    _ViewDataMethod = new List<ViewDataMethod>();
                }
                return _ViewDataMethod;
            }
            set
            {
                _ViewDataMethod = value;
            }
        }

        private List<View> _views;
        public List<View> Views
        {
            get
            {
                if (_views == null)
                {
                    _views = new List<View>();
                }
                return _views;
            }
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
            get
            {
                if (_log == null)
                {
                    _log = new VisitorLog();
                }
                return _log;
            }

            set
            {
                _log = value;
            }
        }
     
        public void AddLogEntry(string Name, string Value, DateTime StartTime, Int16 StatusCode, string detail = null)
        {
            if (this.RenderContext.Request.Channel == RequestChannel.Default && this.WebSite.EnableVisitorLog)
            {
                this.Log.AddEntry(Name, Value, StartTime, DateTime.UtcNow, StatusCode, detail);
            }
        }
    } 
}
