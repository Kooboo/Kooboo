//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Mail.Queue.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kooboo.Mail.Transport
{
    public static class Incoming
    {
        private static Logging.ILogger _logger;

        static Incoming()
        {
            _logger = Logging.LogProvider.GetLogger("smtp", "receive");
        }

        public static Task Receive(string mailFrom, List<string> rcptos, string messageBody)
        {
            _logger.LogInformation($"{mailFrom},{String.Join("|", rcptos)},Received");

            var msginfo = Kooboo.Mail.Utility.MessageUtility.ParseMeta(messageBody);
            return Receive(mailFrom, rcptos, messageBody, msginfo);
        }

        private static HashSet<string> correctRcptTo(List<string> orgTos)
        {
            HashSet<string> rcptTo = new HashSet<string>();
            foreach (var item in orgTos)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    var add = Mail.Utility.AddressUtility.GetAddress(item);

                    if (Kooboo.Mail.Utility.AddressUtility.IsValidEmailAddress(add))
                    {
                        rcptTo.Add(add);
                    }
                }
            }
            return rcptTo;
        }

        public static async Task Receive(string mailFrom, List<string> rcptos, string messageBody, Message msginfo)
        {
            Kooboo.Mail.Smtp.Log.LogInfo("Receive mail: " + mailFrom + " TO: " + string.Join(",", rcptos.ToArray()) + "\r\n" + messageBody);

            HashSet<string> rcptTo = correctRcptTo(rcptos);

            if (string.IsNullOrEmpty(mailFrom) || rcptTo.Count == 0)
            {
                return;
            }

            mailFrom = Mail.Utility.AddressUtility.GetAddress(mailFrom);

            msginfo.MailFrom = mailFrom;

            List<string> externalTos = new List<string>();

            foreach (var item in rcptTo)
            {
                var orgdb = Kooboo.Mail.Factory.DBFactory.OrgDb(item);
                if (orgdb != null)
                {
                    var address = orgdb.EmailAddress.Find(item);
                    if (address != null)
                    {
                        msginfo.OutGoing = false;
                        msginfo.AddressId = address.Id;
                        msginfo.RcptTo = item;
                        var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(address.UserId, orgdb.OrganizationId);
                        await Receive(orgdb, maildb, address, mailFrom, messageBody, msginfo);
                    }
                    else
                    {
                        var org = Kooboo.Data.GlobalDb.Organization.Get(orgdb.OrganizationId);
                        if (org != null && org.ServerId != Kooboo.Data.AppSettings.ServerSetting.ServerId)
                        {
                            // smart host.
                            externalTos.Add(item);
                        }
                        else
                        {
                            await Delivery.NotifyFailure(mailFrom, item, messageBody, "Mailbox not found");
                        }
                    }
                }
                else
                {
                    // external email...
                    externalTos.Add(item);
                }
            }

            await Kooboo.Mail.Transport.Delivery.Send(mailFrom, externalTos, messageBody);
        }

        public static void SaveSent(string mailFrom, Message msg, string messagebody)
        {
            var address = Utility.AddressUtility.GetAddress(mailFrom);
            msg.OutGoing = true;
            var localemailaddress = Kooboo.Mail.Utility.AddressUtility.GetLocalEmailAddress(address);
            if (localemailaddress != null)
            {
                var userdb = Kooboo.Mail.Factory.DBFactory.UserMailDb(localemailaddress.UserId, localemailaddress.OrgId);
                msg.FolderId = Folder.ToId(Folder.Sent);
                msg.Id = 0; // or clone and set id = 0;
                msg.AddressId = localemailaddress.Id;
                userdb.Messages.Add(msg, messagebody);
            }
        }

        public static async Task Receive(OrgDb orgdb, MailDb maildb, EmailAddress address, string mailFrom, string messagebody, Message msg)
        {
            // always in first.
            var folder = Spam.SpamFilter.DetermineFolder();
            msg.FolderId = folder.Id;
            maildb.Messages.Add(msg, messagebody);

            if (address.AddressType == EmailAddressType.Forward)
            {
                mailFrom = Utility.AddressUtility.GetAddress(mailFrom);

                if (address.ForwardAddress != null && Utility.AddressUtility.IsValidEmailAddress(address.ForwardAddress))
                {
                    var newbody = Kooboo.Mail.Utility.ComposeUtility.ComposeForwardAddressMessage(messagebody);

                    if (!string.IsNullOrEmpty(newbody))
                    {
                        List<string> forwardtos = new List<string> {address.ForwardAddress};
                        await Receive(mailFrom, forwardtos, newbody);
                    }
                }
            }
            else if (address.AddressType == EmailAddressType.Group)
            {
                mailFrom = Utility.AddressUtility.GetAddress(mailFrom);

                GroupMail group = new GroupMail();
                string newbody = Utility.ComposeUtility.ComposeGroupMail(messagebody, address.Address);

                if (!string.IsNullOrEmpty(newbody))
                {
                    var memebers = orgdb.EmailAddress.GetMembers(address.Id);
                    group.MailFrom = mailFrom;
                    group.Members = RemoveSelfMembers(memebers.ToList(), address.Address, mailFrom);
                    group.MessageContent = newbody;

                    if (group.Members.Count() <= 5)
                    {
                        await Receive(mailFrom, group.Members, newbody);
                    }
                    else
                    {
                        Queue.QueueManager.AddGroupMail(group);
                        await Queue.QueueManager.Execute();
                    }
                }
            }
        }

        private static List<string> RemoveSelfMembers(List<string> members, string selfAddress, string fromAddress)
        {
            var lower = selfAddress.ToLower();
            var fromlower = fromAddress.ToLower();
            var finds = members.FindAll(o => o.ToLower() == lower || o.ToLower() == fromlower);
            if (finds != null)
            {
                foreach (var item in finds)
                {
                    members.Remove(item);
                }
            }
            return members;
        }

        public static async Task Receive(Kooboo.Mail.Smtp.SmtpSession session)
        {
            if (session.HasMessageBody)
            {
                string clienthost = session.ClientHostName ?? "unknown";

                string ip = session.ClientIP;

                string msgbody = session.MessageBody;
                msgbody = Kooboo.Mail.Utility.MessageUtility.CheckNFixMessage(msgbody);

                string from = GetMailFrom(session);

                var tos = GetRcptTos(session);

                if (!string.IsNullOrEmpty(msgbody) && !string.IsNullOrEmpty(from) && tos != null && tos.Count > 0)
                {
                    msgbody = "Received: from " + clienthost + " (" + session.ClientIP + ") by Kooboo Smtp Server; " + DateTime.UtcNow.ToString("r") + "\r\n" + msgbody;
                }

                string log = "SMTP received: " + session.ClientIP + " " + session.UserName + " " + session.Password;
                _logger.LogInformation(log);

                if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(msgbody) && tos.Count > 0)
                {
                    await Receive(from, tos.ToList(), msgbody);
                }
            }
        }

        internal static string GetMailFrom(Smtp.SmtpSession session)
        {
            foreach (var item in session.Log)
            {
                if (item.Key.Name == Smtp.SmtpCommandName.MAILFROM && item.Value.Code == 250)
                {
                    return Utility.AddressUtility.GetAddress(item.Key.Value);
                }
            }
            return null;
        }

        internal static HashSet<string> GetRcptTos(Smtp.SmtpSession session)
        {
            HashSet<string> tos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in session.Log)
            {
                if (item.Key.Name == Smtp.SmtpCommandName.RCPTTO && item.Value.Code == 250)
                {
                    if (!string.IsNullOrEmpty(item.Key.Value))
                    {
                        tos.Add(item.Key.Value);
                    }
                }
            }
            return tos;
        }
    }
}