//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Extensions;

namespace Kooboo.Sites.Models
{
    public class DomElement : Kooboo.Data.Interface.ISiteObject
    { 
        public DomElement()
        {
            this.ConstType = ConstObjectType.DomElement; 
        }

        private Guid _id;
        public Guid Id
        {
            set { _id = value; }
            get
            {
                if (_id == default(Guid))
                {
                    _id = Kooboo.Data.IDGenerator.GetPageElementId(this.OwnerObjectId, this.OwnerObjectType, this.KoobooId);
                }
                return _id;
            }
        }

        public Guid ParentId
        {
            get;
            set;
        }

        /// <summary>
        /// TagName or Node Name. 
        /// </summary>
        public string Name { get; set; }

        public int Depth { get; set; }
        public int Sibling { get; set; }

        public int OpenTagStartIndex { get; set; }
        public int EndTagEndIndex { get; set; }

        private Dictionary<string, string> _nodeattributes;

        /// <summary>
        /// used for display. 
        /// </summary>
        public Dictionary<string, string> NodeAttributes
        {
            get
            {
                if (_nodeattributes == null)
                {
                    _nodeattributes = new Dictionary<string, string>();
                }
                return _nodeattributes;
            }
            set
            {
                _nodeattributes = value;
            }
        }

        private Guid _nodeattributehash;
        public Guid NodeAttributeHash
        {
            get
            {
                if (_nodeattributehash == default(Guid))
                {
                    _nodeattributehash = NodeAttributeString.ToHashGuid();
                }
                return _nodeattributehash;
            }
        }

        private string _NodeAttributeString;
        public string NodeAttributeString
        {
            get
            {
                if (string.IsNullOrEmpty(_NodeAttributeString))
                {
                    _NodeAttributeString = string.Empty;
                    foreach (var item in NodeAttributes)
                    {
                        _NodeAttributeString += item.Key + "=" + item.Value;
                    }
                }
                return _NodeAttributeString;
            }
        }

        private Guid _parentPathHash;
        public Guid ParentPathHash
        {
            get
            {
                if (_parentPathHash == default(Guid) && !string.IsNullOrEmpty(this.ParentPath))
                {
                    _parentPathHash = this.ParentPath.ToHashGuid();
                }
                return _parentPathHash;
            }
            set
            {
                _parentPathHash = value;
            }
        }

        private string _ParentPath;
         /// <summary>
        /// example: body/div/p/span
        /// The path from body to this element. 
        /// </summary>
        public string ParentPath
        {
            get
            {
                return _ParentPath;
            }
            set
            {
                _ParentPath = value;
                this.ParentPathHash = _ParentPath.ToHashGuid();
            }
        }
          
        private string _subElementString;

        public string SubElementString
        {
            get
            {
                //if (_subElementString == null && _subelements !=null)
                //{
                //     _subElementString = string.Empty;
                //    foreach (var item in this.SubElements.OrderBy(o => o.Key))
                //    {
                //        _subElementString += item.Key + item.Value;
                //    }
                //    _subElementString = _subElementString.Replace(" ", "");
                //    _subElementString = _subElementString.Replace(Environment.NewLine, ""); 
                //}
                        
                return _subElementString; 
            }
            set
            {
                _subElementString = value; 
            }
        }

        private Guid _SubElementHash;
        public Guid SubElementHash
        {
            get
            {
                if (_SubElementHash == default(Guid) && !string.IsNullOrEmpty(_subElementString))
                { 
                    _SubElementHash = this._subElementString.ToHashGuid(); 
                }
                return _SubElementHash;
            } 
        }

        private string _koobooid;

        /// <summary>
        /// The Kooboo id of this elements, can be used to retrieve this element back. 
        /// </summary>
        public string KoobooId
        {
            get
            { return _koobooid; }
            set
            {
                _koobooid = value;
                this.KoobooIdHash = _koobooid.ToHashGuid();
            }
        }

        private Guid _koobooIdHash;

        public Guid KoobooIdHash
        {
            get
            {
                if (_koobooIdHash == default(Guid) && !string.IsNullOrEmpty(this.KoobooId))
                {
                    _koobooIdHash = this.KoobooId.ToHashGuid();
                }
                return _koobooIdHash;
            }
            set
            {
                _koobooIdHash = value;
            }

        }

     
        public Guid InnerHtmlHash
        {
            get;
            set;
        }

        public Guid OwnerObjectId { get; set; }

        public byte OwnerObjectType { get; set; }
          
        public byte ConstType { get; set; } = ConstObjectType.DomElement;

        [Kooboo.IndexedDB.CustomAttributes.KoobooIgnore]
        public DateTime CreationDate
        {
            get;set;
        }

        [Kooboo.IndexedDB.CustomAttributes.KoobooIgnore]
        public DateTime LastModified
        {
            get;set;
        }
    }
     
}
