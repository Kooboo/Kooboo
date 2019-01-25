//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Diagnosis
{
   public class Message
    {
        public Guid CheckerId { get; set; }

        public string body { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public MessageType Type { get; set; }
    }
}
