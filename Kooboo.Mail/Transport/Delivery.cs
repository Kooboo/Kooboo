//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kooboo.Mail.Models;
using MailKit;

namespace Kooboo.Mail.Transport
{
    public static class Delivery
    {
        public static async Task<ActionResponse> Send(string MailFrom, string RcptTo, string MessageContent)
        {
            bool shouldRetry = false;
            try
            {
                var setting = await Settings.GetSendSetting(Data.AppSettings.IsOnlineServer,
                    MailFrom, RcptTo);

                if (!setting.OkToSend)
                {
                    return new ActionResponse() { Success = false, Message = setting.ErrorMessage, ShouldRetry = false };
                }

                string server;
                int port = 25;
                string username = null;
                string password = null;
                if (setting.UseKooboo)
                {
                    server = setting.Server;
                    port = setting.Port;
                }
                else if (setting.CustomSmtp)
                {
                    server = setting.Server;
                    port = setting.Port;
                    username = setting.UserName;
                    password = setting.Password;
                }
                else
                {
                    if (setting.Mxs == null || !setting.Mxs.Any())
                    {
                        return new ActionResponse()
                        { Success = false, Message = "mx record not found", ShouldRetry = false };
                    }
                    else
                    {
                        server = setting.Mxs[0];
                        port = 25;
                    }
                }

                if (!string.IsNullOrEmpty(server))
                {
                    return Utility.MailKitUtility.Send(MailFrom, RcptTo, MessageContent, server, port, setting.UseKooboo, username, password);
                }
            }
            catch (Exception ex)
            {
                if (ex is ServiceNotConnectedException)
                {
                    shouldRetry = true;
                }
            }

            return new ActionResponse() { Success = false, ShouldRetry = shouldRetry };
        }

        public static void EnsureSend(string mailfrom, List<string> RctpTos, string MessageBody)
        {
            if (RctpTos == null || RctpTos.Count == 0)
            {
                return;
            }

            foreach (var item in RctpTos)
            {
                Kooboo.Mail.Queue.QueueManager.AddSendQueue(mailfrom, item, MessageBody);
            }
            Task.Run(() => Kooboo.Mail.Queue.QueueManager.Execute());
            //await Kooboo.Mail.Queue.QueueManager.Execute();
        }

        public static async Task NotifyFailure(string OrgMailFrom, string rcptto, string messagebody,
            string errorreason)
        {
            var emailBody = Utility.ComposeUtility.ComposeDeliveryFailed(OrgMailFrom, rcptto, messagebody, errorreason);

            string newMailFrom = "postmaster_noreply@kooboo.net";
            await Send(newMailFrom, OrgMailFrom, emailBody);
        }
    }
}