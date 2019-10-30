//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Definition;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Kooboo.Sites.Ecommerce.Models
{
    public class ProductProperty
    {
        public ProductProperty()
        {
            Editable = false;
            IsSpecification = false;
        }

        public string Name { get; set; }

        private string _displayName;

        public string DisplayName
        {
            get
            {
                return string.IsNullOrWhiteSpace(_displayName) ? Name : _displayName;
            }
            set { _displayName = value; }
        }

        public string ControlType { get; set; }

        public bool IsMedia()
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
            get; set;
        }

        public bool IsSpecification { get; set; }

        public bool MultipleLanguage { get; set; }

        public bool Editable { get; set; }

        public int Order { get; set; }

        public string Tooltip { get; set; }

        public int MaxLength { get; set; }

        public string Validations { get; set; }

        public bool IsSystemField { get; set; }

        public bool MultipleValue { get; set; }

        public string selectionOptions { get; set; }

        public override int GetHashCode()
        {
            string unique = this.ControlType + this.DataType.ToString();
            unique += this.DisplayName + this.Editable.ToString() + this.IsSpecification.ToString() + this.IsSystemField.ToString();
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
}