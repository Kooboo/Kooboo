using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Scripting.Global
{
  public static  class kHelper
    {                    
        public static object PrepareData(object dataobj, Type modelType)
        {
            Dictionary<string, object> data = GetData(dataobj);

            var result = Lib.Reflection.TypeHelper.ToObject(data, modelType);

            return result;
        }

        public static Dictionary<string, object> GetData(object dataobj)
        {
            Dictionary<string, object> data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            System.Collections.IDictionary idict = dataobj as System.Collections.IDictionary;

            if (idict != null)
            {
                foreach (var item in idict.Keys)
                {
                    var value = idict[item];
                    if (value != null)
                    {
                        data.Add(item.ToString(), value.ToString());
                    }
                }
            }
            else
            {
                var dynamicobj = dataobj as IDictionary<string, object>;
                if (dynamicobj != null)
                {
                    foreach (var item in dynamicobj.Keys)
                    {
                        var value = dynamicobj[item];
                        if (value != null)
                        {
                            data.Add(item.ToString(), value.ToString());
                        }
                    }
                }
            }

            return data;
        }

    }
}
