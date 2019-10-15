//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Data.Models
{
    public class DataMethodResult
    {
        /// <summary>
        /// Datasource Execution Return Result, can be null.
        /// </summary>
        public object Value { get; set; }

        public bool HasValue => Value != null;

        private string _objectType;

        /// <summary>
        /// The type of object that can be used render engine model binding. like News.Id. News is the ObjectType.
        /// </summary>
        public string ObjectType
        {
            get
            {
                if (string.IsNullOrEmpty(_objectType) && this.HasValue)
                {
                    var valuetype = this.Value.GetType();

                    _objectType = Lib.Reflection.TypeHelper.IsGenericCollection(valuetype) ? Lib.Reflection.TypeHelper.GetEnumberableType(valuetype).Name.ToLower() : valuetype.Name.ToLower();
                }
                return _objectType;
            }
            set => _objectType = value;
        }

        /// <summary>
        /// if value set, user will be redirected to this url after datasource execution.
        /// </summary>
        public string RedirectUrl { get; set; }

        private Dictionary<string, DataMethodResult> _children;

        public bool HasChildren => _children != null;

        public Dictionary<string, DataMethodResult> Children => _children ?? (_children = new Dictionary<string, DataMethodResult>());

        public DataMethodResult()
        {
        }

        public DataMethodResult(object value)
        {
            Value = value;
        }

        public object GetValue(string expression)
        {
            return Value == null ? null : Kooboo.Lib.Reflection.Dynamic.GetObjectMember(Value, expression);
        }
    }
}