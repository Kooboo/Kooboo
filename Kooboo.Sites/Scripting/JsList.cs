using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Scripting
{
    public class JsList<T> : List<T>
    {
        public int length
        {
            get
            {
                return this.Count;
            }
        }
    }
    public static class Enumerable
    {
        public static JsList<T> ToJSList<T>(this IEnumerable<T> list)
        {
            var jsList = new JsList<T>();
            if(list!=null)
            {
                foreach (var item in list)
                {
                    jsList.Add(item);
                }
            }
            return jsList;
        }

    }
}