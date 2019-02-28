//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Models
{


    [Kooboo.Attributes.Diskable(Kooboo.Attributes.DiskType.Json)]
    public class FormSetting : CoreObject
    {
        public FormSetting()
        {
            this.ConstType = ConstObjectType.FormSetting;
        }
         
        private Guid _id;
        public override Guid Id
        {
            set { _id = value; }
            get
            { 
                if (_id == default(Guid) && this.FormId != default(Guid))
                { 
                     _id = Data.IDGenerator.GetId(ConstObjectType.FormSetting.ToString() + this.FormId.ToString());
                } 
                return _id;
            }
        }

        public Guid FormId { get; set; }

        public bool Enable { get; set; }
         
        /// <summary>
        /// the Action method, HTTP Post or Http Get. 
        /// </summary>
        public string Method { get; set; }

        public string RedirectUrl { get; set; }

        public bool AllowAjax { get; set; }

        public string SuccessCallBack { get; set; }

        public string FailedCallBack { get; set; }

        private Dictionary<string, string> _setting; 

        [Kooboo.IndexedDB.CustomAttributes.KoobooKeyIgnoreCase]
        public Dictionary<string, string> Setting {
            get
            {
                if (_setting == null)
                {
                    _setting = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase); 
                }
                return _setting; 
            }
            set { _setting = value;  } 
        }  

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
            string unique =  this.FormSubmitter + this.Method + this.RedirectUrl + this.AllowAjax.ToString() + this.FailedCallBack + this.SuccessCallBack +this.Enable.ToString();
            foreach (var item in Setting)
            {
                unique += item.Key + item.Value;
            }
            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);
        } 
    }





}
