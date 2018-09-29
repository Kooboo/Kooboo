//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Service
{
  public static  class RelationService
    {

        public static Dictionary<string, int> CountByRelation(List<Kooboo.Sites.Relation.ObjectRelation> relations, bool CountConstTypeX = true)
        {
            Dictionary<string, int> result = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase); 

            if (relations !=null)
            {
                foreach (var item in relations)
                {
                    string name = null;
                    if (CountConstTypeX)
                    {
                        name = ConstObjectType.GetName(item.ConstTypeX);
                    }
                    else
                    {
                        name = ConstObjectType.GetName(item.ConstTypeY); 
                    }

                    if (result.ContainsKey(name))
                    {
                        var value = result[name];
                        value = value + 1;
                        result[name] = value; 
                    }
                    else
                    {
                        result.Add(name, 1); 
                    } 
                } 
            }

            return result; 
        } 
    }
}
