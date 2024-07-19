//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Lib.Serializer
{
    public class Copy
    {
        public static T DeepCopy<T>(T input)
        {
            var json = Lib.Helper.JsonHelper.Serialize(input);

            return Lib.Helper.JsonHelper.Deserialize<T>(json);
        }
    }
}
