//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using Kooboo.Data.Interface;
namespace Kooboo.Data.Models
{
    [Kooboo.Attributes.Diskable]
    public class DataMethodSetting : ICoreObject, IDataMethodSetting, ISiteObject
    {
        public DataMethodSetting()
        {
            this.ConstType = ConstObjectType.DataMethodSetting;
        }

        private Guid _id;
        public Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    string uniquestring = this.DeclareType + this.MethodSignatureHash.ToString() + this.MethodName + this.IsGlobal.ToString() + this.IsPublic.ToString();
                    _id = Lib.Security.Hash.ComputeGuidIgnoreCase(uniquestring);
                }
                return _id;
            }
            set
            { _id = value; }
        }

        /// <summary>
        /// By default,this is same as the OriginalMethodName. 
        /// </summary>
        public string MethodName { get; set; }

        public string DisplayName
        {
            get
            {
                //TODO: FIX
                return MethodName;
            }
        }

        /// <summary>
        /// The type that contains this method.
        /// </summary>
        public string DeclareType { get; set; }

        private Guid _DeclareTypeHash;
        public Guid DeclareTypeHash
        {
            get
            {
                if (_DeclareTypeHash == default(Guid) && !string.IsNullOrEmpty(DeclareType))
                {
                    _DeclareTypeHash = Lib.Security.Hash.ComputeGuidIgnoreCase(this.DeclareType);  
                }
                return _DeclareTypeHash;
            }
            set
            { _DeclareTypeHash = value; }
        }

        /// <summary>
        /// To indicate whether this is a third party type or an IDataSource type.
        /// </summary>
        public bool IsThirdPartyType { get; set; }

        public string OriginalMethodName { get; set; }

        public Guid MethodSignatureHash { get; set; }

        public bool IsStatic { get; set; }

        public bool IsVoid { get; set; }

        private Dictionary<string, string> _Parameters;
        public Dictionary<string, string> Parameters
        {
            get
            {
                if (_Parameters == null)
                {
                    _Parameters = new Dictionary<string, string>();
                }
                return _Parameters;
            }
            set
            {
                _Parameters = value;
            }
        }

        private Dictionary<string, ParameterBinding> _ParameterBindings;

        /// <summary>
        /// The flat fields of parameter bindings... 
        /// </summary>
        public Dictionary<string, ParameterBinding> ParameterBinding
        {
            get
            {
                if (_ParameterBindings == null)
                {
                    _ParameterBindings = new Dictionary<string, ParameterBinding>();
                }
                return _ParameterBindings;
            }
            set
            {
                _ParameterBindings = value;
            }
        }
        
        public long Version
        {
            get; set;
        }

        public bool Online { get; set; }

        public byte ConstType
        {
            get; set;
        }

        public DateTime CreationDate
        {
            get; set;
        }

        public DateTime LastModified
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public bool IsGlobal
        {
            get; set;
        }

        public bool IsPost { get; set; }
        public bool IsTask
        {
            get; set;
        }
        public string ReturnType { get; set; }

        public bool IsPagedResult { get; set; }

        public string Description { get; set; }

        public bool IsPublic { get; set; }

        // for kscript code. 
        public Guid CodeId { get; set; }

        public bool IsKScript {
            get 
            {
                return this.CodeId != default(Guid); 
            }
        }

        public override int GetHashCode()
        {
            string unique = this.DeclareType + this.Description + this.DisplayName;
            unique += this.IsGlobal.ToString() + this.IsPost.ToString() + this.IsPublic.ToString() + this.IsStatic.ToString() + this.IsTask.ToString() + this.IsThirdPartyType.ToString() + this.IsVoid.ToString() + this.IsPagedResult.ToString();

            unique += this.MethodName + this.MethodSignatureHash.ToString() + this.Name + this.Online.ToString() + this.OriginalMethodName + this.ReturnType; 

            if (_Parameters !=null)
            {
                foreach (var item in _Parameters)
                {
                    unique += item.Key + item.Value; 
                }
            }

            if (_ParameterBindings !=null)
            {
                foreach (var item in _ParameterBindings)
                {
                    unique += item.Key + item.Value.GetHashCode().ToString(); 
                }
            }

            return Lib.Security.Hash.ComputeIntCaseSensitive(unique); 
        }
    }

    public class ParameterBinding
    { 
        public string Binding
        {
            get;set;
        }

        public string FullTypeName { get; set; }

        public string DisplayName { get; set;  }

        public bool IsCollection { get; set; }

        public bool IsDictionary { get; set; }

        public bool IsClass { get; set; }

        public string ControlType
        {
            get
            {
                if (this.IsClass)
                {
                    return null;
                }
                else if (this.IsDictionary)
                {
                    return "dictionary";
                }
                else if (this.IsContentFilter)
                {
                    return "contentFilter";
                }
                else if (this.IsContentFolder)
                { 
                    return "contentFolder";
                }
                else if (this.IsProductType)
                {
                    return "productType"; 
                }
                else if (this.IsCollection)
                {
                    return "collection";
                } 
                else if (this.IsOrderBy)
                { return "orderBy"; }
                else
                {
                    if (!string.IsNullOrEmpty(this.FullTypeName))
                    {
                        if (this.FullTypeName == typeof(System.Boolean).FullName)
                        {
                            return "checkBox";
                        }
                    }
                    else if (this.IsData)
                    {
                        return "textArea"; 
                    }
                    return "normal";
                }
            }
        }

        public bool IsContentFolder
        {
            get; set;
        }

        public bool IsProductType { get; set; }

        public bool IsData { get; set; }

        public bool IsOrderBy { get; set; }

        /// <summary>
        /// For the content filter...
        /// </summary>
        public bool IsContentFilter
        {
            get
            {
                return !string.IsNullOrEmpty(this.KeyType) && (this.KeyType.Contains("FilterDefinition") || this.KeyType.Contains("Filter"));
            }
        }

        /// <summary>
        /// This is for Collection key Type in case of IList, ICollection Type.. 
        /// Or the Dictionary KeyType. 
        /// </summary>
        public string KeyType { get; set; }

        /// <summary>
        /// In the case of Dictionary, this is the dictionary value type... 
        /// </summary>
        public string ValueType { get; set; }

        /// <summary>
        /// Only valid for Collection/Dictionary, it can be a json binding or field binings like {input.Field}
        /// </summary>
        public bool IsJsonBinding {
            get
            {
                return this.IsCollection || this.IsDictionary || this.IsContentFilter; 
            }
        }

        public override int GetHashCode()
        {
            string unique = this.Binding + this.ControlType + this.DisplayName + this.FullTypeName + this.IsClass.ToString();
            unique += this.IsCollection.ToString() + this.IsContentFilter.ToString() + this.IsContentFolder.ToString() + this.IsProductType.ToString(); 
            unique += this.IsData.ToString() + this.IsDictionary.ToString() + this.IsOrderBy.ToString();
            unique += this.KeyType + this.ValueType;   
            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);  

        }
    }
}
