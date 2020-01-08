//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;
using KScript.Sites;

namespace KScript
{
    public class kSiteDb
    {
        private RenderContext context { get; set; }

        public kSiteDb(RenderContext context)
        {
            this.context = context;
            _locker = new object();
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
                            _page = new RoutableTextRepository(this.context.WebSite.SiteDb().Pages, this.context);
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
                            _layout = new TextRepository(this.context.WebSite.SiteDb().Layouts, this.context);
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
                            _textcontents = new TextContentObjectRepository(this.context.WebSite.SiteDb().TextContent, this.context);
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
                            _htmlblock = new MultilingualRepository(this.context.WebSite.SiteDb().HtmlBlocks, this.context);
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
                            _labels = new MultilingualRepository(this.context.WebSite.SiteDb().Labels, this.context);
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
                            _script = new RoutableTextRepository(this.context.WebSite.SiteDb().Scripts, this.context);
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
                            _styles = new RoutableTextRepository(this.context.WebSite.SiteDb().Styles, this.context);
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
                            _images = new BinaryRepository(this.context.WebSite.SiteDb().Images, this.context);
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
                            _files = new BinaryRepository(this.context.WebSite.SiteDb().Files, this.context);
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
                            _routes = new RouteObjectRepository(this.context.WebSite.SiteDb().Routes, this.context);
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
                            _formValues = new FormValuesRepository(this.context.WebSite.SiteDb().FormValues, this.context);
                        }
                    }
                }
                return _formValues;
            }
        }
    }

}
