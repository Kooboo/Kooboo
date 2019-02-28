//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.Extensions;
using Kooboo.Sites.Models;
using Kooboo.Sites.Extensions;
using System.Collections.Generic;
using Kooboo.Data.Interface;

namespace Kooboo.Sites.Models
{
    [Kooboo.Attributes.Diskable]
    [Kooboo.Attributes.Routable]
    public class Style : CoreObject, IEmbeddable, IExtensionable
    {
        public Style()
        {
            this.ConstType = ConstObjectType.Style;
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
        /// </summary>
        [Kooboo.Attributes.SummaryIgnore]
        public bool IsEmbedded
        {
            get;
            set;
        }

        private string _extension; 

        [Kooboo.Attributes.SummaryIgnore]
        public string Extension {

            get
            {
                if (string.IsNullOrWhiteSpace(_extension))
                {
                    return "css"; 
                }
                else
                {
                    return _extension; 
                }   
            }
            set
            {
                _extension = value; 
                if(_extension !=null)
                {
                    _extension = _extension.ToLower(); 
                }
            }   
        } 

        private int _bodyhash;

        /// <summary>
        /// The hash code of Css text. 
        /// </summary>
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

        public string DisplayName
        {
            get
            {
                return this.Name;
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
        
        //HTMLStyleElement.media
        //Is a DOMString representing the intended destination medium for style information.
        public string media { get; set; }

        //HTMLStyleElement.type
        //Is a DOMString representing the type of style being applied by this statement.
        public string type { get; set; }
          
        private string _csstext;

        /// <summary>
        /// the embedded css text. 
        /// </summary>
        public string Body
        {
            get
            {
                return _csstext;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(_csstext) && value !=null && _csstext != value)
                {
                    this.SourceChange = true; 
                }     
                _csstext = value;
                if (!string.IsNullOrEmpty(_csstext))
                {
                    _csstext = _csstext.Trim(new[] { '\uFEFF', '\u200B' });
                } 
                this._bodyhash = default(int);
            }
        }


        #region "less, scss, sass"

        public string Source { get; set; }

        public bool SourceChange { get; set; } = false;   

        #endregion


        public string DomTagName
        {
            get
            {
                return "style"; 
            }
        }

        public override int GetHashCode()
        {
            string unique = this.Name;
            unique += this.IsEmbedded.ToString();
            unique += this.Extension + this.Engine + this.Body;
            unique += this.OwnerObjectId.ToString() + this.OwnerConstType.ToString();

            unique += this.KoobooOpenTag + ItemIndex.ToString() + this.media;
            unique += this.type;

            unique += this.Source + this.SourceChange.ToString(); 

            return Lib.Security.Hash.ComputeIntCaseSensitive(unique); 
        }
    }

    public class CssWebSaveFonts
    {
        public static IEnumerable<string> Fonts = new[] { 
            "Georgia","serif","Palatino Linotype","Book Antiqua","Palatino","serif","Times New Roman","Times","serif","Arial","Helvetica","sans-serif","Arial Black","Gadget","sans-serif","Comic Sans MS","cursive","sans-serif","Impact","Charcoal","sans-serif","Lucida Sans Unicode","Lucida Grande","sans-serif","Tahoma","Geneva","sans-serif","Trebuchet MS","Helvetica","sans-serif","Verdana","Geneva","sans-serif","Courier New","Courier","monospace","Lucida Console","Monaco","monospace"
        };
    }
}
