//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Api.ModelBinding
{
    public class BindingResponse
    {
        public bool IsSuccess { get; set; }
        private Dictionary<string, string> _FieldMessage;
        public Dictionary<string, string> FieldMessage
        {
            get
            {
                if (_FieldMessage == null)
                {
                    _FieldMessage = new Dictionary<string, string>();
                }
                return _FieldMessage;
            }
            set
            {
                _FieldMessage = value;
            }
        }

        public object Model;

        public string Message;
    }
}
