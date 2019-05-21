using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Kooboo.Sites.Scripting.Helper
{
    public class JsonConverterDynamicObject : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true; 
        }
         

        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            
          return serializer.Deserialize(reader, typeof(IDictionary<string, object>));
             
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (value is Kooboo.Data.Interface.IDynamic)
            {
                var dynamic = value as Kooboo.Data.Interface.IDynamic;

                serializer.Serialize(writer, dynamic.Values);  
            }
        }

    }
}
