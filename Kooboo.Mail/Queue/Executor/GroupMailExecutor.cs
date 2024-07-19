//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kooboo.Mail.Models;

namespace Kooboo.Mail.Queue.Executor
{
    public class GroupMailExecutor : IExecutor<Model.GroupMail>
    {
        public async Task<ActionResponse> Execute(string JsonModel)
        {
            var model = Lib.Helper.JsonHelper.Deserialize<Model.GroupMail>(JsonModel);
            if (model != null)
            {
                // check  if local or remote.... 
                List<string> local = new List<string>();
                List<string> remote = new List<string>();

                foreach (var item in model.Members)
                {
                    if (Utility.AddressUtility.IsLocalEmailAddress(item))
                    {
                        local.Add(item);
                    }
                    else
                    {
                        remote.Add(item);
                    }
                }

                if (local.Count() > 0)
                {
                    await Transport.Incoming.Receive(model.MailFrom, local, model.MessageContent);
                }
                else
                {
                    foreach (var item in remote)
                    {
                        var response = await Transport.Delivery.Send(model.MailFrom, item, model.MessageContent);

                        if (!response.Success && response.ShouldRetry)
                        {
                            Queue.QueueManager.AddSendQueue(model.MailFrom, item, model.MessageContent);
                        }
                    }
                }
            }

            // group mail will break into smaller, always true. 
            return new ActionResponse() { Success = true };
        }

        public Task SendFail(string JsonModel, string FailReason)
        {
            // break into smaller send... willl not fail as a group. 
            return Task.FromResult(false);
        }
    }
}
