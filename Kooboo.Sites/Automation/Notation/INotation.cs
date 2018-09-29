using System;
using System.Collections.Generic;
using Kooboo.Dom;

namespace Kooboo.Sites.Automation.Notation
{
    public interface INotation
    {
       

        /// <summary>
        /// The unique name of this notation. 
        /// </summary>
        string Name { get; }

        /// <summary>
        /// return value type. This is used for rule configuration page. 
        /// Return type are only allowed Bool, Interger, string. 
        /// In case that you will be returning a list of string value, you should also return them in the ReturnStringValueList
        /// </summary>
        Type ReturnType { get; }


        /// <summary>
        /// in case you would like to return string list of all possible string values.
        /// this is used for rule configuration page. 
        /// </summary>
        List<string> ReturnStringValueList { get; }

        /// <summary>
        /// Execute and return the result. 
        /// </summary>
        /// <returns></returns>
        object Execute(Element element);



    }
}
