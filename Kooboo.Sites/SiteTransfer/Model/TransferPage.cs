//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.Lib.Helper;
using Kooboo.Extensions; 

namespace Kooboo.Sites.SiteTransfer
{
    /// <summary>
    /// this is named importpage, but actually can be a pdf or download file. etc. 
    /// </summary>
    public class TransferPage  : Kooboo.Sites.Models.SiteObject
    {

        public TransferPage()
        {
            this.ConstType = ConstObjectType.TransferPage;  
        }
       
        private Guid _id; 
        public override Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    string uniquestring = this.taskid.ToString() + this.absoluteUrl;
                    _id = uniquestring.ToHashGuid();
                }
                return _id;
            }

            set
            {
                _id = value; 
            }
        }

        /// <summary>
        /// The task id that this import page belongs to.
        /// </summary>
        public Guid taskid { get; set; }

        /// <summary>
        /// The page id that this transfer page had been converted to. 
        /// </summary>
        public Guid PageId { get; set; } 

        /// <summary>
       /// The absolute url start with http to download.
       /// </summary>
        public string absoluteUrl {get;set;}

        public Guid HtmlSourceHash { get; set; }

        public int depth;

        public bool done;

        /// <summary>
        /// The start page of a website.
        /// </summary>
        public bool DefaultStartPage;

        public override int GetHashCode()
        {
            string unique = this.taskid.ToString() + this.PageId.ToString() + this.absoluteUrl + this.depth.ToString() + this.done.ToString() + this.DefaultStartPage.ToString();

            return Lib.Security.Hash.ComputeIntCaseSensitive(unique); 
        }

    }
}
