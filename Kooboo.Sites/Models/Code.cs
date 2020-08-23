//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.Data.Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace Kooboo.Sites.Models
{
    [Kooboo.Attributes.Diskable(Kooboo.Attributes.DiskType.Text)] 
    [Attributes.NameAsID] 
    public class Code : CoreObject, ITextObject, IEmbeddable, IExtensionable
    {
        public Code()
        {
            this.ConstType = ConstObjectType.Code;
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
                        _id = Kooboo.Data.IDGenerator.Generate(this.Name, this.ConstType);
                    }
                }
                return _id;
            }
        }

        [Kooboo.Attributes.SummaryIgnore]
        public string Extension { get; set; } = ".js";
  
        private string _body; 

        public string Body {
            get
            {
                return _body; 
            }
            set
            {
                _body = value;
                this._bodyhash = default(int); 
            }
        }

        private int _bodyhash;
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

         
        // the kscript that will generate the configuration, that is an list with name, controltype, values. 
        public string Config { get; set; }

        // only for front event. 
        [JsonConverter(typeof(StringEnumConverter))]
        public  FrontEvent.enumEventType EventType { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public CodeType CodeType { get; set; }
          
        public bool IsJson { get; set; }
         
        public bool Cors { get; set; }

        public Guid OwnerObjectId { get; set; }
        public byte OwnerConstType { get; set; }
        public bool IsEmbedded { get; set; }
       
        public string KoobooOpenTag { get; set; }
        public string Engine { get; set; }
        public int ItemIndex { get; set; }

        public string DomTagName { get { return "script";  } }

        public List<string> Parameters { get; set; }

        public override int GetHashCode()
        {
            string unique = this.Body + this.Config + this.EventType.ToString() + this.CodeType.ToString();

            unique += this.ItemIndex.ToString() + this.OwnerConstType.ToString() + this.OwnerObjectId.ToString() + this.KoobooOpenTag; 

            unique += this.IsJson.ToString() + this.Cors.ToString();

            return Lib.Security.Hash.ComputeIntCaseSensitive(unique);
        }

    } 
    public enum CodeType
    { 
        Event=0, 
        Datasource = 2, 
        Job = 3,
        Api =4, 
        PageScript =5, 
        Diagnosis =6, 
        PaymentCallBack = 7,
    } 

}
