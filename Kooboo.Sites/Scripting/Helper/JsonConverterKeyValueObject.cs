using Kooboo.Sites.Scripting.Global.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Sites.Scripting.Helper
{
    class JsonConverterKeyValueObject : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }


        public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {

            return serializer.Deserialize(reader, typeof(object));

        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (value is KeyValueObject)
            {
                serializer.Serialize(writer, value?.ToString());
            }
        }

    }
}
