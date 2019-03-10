//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Sites.Models;
 
namespace Kooboo.Sites.SiteTransfer
{ 
    public  class TransferTask : SiteObject
    {
        public TransferTask()
        {
            this.ConstType = ConstObjectType.TransferTask;
        }

        private Guid _id; 
        public override Guid Id {
            get
            {
                if (_id == default(Guid))
                {
                    _id = System.Guid.NewGuid(); 
                }
                return _id; 
            }
            set
            {
                _id = value; 
            }
        }

        private HashSet<string> _domains; 
        public HashSet<string> Domains {
            get
            {
                if (_domains == null)
                {
                    _domains = new HashSet<string>(); 
                }
                return _domains; 
            }
            set { _domains = value;  }
        }

        public EnumTransferTaskType TaskType { get; set; }

        private string _fullstarturl;
        
        public string FullStartUrl
        {
            get
            {
                return _fullstarturl;
            }

            set
            {
                _fullstarturl = value;

                if (!_fullstarturl.ToLower().StartsWith("http://") && !_fullstarturl.ToLower().StartsWith("https://"))
                {
                    _fullstarturl = "http://" + _fullstarturl;
                }
            }
        }
         
        public int Levels { get; set; }

        public bool done { get; set; }

        public int Totalpages { get; set; }

       // only for transfer single page. 
        public string RelativeName { get; set; }

        public Guid UserId { get; set; }

        private Dictionary<string, string> _cookie; 
        public Dictionary<string, string> cookies {
            get
            {
                if (_cookie ==null)
                {
                    _cookie = new Dictionary<string, string>();
                }
                return _cookie; 
            }
            set
            {
                _cookie = value; 
            }
        }

        public override int GetHashCode()
        {
            string unique = this.FullStartUrl + this.Levels.ToString() + this.Totalpages.ToString();
            unique += this.UserId.ToString() + this.RelativeName + this.Levels.ToString();

            unique += this.done.ToString(); 

            foreach (var item in this.Domains)
            {
                unique += item; 
            }
            foreach (var item in this.cookies)
            {
                unique += item.Key + item.Value; 
            }
            return Lib.Security.Hash.ComputeIntCaseSensitive(unique); 
        }
    }
      
}
