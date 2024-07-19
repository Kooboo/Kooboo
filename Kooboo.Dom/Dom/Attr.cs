//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Dom
{
    /// <summary>
    // Attr objects are simply known as attributes. They are sometimes referred to as content attributes to avoid confusion with IDL attributes.
    //Attributes have a local name and value.
    //For legacy reasons, attributes also have an associated name, namespace, and namespace prefix.
    //When an attribute is created, its local name and value are always given. Unless explicitly given when an attribute is created, its name is identical to its local name, and its namespace and namespace prefix are null.
    /// </summary>
    [Serializable]
    public class Attr
    {

        public string localName;
        public string value;

        public string name;
        public string namespaceURI;
        public string prefix;

        public bool specified;

    }
}
