//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Definition;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
  
//Kooboo.Sites.DataSources.FilterDefinition

namespace Kooboo.Sites.DataSources
{ 
    public class FilterDefinition
    {
        public string FieldName { get; set; }

        public bool IsNameValueType { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Comparer Comparer { get; set; } = Comparer.EqualTo; 

        public string FieldValue { get; set; } 

        public bool IsValueValueType { get; set; }

    }
}
