//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using System;

namespace Kooboo.Attributes
{
    // indicate one data method require the prouct type para first. used on Ecommerce product datasource..
    [AttributeUsage(AttributeTargets.Method)]
    public class RequireProductType : Attribute
    {
    }
}