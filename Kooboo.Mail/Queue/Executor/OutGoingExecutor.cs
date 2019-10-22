//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Mail.Models;
using System.Threading.Tasks;

namespace Kooboo.Mail.Queue.Executor
{
    public class OutGoingExecutor : IExecutor<Model.OutGoing>
    {
        public async Task<ActionResponse> Execute(string jsonModel)
        {
            var model = Lib.Helper.JsonHelper.Deserialize<Model.OutGoing>(jsonModel);
            if (model != null)
            {
                var result = await Kooboo.Mail.Transport.Delivery.Send(model.MailFrom, model.RcptTo, model.MsgBody);
                return result;
            }
            return new ActionResponse() { Success = false, ShouldRetry = false, Message = "Invalid message body" };
        }

        public async Task SendFail(string jsonModel, string failReason)
        {
            var model = Lib.Helper.JsonHelper.Deserialize<Model.OutGoing>(jsonModel);
            if (model != null)
            {
                await Transport.Delivery.NotifyFailure(model.MailFrom, model.RcptTo, model.MsgBody, failReason);
            }
        }
    }
}