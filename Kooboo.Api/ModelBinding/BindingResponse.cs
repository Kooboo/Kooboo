//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.Api.ModelBinding
{
    public class BindingResponse
    {
        public bool IsSuccess { get; set; }
        private Dictionary<string, string> _fieldMessage;

        public Dictionary<string, string> FieldMessage
        {
            get => _fieldMessage ?? (_fieldMessage = new Dictionary<string, string>());
            set => _fieldMessage = value;
        }

        public object Model;

        public string Message;
    }
}