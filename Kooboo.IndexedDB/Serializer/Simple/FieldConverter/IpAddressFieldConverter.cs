//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.IndexedDB.Helper;
using System;
using System.Net;

namespace Kooboo.IndexedDB.Serializer.Simple.FieldConverter
{
    public class IpAddressFieldConverter<T> : IFieldConverter<T>
    {
        private Func<T, IPAddress> getValue;
        private Action<T, IPAddress> setValue;

        public IpAddressFieldConverter(string fieldName)
        {
            this.getValue = ObjectHelper.GetGetValue<T, IPAddress>(fieldName);
            this.setValue = ObjectHelper.GetSetValue<T, IPAddress>(fieldName);
            this.FieldNameHash = ObjectHelper.GetHashCode(fieldName);
        }

        public int ByteLength
        {
            get
            {
                return 0;
            }
        }

        public int FieldNameHash
        {
            get; set;
        }

        public void SetByteValues(T value, byte[] bytes)
        {
            if (bytes != null && bytes.Length > 0)
            {
                IPAddress address = new IPAddress(bytes);
                this.setValue(value, address);
            }
        }

        public byte[] ToBytes(T value)
        {
            IPAddress fieldvalue = this.getValue(value);
            return fieldvalue?.GetAddressBytes();
        }
    }

    public class IpAddressFieldConverter : IFieldConverter
    {
        private Func<object, IPAddress> getValue;
        private Action<object, IPAddress> setValue;

        public IpAddressFieldConverter(string fieldName, Type objectType)
        {
            this.getValue = ObjectHelper.GetGetFieldValue<IPAddress>(fieldName, objectType);
            this.setValue = ObjectHelper.GetSetFieldValue<IPAddress>(fieldName, objectType);
            this.FieldNameHash = ObjectHelper.GetHashCode(fieldName);
        }

        public int ByteLength
        {
            get
            {
                return 0;
            }
        }

        public int FieldNameHash
        {
            get; set;
        }

        public void SetByteValues(object value, byte[] bytes)
        {
            if (bytes != null && bytes.Length > 0)
            {
                IPAddress bytevalue = new System.Net.IPAddress(bytes);
                this.setValue(value, bytevalue);
            }
        }

        public byte[] ToBytes(object value)
        {
            IPAddress fieldvalue = this.getValue(value);
            return fieldvalue?.GetAddressBytes();
        }
    }
}