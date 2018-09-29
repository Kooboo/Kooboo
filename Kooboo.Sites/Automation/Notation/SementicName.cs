//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Automation.Notation
{
   public class SementicName : INotation
    {
        public string Name
        {
            get { return "SementicName"; }
        }

        public Type ReturnType
        {
            get { return typeof(bool); }
        }

        public List<string> ReturnStringValueList
        {
            get { return null; }
        }

        public object Execute(Dom.Element element)
        {
            string id = element.id;
            string classname = element.className;

            if (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(classname))
            {
                return false;
            }
            else if (string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(classname))
            {
                classname = classname.ToLower(); 

                foreach (var item in Tag.Sementic.ClassNames)
                {
                    if (classname.Contains(item))
                    {
                        return true;
                    }
                }
            }
            else if (!string.IsNullOrEmpty(id) && string.IsNullOrEmpty(classname))
            {

                id = id.ToLower();

                foreach (var item in Tag.Sementic.ClassNames)
                {
                    if (id.Contains(item))
                    {
                        return true;
                    }
                }

            }
            else
            {
                // both are not null. 
                id = id.ToLower();
                classname = classname.ToLower(); 
                foreach (var item in Tag.Sementic.ClassNames)
                {
                    if (id.Contains(item) || classname.Contains(item))
                    {
                        return true;
                    }
                }

            }

      
            return false;
        }
    }
}
