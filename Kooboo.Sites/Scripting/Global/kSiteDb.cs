//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;

namespace Kooboo.Sites.Scripting.Global
{
    public class kSiteDb
    {
        private RenderContext Context { get; set; }

        public kSiteDb(RenderContext context)
        {
            this.Context = context;
            _locker = new object();
        }

        private static object _locker;

        private SiteItem.RoutableTextRepository _page;

        public SiteItem.RoutableTextRepository Pages
        {
            get
            {
                if (_page == null)
                {
                    lock (_locker)
                    {
                        if (_page == null)
                        {
                            _page = new SiteItem.RoutableTextRepository(this.Context.WebSite.SiteDb().Pages, this.Context);
                        }
                    }
                }
                return _page;
            }
        }

        private SiteItem.TextRepository _views;

        public SiteItem.TextRepository Views
        {
            get
            {
                if (_views == null)
                {
                    lock (_locker)
                    {
                        if (_views == null)
                        {
                            _views = new SiteItem.TextRepository(this.Context.WebSite.SiteDb().Views, this.Context);
                        }
                    }
                }
                return _views;
            }
        }

        private SiteItem.TextRepository _layout;

        public SiteItem.TextRepository Layouts
        {
            get
            {
                if (_layout == null)
                {
                    lock (_locker)
                    {
                        if (_layout == null)
                        {
                            _layout = new SiteItem.TextRepository(this.Context.WebSite.SiteDb().Layouts, this.Context);
                        }
                    }
                }
                return _layout;
            }
        }

        private SiteItem.TextContentObjectRepository _textcontents;

        public SiteItem.TextContentObjectRepository TextContents
        {
            get
            {
                if (_textcontents == null)
                {
                    lock (_locker)
                    {
                        if (_textcontents == null)
                        {
                            _textcontents = new SiteItem.TextContentObjectRepository(this.Context.WebSite.SiteDb().TextContent, this.Context);
                        }
                    }
                }
                return _textcontents;
            }
        }

        public SiteItem.TextContentObjectRepository TextContent
        {
            get
            {
                return this.TextContents;
            }
        }

        private SiteItem.MultilingualRepository _htmlblock;

        public SiteItem.MultilingualRepository HtmlBlocks
        {
            get
            {
                if (_htmlblock == null)
                {
                    lock (_locker)
                    {
                        if (_htmlblock == null)
                        {
                            _htmlblock = new SiteItem.MultilingualRepository(this.Context.WebSite.SiteDb().HtmlBlocks, this.Context);
                        }
                    }
                }
                return _htmlblock;
            }
        }

        private SiteItem.MultilingualRepository _labels;

        public SiteItem.MultilingualRepository Labels
        {
            get
            {
                if (_labels == null)
                {
                    lock (_locker)
                    {
                        if (_labels == null)
                        {
                            _labels = new SiteItem.MultilingualRepository(this.Context.WebSite.SiteDb().Labels, this.Context);
                        }
                    }
                }
                return _labels;
            }
        }

        private SiteItem.RoutableTextRepository _script;

        public SiteItem.RoutableTextRepository Scripts
        {
            get
            {
                if (_script == null)
                {
                    lock (_locker)
                    {
                        if (_script == null)
                        {
                            _script = new SiteItem.RoutableTextRepository(this.Context.WebSite.SiteDb().Scripts, this.Context);
                        }
                    }
                }
                return _script;
            }
        }

        private SiteItem.RoutableTextRepository _styles;

        public SiteItem.RoutableTextRepository Styles
        {
            get
            {
                if (_styles == null)
                {
                    lock (_locker)
                    {
                        if (_styles == null)
                        {
                            _styles = new SiteItem.RoutableTextRepository(this.Context.WebSite.SiteDb().Styles, this.Context);
                        }
                    }
                }
                return _styles;
            }
        }

        private SiteItem.BinaryRepository _images;

        public SiteItem.BinaryRepository Images
        {
            get
            {
                if (_images == null)
                {
                    lock (_locker)
                    {
                        if (_images == null)
                        {
                            _images = new SiteItem.BinaryRepository(this.Context.WebSite.SiteDb().Images, this.Context);
                        }
                    }
                }
                return _images;
            }
        }

        private SiteItem.BinaryRepository _files;

        public SiteItem.BinaryRepository Files
        {
            get
            {
                if (_files == null)
                {
                    lock (_locker)
                    {
                        if (_files == null)
                        {
                            _files = new SiteItem.BinaryRepository(this.Context.WebSite.SiteDb().Files, this.Context);
                        }
                    }
                }
                return _files;
            }
        }

        public Repository.RouteRepository Routes
        {
            get
            {
                return this.Context.WebSite != null ? this.Context.WebSite.SiteDb().Routes : null;
            }
        }

        private SiteItem.FormValuesRepository _formValues;

        public SiteItem.FormValuesRepository FormValues
        {
            get
            {
                if (_formValues == null)
                {
                    lock (_locker)
                    {
                        if (_formValues == null)
                        {
                            _formValues = new SiteItem.FormValuesRepository(this.Context.WebSite.SiteDb().FormValues, this.Context);
                        }
                    }
                }
                return _formValues;
            }
        }
    }
}