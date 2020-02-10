//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Attributes;
using Kooboo.Data.Definition;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Contents.Models
{ 
    public class ContentProperty
    {
        public ContentProperty()
        {
            Editable = false;
            IsSummaryField = false;
        }
          
        public string Name { get; set; }


        private string _DisplayName; 
 
        public string DisplayName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_DisplayName))
                {
                    return Name; 
                } 
                return _DisplayName;
            }
            set { _DisplayName = value;  }
        }
        
        public string ControlType { get; set; }

        public  bool IsMedia()
        {
          if (string.IsNullOrEmpty(this.ControlType))
            {
                return false; 
            }
            return this.ControlType.ToLower().Contains("media"); 
        }
         
        [JsonConverter(typeof(StringEnumConverter))]
        public DataTypes DataType
        {
            get;set;
        }
  
        public bool IsSummaryField { get; set; }
        
        public bool MultipleLanguage { get; set; }

        public bool Editable { get; set; }

        public int Order { get; set; }
        
        public string Tooltip { get; set; }

        public int MaxLength { get; set; }
        
        public string Validations { get; set; }
        
        public bool IsSystemField  {  get ;set;  }

        public bool MultipleValue { get; set; }

        public string selectionOptions { get; set; }

        [KIgnore]
        public override int GetHashCode()
        {
            string unique = this.ControlType + this.DataType.ToString();
            unique += this.DisplayName + this.Editable.ToString() + this.IsSummaryField.ToString() + this.IsSystemField.ToString();
            unique += this.MaxLength.ToString();
            unique += this.MultipleLanguage.ToString() + this.Name;
            unique += this.Order.ToString() + this.Tooltip;
            unique += this.Validations;
            unique += this.MultipleValue.ToString();
            unique += this.selectionOptions;
            unique += this.Editable.ToString(); 
            return Lib.Security.Hash.ComputeIntCaseSensitive(unique); 
        }
    }
     
    public class ContentPropertyEquality : IEqualityComparer<ContentProperty>
    {
        public static ContentPropertyEquality Instance = new ContentPropertyEquality();

        public bool Equals(ContentProperty x, ContentProperty y)
        {
            return x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(ContentProperty obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}
