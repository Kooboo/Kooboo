//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

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
