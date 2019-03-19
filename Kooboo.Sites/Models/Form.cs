//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Data.Interface;

namespace Kooboo.Sites.Models
{
    /// <summary>
    /// The <form></form> object that embedded in the html source of view, layout or page
    /// </summary>
    [Kooboo.Attributes.Diskable(Kooboo.Attributes.DiskType.Text)]
    [Attributes.NameAsID]
    public class Form : CoreObject, IEmbeddable
    {
        public Form()
        {
            this.ConstType = ConstObjectType.Form; 
        }

        private string _body;
        public string Body
        {
            get
            {
                return _body;
            }
            set
            {
                _body = value;
                _bodyhash = default(int);
            }
        }

        public string Engine { get; set; }

        public string Style { get; set; }
  
        private Guid _id;
        public override Guid Id
        {
            get
            { 
                if (_id == default(Guid))
                { 

                    if (this.OwnerObjectId != default(Guid))
                    {
                        _id = Kooboo.Data.IDGenerator.GetFormId(this.OwnerObjectId, this.KoobooId);
                    }
                    else
                    {
                        _id = Kooboo.Data.IDGenerator.Generate(this.Name, this.ConstType);  
                    } 
                }
                return _id;
            }
            set
            {
                _id = value;
            }
        }


        private Dictionary<string, string> _attributes; 
        public Dictionary<string, string> Attributes {
            get {
               if (_attributes == null)
                {
                    _attributes = new Dictionary<string, string>(); 
                }
                return _attributes; 
            }
            set {
                _attributes = value; 
            }
        }
     

        /// <summary>
        /// the Action method, HTTP Post or Http Get. 
        /// </summary>
        public string Method { get; set; }

        public string Fields { get; set; }

        [Obsolete]
        public string RedirectUrl { get; set; }

        [Obsolete]
        public bool AllowAjax { get; set; }

        [Obsolete]
        public string SuccessCallBack { get; set; }

        [Obsolete]
        public string FailedCallBack { get; set; }

        /// <summary>
        /// The Kooboo Id attribute of this Dom Element. 
        /// </summary>
        public string KoobooId { get; set; }

        [Kooboo.Attributes.SummaryIgnore]
        public Guid OwnerObjectId { get; set; }

        public string KoobooOpenTag { get; set; }

        /// <summary>
        /// The constobjecttype, can be like page, layout or view. 
        /// </summary>
        [Kooboo.Attributes.SummaryIgnore]
        public byte OwnerConstType { get; set; }

        // this is used to determine whether form has been updated or not..
        [Kooboo.Attributes.SummaryIgnore]
        public bool IsEmbedded
        {
            get; set;
        }

        private int _bodyhash;
        [Kooboo.Attributes.SummaryIgnore]
        public int BodyHash
        {
            get
            {
                if (_bodyhash == default(int) && !string.IsNullOrEmpty(Body))
                {
                    _bodyhash = Lib.Security.Hash.ComputeIntCaseSensitive(Body);
                }
                return _bodyhash;
            } 
            set
            {
                _bodyhash = value;
            }
        }

        [Kooboo.Attributes.SummaryIgnore]
        public int ItemIndex
        {
            get; set;
        }

        public string DomTagName
        {
            get { return "form"; }
        }

        private Dictionary<string, string> _setting; 
        public Dictionary<string, string> Setting {
            get {
                if (_setting== null)
                {
                    _setting = new Dictionary<string, string>(); 
                }
                return _setting;  
            }
            set
            {
                _setting = value; 
            }
        }

        [Obsolete]
        public string FormSubmitter { get; set; }

        public bool IsSelfRefresh
        {
            get
            {
                return (!string.IsNullOrEmpty(this.RedirectUrl) && this.RedirectUrl.ToLower() == Kooboo.Sites.SiteConstants.SelfRefreshUrl);
            }
        }

        public override int GetHashCode()
        {
            string unique = this.Body + this.FormSubmitter + this.Method + this.RedirectUrl + this.AllowAjax.ToString() + this.FailedCallBack + this.SuccessCallBack + this.IsEmbedded.ToString();

            if (_setting !=null)
            {
                foreach (var item in Setting)
                {
                    unique += item.Key + item.Value;
                }
            }

            if (_attributes !=null)
            {
                foreach (var item in Attributes)
                {
                    unique += item.Key + item.Value;
                }
            } 

            unique += this.Fields + this.Style; 
            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);
        }

        public FormType FormType { get; set; }
    }
     
    public enum FormType
    {
        Normal = 0,
        KoobooForm =1
    }
}
