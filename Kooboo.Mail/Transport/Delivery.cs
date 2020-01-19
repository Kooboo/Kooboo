//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Kooboo.Mail.Models;
using Kooboo.Mail.Utility;

namespace Kooboo.Mail.Transport
{
    public static class Delivery
    {
        private static Logging.ILogger _logger;

        static Delivery()
        {
            _logger = Logging.LogProvider.GetLogger("smtp", "send");
        }

        public static async Task<ActionResponse> Send(string MailFrom, string RcptTo, string MessageContent)
        {
            try
            {
                var result = await DoSend(MailFrom, RcptTo, MessageContent);
                if (result.Success)
                {
                    _logger.LogInformation($"{MailFrom},{RcptTo},Delivered,");
                }
                else
                {
                    _logger.LogInformation($"{MailFrom},{RcptTo},Bounced,{result.Message}");
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"{MailFrom},{RcptTo},Exception,{ex.Message}");
                throw;
            }
        }

        private static async Task<ActionResponse> DoSend(string MailFrom, string RcptTo, string MessageContent)
        {
            var setting = await  Settings.GetSendSetting(Data.AppSettings.ServerSetting, Data.AppSettings.IsOnlineServer, MailFrom, RcptTo);

            Kooboo.Data.Log.Instance.Email.Write("-- sending \r\n"); 
            Data.Log.Instance.Email.Write(MailFrom +" to: " + RcptTo);

            if (!setting.OkToSend)
            {
                return new ActionResponse() { Success = false, Message = setting.ErrorMessage, ShouldRetry = false };
            }

            using (var sendClient = new Kooboo.Mail.Smtp.SmtpClient(setting.LocalIp))
            {
                if (setting.UseKooboo)
                {
                    Kooboo.Data.Log.Instance.Email.Write(setting.KoobooServerIp + " " + setting.Port);

                    await sendClient.Connect(setting.KoobooServerIp, setting.Port);
                }
                else
                {
                    foreach (var item in setting.Mxs)
                    {
                        Kooboo.Data.Log.Instance.Email.Write(item);

                        try
                        {
                            await sendClient.Connect(item, 25);
                            break;
                        }
                        catch (System.Exception ex)
                        {
                            Data.Log.Instance.Email.WriteException(ex); 

                            // Kooboo.Mail.Smtp.Log.LogInfo(ex.Message + ex.StackTrace + ex.Source);
                        }
                    }
                }

                if (!sendClient.Connected)
                {
                    string msg = "Can not connect to remote mail server";
                    return new ActionResponse() { Success = false, ShouldRetry = true, Message = msg };
                }

                return await Send(sendClient, MailFrom, RcptTo, MessageContent, setting);
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



        public static async Task Send(string mailfrom, List<string> RctpTos, string MessageBody)
        {  
            if (RctpTos == null || RctpTos.Count == 0)
            {
                return;
            }
             
            foreach (var item in RctpTos)
            {
                Kooboo.Mail.Queue.QueueManager.AddSendQueue(mailfrom, item, MessageBody);
            }

            await Kooboo.Mail.Queue.QueueManager.Execute();
            //}
        }

        public static async Task NotifyFailure(string OrgMailFrom, string rcptto, string messagebody, string errorreason)
        {
            var emailbody = Utility.ComposeUtility.ComposeDeliveryFailed(OrgMailFrom, rcptto, messagebody, errorreason);

            var fromaddress = Utility.AddressUtility.GetAddress(OrgMailFrom);  // only to be sure. 

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
                await Send(msg.From, OrgMailFrom, emailbody);
            }
        }
    }

}
