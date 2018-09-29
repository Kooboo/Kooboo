//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.Data.Interface;
using Newtonsoft.Json;

namespace Kooboo.Sites.Models
{
    public abstract class SiteObject : ISiteObject
    {
        [Kooboo.Attributes.SummaryIgnore]
        public byte ConstType { get; set; }

        private DateTime _creationdate;   
        [Kooboo.Attributes.SummaryIgnore]
        public DateTime CreationDate
        {
            get
            {
                if (_creationdate == default(DateTime))
                {
                    _creationdate = DateTime.UtcNow; 
                }
                return _creationdate; 
            }
            set
            {
                _creationdate = DateTime.SpecifyKind(value, DateTimeKind.Utc); 
            }
        }

        private DateTime _lastmodify; 
        [Kooboo.Attributes.SummaryIgnore]
        public DateTime LastModified {
            get
            {
                if (_lastmodify == default(DateTime))
                {
                    _lastmodify = DateTime.UtcNow; 
                }
                return _lastmodify; 
            }
            set
            { 
                _lastmodify = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                _lastmodifytick = default(long); 
            }
        }  

        private long _lastmodifytick; 
        [Kooboo.Attributes.SummaryIgnore]
        public long LastModifyTick
        {
            get {
                if (_lastmodifytick == default(long))
                {
                    _lastmodifytick = this.LastModified.Ticks; 
                }
                return _lastmodifytick; 
            }
            set
            {
                _lastmodifytick = value; 
            }
        }
        
        private Guid _id; 
        [Kooboo.Attributes.SummaryIgnore]
        public virtual Guid Id
        {
            get
            {
                if (_id == default(Guid))
                {
                    if (!string.IsNullOrEmpty(this.Name))
                    {
                        _id = Data.IDGenerator.Generate(this.Name, this.ConstType);
                    }
                    else
                    {
                        _id = System.Guid.NewGuid(); 
                    }
                }
                return _id;
            }

            set
            {
                _id = value;
            }
        }

        [JsonProperty("name")]
        public virtual string Name { get; set; }

        public T Clone<T>() where T: SiteObject
        {
            IndexedDB.Serializer.Simple.SimpleConverter<T> converter = new IndexedDB.Serializer.Simple.SimpleConverter<T>();
            var bytes = converter.ToBytes((T)this); 
            return converter.FromBytes(bytes);  
        }
    }

}
