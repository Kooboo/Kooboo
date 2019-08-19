//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using Kooboo.Sites.Extensions;

namespace Kooboo.Sites.Scripting.Global
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

        SiteItem.RoutableTextRepository _page;

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
                            _page = new SiteItem.RoutableTextRepository(this.context.WebSite.SiteDb().Pages, this.context);
                        }
                    }
                }
                return _page;
            }
        }





        SiteItem.TextRepository _views;

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
                            _views = new SiteItem.TextRepository(this.context.WebSite.SiteDb().Views, this.context);
                        }
                    }
                }
                return _views;
            }
        }

        SiteItem.TextRepository _layout;

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
                            _layout = new SiteItem.TextRepository(this.context.WebSite.SiteDb().Layouts, this.context);
                        }
                    }
                }
                return _layout;
            }
        }

        SiteItem.TextContentObjectRepository _textcontents;

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
                            _textcontents = new SiteItem.TextContentObjectRepository(this.context.WebSite.SiteDb().TextContent, this.context);
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
                            _htmlblock = new SiteItem.MultilingualRepository(this.context.WebSite.SiteDb().HtmlBlocks, this.context);
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
                            _labels = new SiteItem.MultilingualRepository(this.context.WebSite.SiteDb().Labels, this.context);
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
                            _script = new SiteItem.RoutableTextRepository(this.context.WebSite.SiteDb().Scripts, this.context);
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
                            _styles = new SiteItem.RoutableTextRepository(this.context.WebSite.SiteDb().Styles, this.context);
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
                            _images = new SiteItem.BinaryRepository(this.context.WebSite.SiteDb().Images, this.context);
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
                            _files = new SiteItem.BinaryRepository(this.context.WebSite.SiteDb().Files, this.context);
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
                return this.context.WebSite != null ? this.context.WebSite.SiteDb().Routes : null;
            }
        }

        SiteItem.FormValuesRepository _formValues;

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
                            _formValues = new SiteItem.FormValuesRepository(this.context.WebSite.SiteDb().FormValues, this.context);
                        }
                    }
                }
                return _formValues;
            }
        }


    }

}
