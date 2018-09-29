//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;  
using System.Linq;    

namespace Kooboo.Lib.Helper
{
  public static  class JsonHelper
    {
        private static JsonSerializerSettings camelsetting  = new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() };

        public static string Serialize(object Model)
        {
            return  JsonConvert.SerializeObject(Model, Formatting.Indented, camelsetting);  
        }

        public static string SerializeCaseSensitive(object model)
        {
            return JsonConvert.SerializeObject(model, Formatting.Indented); 
        }

        public static T Deserialize<T>(string json)
        {
            if (json[0] == 65279)
            {
                json = json.Substring(1); 
            }
            return JsonConvert.DeserializeObject<T>(json, camelsetting); 
        } 

        public static JObject DeserializeJObject(string json)
        {
            if (json[0] == 65279)
            {
                json = json.Substring(1);
            } 
            return (JObject)JsonConvert.DeserializeObject(json); 
        }

        public static object DeserialzeBaseObject(string json)
        {
            if (json[0] == 65279)
            {
                json = json.Substring(1);
            }
            return  JsonConvert.DeserializeObject(json);
        }

        public static object DeserializeObject(string json)
        {
            if (json[0] == 65279)
            {
                json = json.Substring(1);
            }

            return  JsonConvert.DeserializeObject(json);
        }

         

        public static object  Deserialize(string json)
        {
            if (json[0] == 65279)
            {
                json = json.Substring(1);
            }

            return  JsonConvert.DeserializeObject(json);
        }


        public static object Deserialize(string json, Type type)
        {
           if (json[0] == 65279)
            {
                json = json.Substring(1); 
            }
            return JsonConvert.DeserializeObject(json, type);
        }

        public static string GetString(JObject jobject, string memberName)
        {
            var token = GetObject(jobject, memberName); 
            if (token==null)
            {
                return null; 
            }
            else
            { return token.ToString();  }
        }

        public static Object GetObject(JObject jobject, string memberName)
        {
            if (jobject == null)
            {
                return null; 
            }
            var token = jobject[memberName];
            if (token == null)
            {
                string lower = memberName.ToLower();

                foreach (var item in jobject.Properties().ToList())
                {
                    if (item.Name.ToLower() == lower)
                    {
                        token = item.Value;
                    }
                }
            }
            return token;  
        }

        // dirty way of checking IsJson..
        public static bool IsJson(string input)
        {
            if (input == null)
            {
                return false; 

            }

            input = input.Trim();
            return input.StartsWith("{") && input.EndsWith("}")
                   || input.StartsWith("[") && input.EndsWith("]");
        }
    }
}
