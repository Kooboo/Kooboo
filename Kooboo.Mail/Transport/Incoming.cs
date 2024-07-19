//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ical.Net;
using Kooboo.Data;
using Kooboo.Data.Language;
using Kooboo.Data.Models;
using Kooboo.Mail.Imap;
using Kooboo.Mail.Models;
using Kooboo.Mail.Queue.Model;
using Kooboo.Mail.Utility;
using MimeKit;
using NUglify.Helpers;


namespace Kooboo.Mail.Transport
{
    public static class Incoming
    {
        private static Logging.ILogger _logger;

        static Incoming()
        {
            _logger = Logging.LogProvider.GetLogger("smtp", "receive");
        }

        public static Task Receive(string MailFrom, List<string> Rcptos, string MessageBody)
        {
            var msginfo = Kooboo.Mail.Utility.MessageUtility.ParseMeta(MessageBody);
            return Receive(MailFrom, Rcptos, MessageBody, msginfo);
        }

        private static HashSet<string> correctRcptTo(List<string> orgTos)
        {
            HashSet<string> RcptTo = new HashSet<string>();
            foreach (var item in orgTos)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    var add = Mail.Utility.AddressUtility.GetAddress(item);

                    if (Kooboo.Mail.Utility.AddressUtility.IsValidEmailAddress(add))
                    {
                        RcptTo.Add(add);
                    }
                }
            }
            return RcptTo;
        }

        public static async Task Receive(string MailFrom, List<string> Rcptos, string MessageBody, Message msginfo)
        {
            string log = "Receive mail: " + MailFrom + " TO: " + string.Join(",", Rcptos.ToArray()) + "\r\n" + MessageBody;

            Kooboo.Data.Log.Instance.Email.Write(log);

            HashSet<string> RcptTo = correctRcptTo(Rcptos);

            if (string.IsNullOrEmpty(MailFrom) || RcptTo.Count() == 0)
            {
                return;
            }

            MailFrom = Mail.Utility.AddressUtility.GetAddress(MailFrom);

            msginfo.MailFrom = MailFrom;

            List<string> ExternalTos = new List<string>();

            MailDb SenderDb = null;

            foreach (var item in RcptTo)
            {
                var orgdb = Kooboo.Mail.Factory.DBFactory.OrgDb(item);
                if (orgdb != null)
                {
                    var address = orgdb.Email.Find(item);
                    if (address != null)
                    {
                        msginfo.OutGoing = false;
                        msginfo.AddressId = address.Id;
                        msginfo.RcptTo = item;
                        var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(address.UserId, orgdb.OrganizationId);

                        var contacts = maildb.AddBook.GetByAddress(MailFrom);

                        bool IsContact = contacts != null;

                        await Receive(orgdb, maildb, address, MailFrom, MessageBody, msginfo, IsContact);

                        // generate delivery log.  
                        var report = new SmtpReportIn() { IsSuccess = true, MessageId = msginfo.SmtpMessageId, HeaderFrom = msginfo.From, RcptTo = item, Log = "Internal Delivery", Subject = msginfo.Subject, DateTick = DateTime.UtcNow.Ticks };

                        if (SenderDb == null)
                        {
                            SenderDb = GetSenderMailDb(MailFrom);
                        }

                        if (SenderDb != null)
                        {
                            SenderDb.SmtpDelivery.AddReport(report);
                        }

                    }
                    else
                    {
                        await Delivery.NotifyFailure(MailFrom, item, MessageBody, "internal mailbox not found");
                    }
                }
                else
                {
                    // external email... 
                    ExternalTos.Add(item);
                }
            }
            //if (SenderDb is not null)
            //{
            //    var user = GlobalDb.Users.Get(SenderDb.UserId);
            //    if (user is not null)
            //        Kooboo.Mail.Transport.Incoming.SaveSent(MailFrom, msginfo, MessageBody, user);
            //}
            Kooboo.Mail.Transport.Delivery.EnsureSend(MailFrom, ExternalTos, MessageBody);
        }

        //public static async Task Receive(string MailFrom, InternetAddressList Rcptos, string MessageBody, Message msginfo)
        //{
        //    string log = "Receive mail: " + MailFrom + " TO: " + string.Join<InternetAddress>(",", Rcptos.ToArray()) + "\r\n" + MessageBody;

        //    Kooboo.Data.Log.Instance.Email.Write(log);

        //    //HashSet<string> RcptTo = correctRcptTo(Rcptos);

        //    if (string.IsNullOrEmpty(MailFrom) || Rcptos.Count() == 0)
        //    {
        //        return;
        //    }

        //    MailFrom = Mail.Utility.AddressUtility.GetAddress(MailFrom);

        //    msginfo.MailFrom = MailFrom;

        //    List<string> ExternalTos = new List<string>();

        //    MailDb SenderDb = null;

        //    foreach (var item in Rcptos)
        //    {
        //        var toMailboxAddress = item as MailboxAddress;
        //        var orgdb = Kooboo.Mail.Factory.DBFactory.OrgDb(toMailboxAddress.Address);
        //        if (orgdb != null)
        //        {
        //            var address = orgdb.Email.Find(toMailboxAddress.Address);
        //            if (address != null)
        //            {
        //                msginfo.OutGoing = false;
        //                msginfo.AddressId = address.Id;
        //                msginfo.RcptTo = toMailboxAddress.ToString();
        //                var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(address.UserId, orgdb.OrganizationId);

        //                var contacts = maildb.AddBook.GetByAddress(MailFrom);

        //                bool IsContact = contacts != null;

        //                await Receive(orgdb, maildb, address, MailFrom, MessageBody, msginfo, IsContact);

        //                // generate delivery log.  
        //                var report = new SmtpReportIn() { IsSuccess = true, MessageId = msginfo.SmtpMessageId, HeaderFrom = msginfo.From, RcptTo = toMailboxAddress.ToString(), Log = "Internal Delivery", Subject = msginfo.Subject, DateTick = DateTime.UtcNow.Ticks };

        //                if (SenderDb == null)
        //                {
        //                    SenderDb = GetSenderMailDb(MailFrom);
        //                }

        //                if (SenderDb != null)
        //                {
        //                    SenderDb.SmtpDelivery.AddReport(report);
        //                }

        //            }
        //            else
        //            {
        //                await Delivery.NotifyFailure(MailFrom, toMailboxAddress.ToString(), MessageBody, "internal mailbox not found");
        //            }
        //        }
        //        else
        //        {
        //            // external email... 
        //            ExternalTos.Add(toMailboxAddress.ToString());
        //        }
        //    }
        //    Kooboo.Mail.Transport.Delivery.EnsureSend(MailFrom, ExternalTos, MessageBody);
        //}


        private static MailDb GetSenderMailDb(string mailfrom)
        {
            var orgdb = Kooboo.Mail.Factory.DBFactory.OrgDb(mailfrom);
            if (orgdb != null)
            {
                var address = orgdb.Email.Find(mailfrom);
                if (address != null)
                {
                    var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(address.UserId, orgdb.OrganizationId);
                    return maildb;
                }
            }
            return null;
        }

        public static void SaveSent(string MailFrom, Message msg, string messagebody, User user)
        {
            msg.OutGoing = true;
            var localemailaddress = Kooboo.Mail.Utility.AddressUtility.GetLocalEmailAddress(MailFrom);

            if (localemailaddress == null || localemailaddress.UserId != user.Id || localemailaddress.OrgId != user.CurrentOrgId)
            {
                var hostname = Utility.AddressUtility.GetEmailDomain(MailFrom);
                var text = Hardcoded.GetValue("550 {0} does not belongs to {1}");
                var value = string.Format(text, hostname, user.CurrentOrgName);
                throw new Exception(value);
            }

            if (localemailaddress != null)
            {
                var userdb = Kooboo.Mail.Factory.DBFactory.UserMailDb(localemailaddress.UserId, localemailaddress.OrgId);
                msg.FolderId = Folder.ToId(Folder.Sent);
                msg.Id = 0; // or clone and set id = 0;  
                msg.MsgId = 0;
                msg.AddressId = localemailaddress.Id;
                msg.Read = true;
                userdb.Message2.Add(msg, messagebody);
                CalendarMailDealing(userdb, messagebody);
            }
        }

        public static async Task Receive(OrgDb orgdb, MailDb maildb, EmailAddress address, string MailFrom, string messagebody, Message msg, bool IsContact)
        {
            // always in first.   
            var folder = Spam.SpamFilter.DetermineFolder(messagebody, IsContact);
            msg.FolderId = folder.Id;

            if (folder.Name == Folder.Spam)
            {
                msg.Read = true;
            }

            CalendarMailDealing(maildb, messagebody);

            maildb.Message2.Add(msg, messagebody);

            if (address.AddressType == EmailAddressType.Forward)
            {
                MailFrom = Utility.AddressUtility.GetAddress(MailFrom);

                if (address.ForwardAddress != null && Utility.AddressUtility.IsValidEmailAddress(address.ForwardAddress))
                {
                    var newbody = Kooboo.Mail.Utility.ComposeUtility.ComposeForwardAddressMessage(messagebody);

                    if (!string.IsNullOrEmpty(newbody))
                    {
                        List<string> forwardtos = new List<string>();
                        forwardtos.Add(address.ForwardAddress);
                        await Receive(MailFrom, forwardtos, newbody);
                    }
                }
            }

            else if (address.AddressType == EmailAddressType.Group)
            {
                MailFrom = Utility.AddressUtility.GetAddress(MailFrom);

                GroupMail group = new GroupMail();
                string newbody = Utility.ComposeUtility.ComposeGroupMail(messagebody, address.Address);

                if (!string.IsNullOrEmpty(newbody))
                {
                    var memebers = orgdb.Email.GetMembers(address.Id);
                    group.MailFrom = address.Address;
                    group.Members = RemoveSelfMembers(memebers.ToList(), address.Address, MailFrom);
                    group.MessageContent = newbody;

                    if (group.Members.Count() <= 5)
                    {
                        await Receive(MailFrom, group.Members, newbody);
                    }
                    else
                    {
                        Queue.QueueManager.AddGroupMail(group);

                        Task.Run(() => Queue.QueueManager.Execute());
                    }
                }
            }
        }

        public static void CalendarMailDealing(MailDb maildb, string messagebody)
        {
            if (string.IsNullOrEmpty(messagebody)) { return; }
            byte[] bodyData = System.Text.Encoding.UTF8.GetBytes(messagebody);
            MemoryStream ms = new MemoryStream(bodyData);

            MimeMessage mimeMessage = null;
            try
            {
                mimeMessage = MimeMessage.Load(ms);
            }
            catch (Exception)
            {
                mimeMessage = null;
            }

            if (mimeMessage == null)
            {
                return;
            }

            if (mimeMessage.BodyParts.Count() > 0)
            {
                mimeMessage.BodyParts.ForEach(part =>
                {
                    if ("text/calendar".Equals(part.ContentType.MimeType, StringComparison.OrdinalIgnoreCase))
                    {
                        string calendarContent = ((TextPart)(part)).Text;
                        Calendar calendar = Calendar.Load(calendarContent);
                        var calendarEvent = calendar.Events[0];
                        var calendarId = calendarEvent.Uid;
                        CalendarInfo calendarInfo = new CalendarInfo()
                        {
                            Id = calendarEvent.Uid,
                            CalendarTitle = calendarEvent.Summary,
                            //Attendees = calendarEvent.Attendees.ToList(),
                            Start = calendarEvent.Start.AsUtc,
                            End = calendarEvent.End.AsUtc,
                            Location = calendarEvent.Location,
                            Mark = calendarEvent.Description,
                        };

                        var attendess = calendarEvent.Attendees?.ToList();
                        if (attendess != null)
                        {
                            var attendeesModel = new List<AttendeeModel>();
                            foreach (var item in attendess)
                            {
                                attendeesModel.Add(new AttendeeModel(item.Value)
                                {
                                    CommonName = item.CommonName,
                                    ParticipationStatus = item.ParticipationStatus,
                                    Role = item.Role,
                                    Rsvp = item.Rsvp,
                                    Type = item.Type,
                                });
                            }
                            calendarInfo.Attendees = attendeesModel;
                        }

                        if (calendarEvent.Organizer != null)
                        {
                            var organMail = calendarEvent.Organizer.Value;
                            calendarInfo.Organizer = $"{organMail.UserInfo}@{organMail.Host}";
                        }
                        var calendarFind = maildb.Calendar.GetScheduleById(calendarId);

                        List<AttendeeModel> filterAttendees = new List<AttendeeModel>();
                        foreach (var attendee in calendarEvent.Attendees)
                        {
                            var mailUri = attendee.Value;
                            AttendeeModel newAttendee = new AttendeeModel(mailUri)
                            {
                                Type = attendee.Type,
                                Role = attendee.Role,
                                CommonName = attendee.CommonName,
                                Rsvp = true,
                                ParticipationStatus = attendee.ParticipationStatus,
                            };
                            filterAttendees.Add(newAttendee);
                        }
                        calendarInfo.Attendees = filterAttendees;
                        if (calendar.Method.Equals("REQUEST", StringComparison.OrdinalIgnoreCase))
                        {
                            calendarInfo.Status = 0;
                            maildb.Calendar.AddOrUpdate(calendarInfo);
                        }

                        if (calendar.Method.Equals("REPLY", StringComparison.OrdinalIgnoreCase))
                        {
                            var replyAttendees = new List<AttendeeModel>();
                            var attendee = calendarInfo.Attendees[0];
                            if (calendarFind != null && calendarFind.Attendees.Count > 0)
                            {
                                foreach (var existAttendee in calendarFind.Attendees)
                                {
                                    string existAttendeeAddress = AddressUtility.GetAddress(attendee?.Value?.ToString()?.Replace("mailto:", ""));
                                    string originAttendeeAddress = AddressUtility.GetAddress(existAttendee?.Value?.ToString()?.Replace("mailto:", ""));

                                    if (existAttendeeAddress.Equals(originAttendeeAddress))
                                    {
                                        existAttendee.ParticipationStatus = attendee.ParticipationStatus;
                                    }
                                    replyAttendees.Add(existAttendee);
                                }
                                calendarFind.Attendees = replyAttendees;
                                maildb.Calendar.AddOrUpdate(calendarFind);
                            }
                        }

                        if (calendar.Method.Equals("CANCEL", StringComparison.OrdinalIgnoreCase))
                        {
                            if (calendarFind != null)
                            {
                                maildb.Calendar.Delete(calendarFind);
                            }
                        }

                    }
                });
            }
        }

        private static string GetAddressNotContainName(string commonName)
        {
            if (string.IsNullOrEmpty(commonName))
                return "";
            var realAttendeeAddress = commonName.Split(' ');
            string existAttendeeAddress = string.Empty;
            if (realAttendeeAddress.Length > 1)
            {
                existAttendeeAddress = realAttendeeAddress[1].Replace("<", "").Replace(">", "");
            }
            else
            {
                existAttendeeAddress = realAttendeeAddress[0];
            }
            return existAttendeeAddress;
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
                string clienthost = session.ClientHostName != null ? session.ClientHostName : "unknown";

                string ip = session.ClientIP;

                string msgbody = session.MessageBody;
                msgbody = Kooboo.Mail.Utility.MessageUtility.CheckNFixMessage(msgbody);

                string from = GetMailFrom(session);

                var tos = GetRcptTos(session);

                if (!string.IsNullOrEmpty(msgbody) && !string.IsNullOrEmpty(from) && tos != null && tos.Count() > 0)
                {

                    if (session.TestResult != null)
                    {
                        var spamHeader = session.TestResult.ToHeaderLine();

                        msgbody = "Received: from " + clienthost + " (" + session.ClientIP + ") by " + GetHostName() + "; " + ImapHelper.DateTimeToRfc2822(DateTime.UtcNow) + "\r\n" + spamHeader + "\r\n" + msgbody;
                    }
                    else
                    {
                        msgbody = "Received: from " + clienthost + " (" + session.ClientIP + ") by " + GetHostName() + "; " + ImapHelper.DateTimeToRfc2822(DateTime.UtcNow) + "\r\n" + msgbody;
                    }
                }

                MailDb SenderDb = GetSenderMailDb(from);
                if (SenderDb != null)
                {
                    var msginfo = Kooboo.Mail.Utility.MessageUtility.ParseMeta(msgbody);
                    var user = GlobalDb.Users.Get(SenderDb.UserId);
                    if (user is not null)
                        Kooboo.Mail.Transport.Incoming.SaveSent(from, msginfo, msgbody, user);
                }

                string log = "SMTP received: " + session.ClientIP + " " + session.UserName + " " + session.Password;
                _logger.LogInformation(log);

                if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(msgbody) && tos.Count() > 0)
                {
                    await Receive(from, tos.ToList(), msgbody);
                }
            }

            session.ReSet();
        }

        private static string GetHostName()
        {
            if (Data.AppSettings.ServerSetting != null && !string.IsNullOrEmpty(Data.AppSettings.ServerSetting.HostDomain))
            {
                return Data.AppSettings.ServerSetting.HostDomain;
            }
            return Kooboo.Mail.Settings.SmtpDomain;
        }

        internal static string GetMailFrom(Smtp.SmtpSession session)
        {
            foreach (var item in session.Log)
            {
                if (item.Key.Name == Smtp.SmtpCommandName.MAILFROM && item.Value.Code == 250)
                {
                    return Utility.SmtpUtility.GetEmailFromMailFromLine(item.Key.Value);
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
