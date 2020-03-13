//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Attributes;
using Kooboo.Data.Context;
using Kooboo.Sites.Scripting.Global;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace KScript
{
    public class Request
    {
        private RenderContext context { get; set; }
        public Request(RenderContext context)
        {
            this.context = context;
        }

        [Kooboo.Attributes.SummaryIgnore]
        [Description(@"var value = k.request.get(""key"");
var value = k.request.queryname;")]
        public string Get(string key)
        {
            var value = this.context.Request.GetValue(key, false);
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

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
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

        private KDictionary _query;

        [Description(@"Access to the http query string
var value = k.request.queryString.queryname")]
        public KDictionary queryString
        {
            get
            {
                if (_query == null)
                {
                    _query = new KDictionary();

                    foreach (var item in this.context.Request.QueryString.AllKeys)
                    {
                        _query.Add(item, this.context.Request.QueryString[item]);
                    }
                }
                return _query;
            }
        }

        private KDictionary _form;

        [Description(@"Access to the http form field value
var value = k.request.form.queryname")]
        public KDictionary form
        {
            get
            {
                if (_form == null)
                {
                    _form = new KDictionary();

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

        [Description(@"Form upload file collections
        if (k.request.files.count>0)
           { 
       k.request.files.forEach(function(item)
        { 
         k.response.write(item.fileName); 
         item.save(item.fileName);  
        })  
      }   ")]
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

        [Description("The request text body")]
        public string body
        {
            get
            {
                return this.context.Request.Body;
            } 
        }

        [KIgnore]
        public byte[] postData
        {
            get
            {
                return this.context.Request.PostData;
            }
        }

        [Description("HTTP Method like GET, POST, PUT")]
        public string method
        {
            get { return this.context.Request.Method; }
        }

        [Description("Client Requst IP")]
        public string clientIp
        {
            get { return this.context.Request.IP; }
        }

        private KDictionary _headers;

        public KDictionary headers
        {
            get
            {
                if (_headers == null)
                {
                    _headers = new KDictionary();

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

        [Description("Current Requst URL")]
        public string url
        {
            get { return this.context.Request.Url; }
        }
    }

    [Serializable]
    public class KDictionary : IDictionary<string, string>, System.Collections.IDictionary
    {
        public KDictionary()
        {

        }

        public KDictionary(Dictionary<string, string> value)
        {
            if (value !=null)
            {
                foreach (var item in value)
                {
                    this.data[item.Key] = item.Value; 
                }
            } 
        }

        private Dictionary<string, string> data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        [KIgnore]
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

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
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

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        [KIgnore]
        ICollection IDictionary.Keys { get { return data.Keys; } }

        [KIgnore]
        ICollection IDictionary.Values
        {
            get { return data.Values; }
        }

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public bool IsFixedSize => true;

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public object SyncRoot => throw new NotImplementedException();

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public bool IsSynchronized => throw new NotImplementedException();

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
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

        public void Set(string key, string value)
        {
            this.Add(key, value);
        }

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public void Add(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        [Kooboo.Attributes.SummaryIgnore]
        public void Clear()
        {
            this.data.Clear();
        }

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public bool Contains(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }
        [Kooboo.Attributes.SummaryIgnore]
        public bool ContainsKey(string key)
        {
            return data.ContainsKey(key);
        }

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return data.GetEnumerator();
        }


        [Kooboo.Attributes.SummaryIgnore]
        public bool Remove(string key)
        {
            return data.Remove(key);
        }

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public bool Remove(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public bool TryGetValue(string key, out string value)
        {
            throw new NotImplementedException();
        }

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
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

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return data.GetEnumerator();
        }

        [Kooboo.Attributes.SummaryIgnore]
        public void Remove(object key)
        {
            this.Remove(key.ToString());
        }

        [Kooboo.Attributes.SummaryIgnore]
        [KIgnore]
        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }
    }

}
