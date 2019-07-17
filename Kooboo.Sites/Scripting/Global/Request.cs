//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Kooboo.Sites.Scripting.Global
{
    public class Request
    {
        private RenderContext context { get; set; }
        public Request(RenderContext context)
        {
            this.context = context;
        }

        [Attributes.SummaryIgnore]
        public string Get(string key)
        {
            var value = this.context.Request.GetValue(key);
            if (value != null)
            {
                return value;
            }
            var dataContextValue = context.DataContext.GetValue(key);
            if (dataContextValue != null)
            {
                return dataContextValue.ToString();
            }
            return null;
        }

        [Attributes.SummaryIgnore]
        public string this[string key]
        {
            get
            {
                return this.Get(key);
            }
            set
            {
                this.queryString.Add(key, value);
            }
        }

        private MyDictionary _query;
        public MyDictionary queryString
        {
            get
            {
                if (_query == null)
                {
                    _query = new MyDictionary();

                    foreach (var item in this.context.Request.QueryString.AllKeys)
                    {
                        _query.Add(item, this.context.Request.QueryString[item]);
                    }
                }
                return _query;
            }
        }

        private MyDictionary _form;
        public MyDictionary form
        {
            get
            {
                if (_form == null)
                {
                    _form = new MyDictionary();

                    foreach (var item in this.context.Request.Forms.AllKeys)
                    {
                        _form.Add(item, this.context.Request.Forms[item]);
                    }
                }

                return _form;
            }
            set
            {
                _form = value;
            }
        }

        private UploadFile[] _files;

        public UploadFile[] files
        {
            get
            {
                if (_files == null)
                {
                    var fileList = new List<UploadFile>();

                    if (this.context.Request.Files != null)
                    {
                        foreach (var item in this.context.Request.Files)
                        {
                            if (!string.IsNullOrWhiteSpace(item.FileName) && item.Bytes != null)
                            { 
                                UploadFile uploadfile = new UploadFile(this.context);
                                uploadfile.FileName = item.FileName;
                                uploadfile.ContentType = item.ContentType;
                                uploadfile.Bytes = item.Bytes;
                                fileList.Add(uploadfile);
                            }
                        }
                    }
                    _files = fileList.ToArray();

                }
                return _files;

            }
        }


        public string body
        {
            get
            {
                return this.context.Request.Body;
            }

        }



        public string method
        {
            get { return this.context.Request.Method; }
        }

        public string clientIp
        {
            get { return this.context.Request.IP; }
        }

        private MyDictionary _headers;

        public MyDictionary headers
        {
            get
            {
                if (_headers == null)
                {
                    _headers = new MyDictionary();

                    foreach (var item in this.context.Request.Headers.AllKeys)
                    {
                        var key = item.Replace("-", "");
                        var value = this.context.Request.Headers[item];

                        _headers.Add(key, value);
                    }
                }

                return _headers;
            }
        }

        public string url
        {
            get { return this.context.Request.Url; }
        }
    }

    public class MyDictionary : IDictionary<string, string>, System.Collections.IDictionary
    {
        private Dictionary<string, string> data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public string this[string key]
        {
            get
            {

                if (data.ContainsKey(key))
                {
                    return data[key];
                }
                return null;

            }
            set
            {
                data[key] = value;
            }
        }

        public ICollection<string> Keys
        {
            get { return data.Keys; }
        }

        public ICollection<string> Values
        {
            get { return data.Values; }
        }

        [Attributes.SummaryIgnore]
        public int Count
        {
            get { return data.Count; }
        }

        public int length
        {
            get
            {
                return this.Count;
            }
        }

        public string Get(string key)
        {
            if (data.ContainsKey(key))
            {
                return data[key];
            }
            return null;
        }

        [Attributes.SummaryIgnore]
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        ICollection IDictionary.Keys { get { return data.Keys; } }

        ICollection IDictionary.Values
        {
            get { return data.Values; }
        }

        [Attributes.SummaryIgnore]
        public bool IsFixedSize => true;

        [Attributes.SummaryIgnore]
        public object SyncRoot => throw new NotImplementedException();

        [Attributes.SummaryIgnore]
        public bool IsSynchronized => throw new NotImplementedException();

        [Attributes.SummaryIgnore]
        public object this[object key]
        {
            get
            {
                string strkey = key.ToString();
                if (this.data.ContainsKey(strkey))
                {
                    return this.data[strkey];
                }
                return null;
            }
            set
            {
                string strkey = key.ToString();
                this.data[strkey] = value.ToString();
            }
        }


        public void Add(string key, string value)
        {
            this.data.Add(key, value);
        }

        [Attributes.SummaryIgnore]
        public void Add(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        [Attributes.SummaryIgnore]
        public void Clear()
        {
            this.data.Clear();
        }

        [Attributes.SummaryIgnore]
        public bool Contains(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }
        [Attributes.SummaryIgnore]
        public bool ContainsKey(string key)
        {
            return data.ContainsKey(key);
        }

        [Attributes.SummaryIgnore]
        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        [Attributes.SummaryIgnore]
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return data.GetEnumerator();
        }


        [Attributes.SummaryIgnore]
        public bool Remove(string key)
        {
            return data.Remove(key);
        }

        [Attributes.SummaryIgnore]
        public bool Remove(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        [Attributes.SummaryIgnore]
        public bool TryGetValue(string key, out string value)
        {
            throw new NotImplementedException();
        }

        [Attributes.SummaryIgnore]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return data.GetEnumerator();
        }

        public bool Contains(object key)
        {
            return this.ContainsKey(key.ToString());
        }


        public void Add(object key, object value)
        {
            Add(key.ToString(), value.ToString());
        }

        [Attributes.SummaryIgnore]
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return data.GetEnumerator();
        }

        [Attributes.SummaryIgnore]
        public void Remove(object key)
        {
            this.Remove(key.ToString());
        }

        [Attributes.SummaryIgnore]
        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }
    }

}
