//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Interface
{
    public interface IDataMethodSetting : IGolbalObject, ICoreObject
    {
        /// <summary>
        /// By default,this is same as the OriginalMethodName. 
        /// </summary>
        string MethodName { get; set; }

        string DisplayName { get; }

        string Description { get; set; }

        /// <summary>
        /// The type that contains this method.
        /// </summary>
        string DeclareType { get; set; }

        Guid DeclareTypeHash { get; set; }

        /// <summary>
        /// To indicate whether this is a third party type or an IDataSource type.
        /// </summary>
        bool IsThirdPartyType { get; set; }

        string ReturnType { get; set; }

        bool IsPagedResult { get; set; }

        string OriginalMethodName { get; set; }

        Guid MethodSignatureHash { get; set; }

        bool IsStatic { get; set; }

        bool IsVoid { get; set; }

        bool IsGlobal { get; set; }

        /// <summary>
        ///  For form submit... form post action...
        /// </summary>
        bool IsPost { get; set; }

        /// <summary>
        /// Awaitable tasks. 
        /// </summary>
        bool IsTask { get; set; }


        bool IsPublic { get; set; }

          Guid CodeId { get; set; }

          bool IsKScript { get; }

        Dictionary<string, string> Parameters { get; set; }

        /// <summary>
        /// The flat fields of parameter bindings... 
        /// </summary>
        Dictionary<string, ParameterBinding> ParameterBinding { get; set; }



    }
}
