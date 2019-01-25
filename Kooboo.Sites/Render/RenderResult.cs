//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Sites.Render
{
    public class RenderResult
    {
        // this is to make it possible for header binding that bind later at the end...
        public Func<string> RenderString { get; set; }

        private string _value; 
        public string Value {
            get {
                if (_value == null)
                {
                    if (this.RenderString != null)
                    {
                        _value = this.RenderString(); 
                    } 
                }
                return _value; 
            }
            set { _value = value;  }
        }

        public bool ClearBefore { get; set; }

        public byte[] Bytes { get; set; }

        private int _len;
        public int Len
        {
           get
            {
                if (_len == 0)
                {
                    if (Bytes != null)
                    {
                        _len = Bytes.Length;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(Value) && this.RenderString != null)
                        {
                            Value = RenderString();
                        }
                        if (Value == null)
                        {
                            _len = 0;
                        }
                        else
                        {
                            this.Bytes = System.Text.Encoding.UTF8.GetBytes(Value);
                            _len = this.Bytes.Length;
                        }
                    }

                }
                return _len;
            }
        }
    }
}
