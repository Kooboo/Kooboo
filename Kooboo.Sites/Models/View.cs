//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Interface;
using System;
using System.Collections.Generic;
  
namespace Kooboo.Sites.Models
{  
    [Kooboo.Attributes.Diskable]
    [Kooboo.Attributes.NameAsID]
    public class View : DomObject, IExtensionable, IScriptable
    { 
        public View()
        { 
            ConstType = ConstObjectType.View;
        }
          
        private Guid _id;
        [Kooboo.Attributes.SummaryIgnore]
        public override Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    if (!string.IsNullOrEmpty(this.Name))
                    {
                        string name = this.Name; 
                        if (this.ModuleId != default(Guid))
                        {
                            name += this.ModuleId.ToString(); 
                        }
                        _id = Data.IDGenerator.Generate(name, this.ConstType);
                    }
                }
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        public override string Name
        {
            get
            {
                return base.Name;
            }

            set
            {
                base.Name = value;
                _id = default(Guid);
            }
        }
         
        public Guid ModuleId { get; set; }

        /// <summary>
        /// View Extension,default:.tal
        /// </summary>
        [Kooboo.Attributes.SummaryIgnore]
        public string Extension { get; set; } = "html"; 
         
        public Dictionary<string, string> Parameters { get; set; }
        public List<string> RequestParas { get; set; }

        public override int GetHashCode()
        {
            string unique = this.Body; 
            
            if (Parameters != null)
            {
                foreach (var item in Parameters)
                {
                    unique += item.Key + item.Value; 
                }
            }
            return Lib.Security.Hash.ComputeIntCaseSensitive(unique); 
        } 
  
    }

}