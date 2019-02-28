//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Data.Context;

namespace Kooboo.Sites.Render.Functions
{
    public class ValueFunction : IFunction
    {
        public ValueFunction()
        {

        }
        public ValueFunction(string value)
        {
            this.Value = value; 
        }
        public string Name
        {
            get
            {
                return "Value"; 
            } 
        }

        public List<IFunction> Parameters
        {  get;set;  }

        private string _value; 
        private string Value {

            get
            {
                return _value;
            }
            set
            {
                _value = value; 
                if (!string.IsNullOrEmpty(_value))
                {
                    _value = _value.Trim(); 
                    if (_value.StartsWith("'") && _value.EndsWith("'"))
                    {
                        _value = _value.Trim('\'');  
                    }
                    else if (_value.StartsWith("\"") && _value.EndsWith("\""))
                    {
                        _value = _value.Trim('"'); 
                    }
                }
            }
        }
           
        public object Render(RenderContext context)
        {
            if (this.Parameters!=null && this.Parameters.Count()>0)
            {

            }
            return Value; 
        }
    }
}
