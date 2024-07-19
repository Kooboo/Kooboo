//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.IndexedDB.CustomAttributes
{

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class KoobooIgnore : Attribute
    {

    }


    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class KoobooKeyIgnoreCase : Attribute
    {
    }

}
