//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Threading.Tasks;
using Kooboo.Mail.Models;

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
