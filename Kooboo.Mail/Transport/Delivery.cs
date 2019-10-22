//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Mail.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Kooboo.Mail.Transport
{
    public static class Delivery
    {
        private static Logging.ILogger _logger;

        static Delivery()
        {
            _logger = Logging.LogProvider.GetLogger("smtp", "send");
        }

        public static async Task<ActionResponse> Send(string mailFrom, string rcptTo, string messageContent)
        {
            try
            {
                var result = await DoSend(mailFrom, rcptTo, messageContent);
                _logger.LogInformation(result.Success
                    ? $"{mailFrom},{rcptTo},Delivered,"
                    : $"{mailFrom},{rcptTo},Bounced,{result.Message}");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"{mailFrom},{rcptTo},Exception,{ex.Message}");
                throw;
            }
        }

        private static async Task<ActionResponse> DoSend(string mailFrom, string rcptTo, string messageContent)
        {
            var setting = await Settings.GetSendSetting(Data.AppSettings.ServerSetting, Data.AppSettings.IsOnlineServer, mailFrom, rcptTo);

            if (!setting.OkToSend)
            {
                return new ActionResponse() { Success = false, Message = setting.ErrorMessage, ShouldRetry = false };
            }

            using (var sendClient = new Kooboo.Mail.Smtp.SmtpClient(setting.LocalIp))
            {
                if (setting.UseKooboo)
                {
                    await sendClient.Connect(setting.KoobooServerIp, setting.Port);
                }
                else
                {
                    foreach (var item in setting.Mxs)
                    {
                        try
                        {
                            await sendClient.Connect(item, 25);
                            break;
                        }
                        catch (System.Exception ex)
                        {
                            Kooboo.Mail.Smtp.Log.LogInfo(ex.Message + ex.StackTrace + ex.Source);
                        }
                    }
                }

                if (!sendClient.Connected)
                {
                    string msg = "Can not connect to remote mail server";
                    return new ActionResponse() { Success = false, ShouldRetry = true, Message = msg };
                }

                return await Send(sendClient, mailFrom, rcptTo, messageContent, setting);
            }
        }

        private static async Task<ActionResponse> Send(Kooboo.Mail.Smtp.SmtpClient client, string mailFrom, string rcptTo, string content, SendSetting setting)
        {
            var reply = await client.CheckServiceReady();
            if (!reply.Ok)
            {
                client.Release();
                return new ActionResponse() { Success = false, ShouldRetry = true, Message = reply.Reply };
            }

            reply = await client.Helo(setting.HostName);

            if (!reply.Ok)
            {
                client.Release();
                return new ActionResponse() { Success = false, ShouldRetry = true, Message = reply.Reply };
            }

            reply = await client.MailFrom(mailFrom);

            if (!reply.Ok)
            {
                client.Release();
                return new ActionResponse() { Success = false, ShouldRetry = false, Message = reply.Reply };
            }

            reply = await client.RcptTo(rcptTo);

            if (!reply.Ok)
            {
                client.Release();
                return new ActionResponse() { Success = false, ShouldRetry = false, Message = reply.Reply };
            }

            reply = await client.Data(content);

            if (!reply.Ok)
            {
                client.Release();

                return new ActionResponse() { Success = false, ShouldRetry = false, Message = reply.Reply };
            }

            await client.Quit();

            return new ActionResponse() { Success = true };
        }

        public static async Task Send(string mailfrom, List<string> rctpTos, string messageBody)
        {
            if (rctpTos == null || rctpTos.Count == 0)
            {
                return;
            }

            //Kooboo.Mail.Smtp.Log.LogInfo("sending external  mails: " + mailfrom + " TO: " + string.Join(",", RctpTos.ToArray()) + "\r\n" + MessageBody);

            //if (RctpTos.Count() <= 3)
            //{
            //    foreach (var item in RctpTos)
            //    {
            //       var result = await Send(mailfrom, item, MessageBody);

            //        if (!result.Success)
            //        {
            //            if (result.ShouldRetry)
            //            {
            //                Kooboo.Mail.Queue.QueueManager.AddSendQueue(mailfrom, item, MessageBody);
            //            }
            //            else
            //            {
            //               NotifyFailure(mailfrom, item, MessageBody, result.Message);
            //            }
            //        }
            //    }
            //}
            //else
            //{
            //_logger.Log($"{mailfrom},{String.Join("|", RctpTos)},Queued");

            foreach (var item in rctpTos)
            {
                Kooboo.Mail.Queue.QueueManager.AddSendQueue(mailfrom, item, messageBody);
            }

            await Kooboo.Mail.Queue.QueueManager.Execute();
            //}
        }

        public static async Task NotifyFailure(string orgMailFrom, string rcptto, string messagebody, string errorreason)
        {
            var emailbody = Utility.ComposeUtility.ComposeDeliveryFailed(orgMailFrom, rcptto, messagebody, errorreason);

            var fromaddress = Utility.AddressUtility.GetAddress(orgMailFrom);  // only to be sure.

            Message msg = Utility.MessageUtility.ParseMeta(emailbody);

            if (msg.To != null && (msg.To.ToLower().Contains("noreply") || msg.To.ToLower().Contains("no-reply")))
            {
                return;
            }

            var orgdb = Kooboo.Mail.Factory.DBFactory.OrgDb(fromaddress);
            if (orgdb != null)
            {
                var address = orgdb.EmailAddress.Find(fromaddress);
                if (address != null)
                {
                    msg.AddressId = address.Id;
                    msg.RcptTo = fromaddress;
                    msg.MailFrom = "postmaster@noreply.kooboo.com";
                    var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(address.UserId, orgdb.OrganizationId);
                    await Transport.Incoming.Receive(orgdb, maildb, address, msg.From, emailbody, msg);
                }
            }
            else
            {
                await Send(msg.From, orgMailFrom, emailbody);
            }
        }
    }
}