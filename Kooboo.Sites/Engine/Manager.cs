//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Engine
{
   public static class Manager
    {

        public static Dictionary<string, IEngine> List { get; set; }

        static Manager()
        {      
            List = new Dictionary<string, IEngine>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in Lib.Reflection.AssemblyLoader.LoadTypeByInterface(typeof(IEngine)))
            {
                var instance = Activator.CreateInstance(item) as IEngine;

                if (instance != null)
                {
                    List[instance.Name] = instance;
                }
            } 
        }
              
        public static IEngine Get(string name)
        {
            if (name !=null)
            {
                if (List.ContainsKey(name))
                {
                    return List[name]; 
                }
            }
            return null; 
        }

        public static bool HasEngine(string engineName)
        {
            var engine = Get(engineName);
            return engine != null; 
        }
         
        public static string Execute(string engineName, RenderContext context, string sourcecode,  string tagName=null, Dictionary<string, string> attributes=null)
        { 
            var engine = Get(engineName);
            if (engine !=null)
            {
                var result = engine.Execute(context, sourcecode); 
                if (engine.KeepTag)
                {
                    if (tagName !=null)
                    {
                        string open = Service.DomService.GenerateOpenTag(attributes, tagName); 
                        result = open + result + "</" + tagName + ">";  
                    }
                }

                return result; 
            }
            return null; 
        }
          
        public static List<IEngine> GetScript()
        {
            return List.Values.Where(o => o.IsScript).ToList(); 
        }

        public static List<IEngine> GetStyle()
        {
            return List.Values.Where(o => o.IsStyle).ToList();
        }  

    }
}
