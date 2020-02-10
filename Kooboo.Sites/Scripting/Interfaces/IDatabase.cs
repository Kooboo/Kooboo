//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Attributes;
using System.ComponentModel;

namespace KScript
{
    [KValueType(typeof(KTable))]
    public interface IDatabase
    {
        [KIgnore]
        ITable this[string key] { get;}

        [Description("Return the kScript database table object, if the table is not exists, it will be created.")]
        ITable GetTable(string Name);
    }
}