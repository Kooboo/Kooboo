//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.Data;
using Kooboo.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Extensions;
using Kooboo.Data.Interface;
using Kooboo.Attributes;

namespace Kooboo.Sites.Models
{
    [Kooboo.Attributes.Diskable(Kooboo.Attributes.DiskType.Text)]
    [Kooboo.Attributes.Routable]
    public class Script : CoreObject, IEmbeddable, IExtensionable
    {
        public Script()
        {
            this.ConstType = ConstObjectType.Script;
        }

        private Guid _id;
        public override Guid Id
        {
            set { _id = value; }
            get
            {

                if (_id == default(Guid))
                {
                    if (this.OwnerObjectId != default(Guid))
                    {
                        string unique = this.ConstType.ToString() + this.OwnerObjectId.ToString() + this.ItemIndex.ToString();
                        _id = Kooboo.Data.IDGenerator.GetId(unique);
                    }
                    else
                    {
                        _id = Kooboo.Data.IDGenerator.GetNewGuid();
                    }
                }

                return _id;
            }
        }

        private string _name;
        public override string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                if (!string.IsNullOrEmpty(_name))
                {
                    _name = Lib.Helper.StringHelper.ToValidFileName(_name);
                }

            }
        }

        public string Engine { get; set; }

        /// <summary>
        /// embedded or external reference. 
        /// Depreciated, not valid any more. 
        /// </summary>
        [Kooboo.Attributes.SummaryIgnore]
        public bool IsEmbedded
        {
            get;
            set;
        }


        private string _extension;

        [Kooboo.Attributes.SummaryIgnore]
        public string Extension
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_extension))
                {
                    return "js";
                }
                else
                {
                    return _extension;
                }
            }
            set
            {
                _extension = value;
                if (_extension != null)
                {
                    _extension = _extension.ToLower();
                }
            }
        }


        /// <summary>
        /// For embedded css, the parent object id. this can be view, layout, page or textcontent. 
        /// </summary>
        [Kooboo.Attributes.SummaryIgnore]
        public Guid OwnerObjectId { get; set; }

        /// <summary>
        /// The type of owner object id. 
        /// </summary>
        [Kooboo.Attributes.SummaryIgnore]
        public byte OwnerConstType { get; set; }

        public string KoobooOpenTag { get; set; }

        /// <summary>
        /// the item index of embedded style sheet. 
        /// </summary>
        [Kooboo.Attributes.SummaryIgnore]
        public int ItemIndex { get; set; }

        private int _bodyhash;

        /// <summary>
        /// The hash code of script text. This is used to find similiar script. 
        /// </summary>
        /// 
        [Kooboo.Attributes.SummaryIgnore]
        public int BodyHash
        {
            get
            {
                if (_bodyhash == default(int) && !string.IsNullOrEmpty(this.Body))
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

        public string type { get; set; }

        /// <summary>
        /// Used for src, the script source will be requested async.
        /// W3C standard.
        /// </summary>
        public bool async { get; set; }

        /// <summary>
        /// The script will be exected at the end. 
        /// W3C Standard. 
        /// </summary>
        public bool defer { get; set; }

        public string crossOrigin { get; set; }

        private string _text;
        /// <summary>
        /// the embedded script text. 
        /// </summary>
        public string Body
        {
            get
            { return _text; }
            set
            {
                _text = value;
                this.BodyHash = default(int);
            }
        }


        public string DomTagName
        {
            get
            {
                return "script";
            }
        }

        public override int GetHashCode()
        {
            string unique = this.Name + this.BodyHash.ToString() + defer.ToString() + this.async.ToString();
            unique += this.Extension + this.Engine; 
            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);
        }

    }
}
