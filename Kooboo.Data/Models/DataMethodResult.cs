//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Models
{  
    public class DataMethodResult
    {
        /// <summary>
        /// Datasource Execution Return Result, can be null.
        /// </summary>
        public object Value { get; set; }

        public bool HasValue
        {
            get { return Value != null; }
        }

        private string _ObjectType;

        /// <summary>
        /// The type of object that can be used render engine model binding. like News.Id. News is the ObjectType.
        /// </summary>
        public string ObjectType
        {
            get
            {
                if (string.IsNullOrEmpty(_ObjectType) && this.HasValue)
                {
                    var valuetype = this.Value.GetType();

                    if (Lib.Reflection.TypeHelper.IsGenericCollection(valuetype))
                    {

                        _ObjectType = Lib.Reflection.TypeHelper.GetEnumberableType(valuetype).Name.ToLower();
                    }
                    else
                    {
                        _ObjectType = valuetype.Name.ToLower();
                    }
                }
                return _ObjectType;
            }
            set
            {
                _ObjectType = value;
            }

        }

        /// <summary>
        /// if value set, user will be redirected to this url after datasource execution.
        /// </summary>
        public string RedirectUrl { get; set; }

        private Dictionary<string, DataMethodResult> _children;

        public bool HasChildren
        {
            get
            { return _children != null; }
        }

        public Dictionary<string, DataMethodResult> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new Dictionary<string, DataMethodResult>();
                }
                return _children;
            }
        }

        public DataMethodResult() { }

        public DataMethodResult(object value)
        {
            Value = value;
        } 

        public object GetValue(string expression)
        {
            if (Value == null)
            {
                return null;
            }

            return Kooboo.Lib.Reflection.Dynamic.GetObjectMember(Value, expression);
        }
          
    } 
    
}
