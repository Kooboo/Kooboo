//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Mail.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Mail.Queue.Executor
{

    public interface IExecutor<T> : IExecutor
    {

    } 

    public interface IExecutor
    {  
        Task<ActionResponse> Execute(string JsonModel);

        Task SendFail(string JsonModel, string FailReason); 
    }
}
