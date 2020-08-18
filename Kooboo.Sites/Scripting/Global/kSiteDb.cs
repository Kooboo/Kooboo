//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Sites.Sync;
using KScript.Sites;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KScript
{
    public class kSiteDb
    {
        private RenderContext context { get; set; }

        private Kooboo.Sites.Repository.SiteDb sitedb { get; set; }

        public kSiteDb(RenderContext context)
        {
            this.context = context;
            this.sitedb = this.context.WebSite.SiteDb();
            _locker = new object();
        }

        public WebSite WebSite
        {

            get
            {
                if (this.context != null && this.context.WebSite != null)
                {
                    return this.context.WebSite;
                }

                if (this.sitedb != null)
                {
                    return this.sitedb.WebSite;
                }

                return null;
            }
        }

        public kSiteDb Get(string SiteName)
        {
            var orgid = this.context.WebSite.OrganizationId;
            var allsites = Kooboo.Data.GlobalDb.WebSites.ListByOrg(orgid);

            if (allsites == null || !allsites.Any())
            {
                return null;
            }

            var find = allsites.Find(o => o.Name == SiteName);
            if (find == null)
            {
                find = allsites.Find(o => o.DisplayName == SiteName);
            }

            if (find == null)
            {
                return null;
            }

            RenderContext newcontext = new RenderContext();
            newcontext.Request = this.context.Request;
            newcontext.User = this.context.User;
            newcontext.WebSite = find;
            newcontext.IsSiteBinding = true;

            return new kSiteDb(newcontext);

        }

        private WebSite _findsite(Guid id)
        {
            Guid orgid = this.context.WebSite.OrganizationId;
            List<WebSite> allsites = Kooboo.Data.GlobalDb.WebSites.ListByOrg(orgid);

            if (allsites == null || !allsites.Any())
            {
                return null;
            }

            return allsites.Find(o => o.Id == id);
        }

        public void DeleteSite(object SiteId)
        { 
            Guid id = Kooboo.Lib.Helper.IDHelper.ParseKey(SiteId);
            WebSite find = _findsite(id);

            if (find != null)
            {
                Kooboo.Sites.Service.WebSiteService.Delete(find.Id);
            }
        }


        public kSiteDb CreatSite(string siteName, string fullDomain, byte[] Binary)
        {
            var orgid = this.context.WebSite.OrganizationId;
            var orgname = siteName;
            bool nameok = false;

            for (int i = 0; i < 99; i++)
            {
                if (Kooboo.Data.GlobalDb.WebSites.CheckNameAvailable(siteName, orgid))
                {
                    nameok = true;
                    break;
                }
                else
                {
                    siteName = orgname + i.ToString();
                }
            }

            if (!nameok)
            {
                throw new Exception(Kooboo.Data.Language.Hardcoded.GetValue("SiteName is taken", this.context));
            }

            MemoryStream memory = new MemoryStream(Binary);
            var newsite = ImportExport.ImportZip(memory, orgid, siteName, fullDomain, orgid);


            RenderContext newcontext = new RenderContext();
            newcontext.Request = this.context.Request;
            newcontext.User = this.context.User;
            newcontext.WebSite = newsite;
            newcontext.IsSiteBinding = true;

            return new kSiteDb(newcontext);

        }


        private object _locker;
        RoutableTextRepository _page;

        public RoutableTextRepository Pages
        {
            get
            {
                if (_page == null)
                {
                    lock (_locker)
                    {
                        if (_page == null)
                        {
                            _page = new RoutableTextRepository(sitedb.Pages, this.context);
                        }
                    }
                }
                return _page;
            }
        }

        TextRepository _views;
        public TextRepository Views
        {
            get
            {
                if (_views == null)
                {
                    lock (_locker)
                    {
                        if (_views == null)
                        {
                            _views = new TextRepository(this.context.WebSite.SiteDb().Views, this.context);
                        }
                    }
                }
                return _views;
            }
        }

        TextRepository _layout;
        public TextRepository Layouts
        {
            get
            {
                if (_layout == null)
                {
                    lock (_locker)
                    {
                        if (_layout == null)
                        {
                            _layout = new TextRepository(this.sitedb.Layouts, this.context);
                        }
                    }
                }
                return _layout;
            }
        }

        TextContentObjectRepository _textcontents;
        public TextContentObjectRepository TextContents
        {
            get
            {
                if (_textcontents == null)
                {
                    lock (_locker)
                    {
                        if (_textcontents == null)
                        {
                            _textcontents = new TextContentObjectRepository(this.sitedb.TextContent, this.context);
                        }
                    }
                }
                return _textcontents;
            }
        }

        [KIgnore]
        public TextContentObjectRepository TextContent
        {
            get
            {
                return this.TextContents;
            }
        }


        private MultilingualRepository _htmlblock;

        public MultilingualRepository HtmlBlocks
        {
            get
            {
                if (_htmlblock == null)
                {
                    lock (_locker)
                    {
                        if (_htmlblock == null)
                        {
                            _htmlblock = new MultilingualRepository(this.sitedb.HtmlBlocks, this.context);
                        }
                    }
                }
                return _htmlblock;
            }
        }

        private MultilingualRepository _labels;

        public MultilingualRepository Labels
        {
            get
            {
                if (_labels == null)
                {
                    lock (_locker)
                    {
                        if (_labels == null)
                        {
                            _labels = new MultilingualRepository(this.sitedb.Labels, this.context);
                        }
                    }
                }
                return _labels;
            }
        }


        private RoutableTextRepository _script;

        public RoutableTextRepository Scripts
        {
            get
            {
                if (_script == null)
                {
                    lock (_locker)
                    {
                        if (_script == null)
                        {
                            _script = new RoutableTextRepository(this.sitedb.Scripts, this.context);
                        }
                    }
                }
                return _script;
            }
        }


        private RoutableTextRepository _styles;

        public RoutableTextRepository Styles
        {
            get
            {
                if (_styles == null)
                {
                    lock (_locker)
                    {
                        if (_styles == null)
                        {
                            _styles = new RoutableTextRepository(this.sitedb.Styles, this.context);
                        }
                    }
                }
                return _styles;
            }
        }


        private BinaryRepository _images;
        public BinaryRepository Images
        {
            get
            {
                if (_images == null)
                {
                    lock (_locker)
                    {
                        if (_images == null)
                        {
                            _images = new BinaryRepository(this.sitedb.Images, this.context);
                        }
                    }
                }
                return _images;
            }
        }

        private BinaryRepository _files;
        public BinaryRepository Files
        {
            get
            {
                if (_files == null)
                {
                    lock (_locker)
                    {
                        if (_files == null)
                        {
                            _files = new BinaryRepository(this.sitedb.Files, this.context);
                        }
                    }
                }
                return _files;
            }
        }

        RouteObjectRepository _routes;
        public KScript.Sites.RouteObjectRepository Routes
        {
            get
            {
                if (_routes == null)
                {
                    lock (_locker)
                    {
                        if (_routes == null)
                        {
                            _routes = new RouteObjectRepository(this.sitedb.Routes, this.context);
                        }
                    }
                }
                return _routes;
            }
        }

        FormValuesRepository _formValues;

        public FormValuesRepository FormValues
        {
            get
            {
                if (_formValues == null)
                {
                    lock (_locker)
                    {
                        if (_formValues == null)
                        {
                            _formValues = new FormValuesRepository(this.sitedb.FormValues, this.context);
                        }
                    }
                }
                return _formValues;
            }
        }
    }

}
