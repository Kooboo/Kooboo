//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using Kooboo.Data.Interface;

namespace Kooboo.Sites.DataSources
{
    /// <summary>
    /// 提供用于读取 DataSource 元信息的方法。
    /// </summary>
    public static class DataSourceContainer
    {
        public static bool IsThirdPartyDataSource(Type dataSourceType)
        {
            return !typeof(IDataSource).IsAssignableFrom(dataSourceType);
        }
      
    }
}