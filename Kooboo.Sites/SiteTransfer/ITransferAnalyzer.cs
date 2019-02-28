//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Dom;
using Kooboo.Sites.Repository;
using Kooboo.Sites.SiteTransfer.Download;

namespace Kooboo.Sites.SiteTransfer
{
    public interface ITransferAnalyzer
    {
        void Execute(AnalyzerContext Context);
    }

    public class AnalyzerContext
    {
        public AnalyzerContext()
        {
            this.Changes = new List<AnalyzerUpdate>();
        }

        public Guid ObjectId { get; set; }

        /// <summary>
        /// The site object const type. 
        /// </summary>
        public byte ObjectType { get; set; }

        /// <summary>
        /// The absolute URL of this object. 
        /// </summary>
        public string AbsoluteUrl { get; set; }

        private string _OriginalImportUrl; 

        /// <summary>
        /// This is used to test when the resource links needs to contains domains part or not. 
        /// Example: http://www.kooboo.com/page/a.png, can be relative of /page/a.png or /kooboo.com/page/a.png. 
        /// </summary>
        public string OriginalImportUrl {
            get
            {
                if (string.IsNullOrEmpty(_OriginalImportUrl))
                {
                    _OriginalImportUrl = this.SiteDb.TransferTasks.FirstImportHost();  

                    if (string.IsNullOrEmpty(_OriginalImportUrl))
                    {
                        _OriginalImportUrl = this.AbsoluteUrl;
                    }
                   
                }
                return _OriginalImportUrl; 
            }
            set
            {
                _OriginalImportUrl = value; 
            }
        }

        /// <summary>
        /// Used when importing a page, to determine whether it is same as the original domain or not. 
        /// </summary>
        
        private string _htmlsource;

        /// <summary>
        /// The html source code. 
        /// </summary>
        public string HtmlSource
        {
            get
            {
                return _htmlsource;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (string.IsNullOrEmpty(_htmlsource))
                    {
                        _htmlsource = value;
                        _dom = null; 
                    }
                    else
                    {
                        if (_htmlsource != value)
                        {
                            _htmlsource = value;
                            _dom = null;
                        }
                    }
                }
            }
        }

        public SiteDb SiteDb { get; set; }


        private Document _dom;

        public Document Dom
        {
            get
            {
                if (_dom == null)
                {
                    _dom = Kooboo.Dom.DomParser.CreateDom(this.HtmlSource); 
                }
                return _dom;
            }
            set
            {
                _dom = value; 
            }
        }

        public List<AnalyzerUpdate> Changes { get; set; }

        public DownloadManager DownloadManager { get; set; }
         
    }

    public class AnalyzerUpdate
    {
        public int StartIndex { get; set; }

        public int EndIndex { get; set; }

        public string NewValue { get; set; }

        public string OldValue { get; set; }

        public bool IsReplace { get; set; }
    }
}
