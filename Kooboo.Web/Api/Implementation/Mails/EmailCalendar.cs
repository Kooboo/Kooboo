using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Ical.Net.DataTypes;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Kooboo.Api;
using Kooboo.Data.Models;
using Kooboo.Lib;
using Kooboo.Mail;
using Kooboo.Mail.Factory;
using Kooboo.Mail.Models;
using Kooboo.Mail.Utility;
using Kooboo.Mail.ViewModel;
using MimeKit;
using static Kooboo.Mail.Utility.ICalendarUtility;
using static Kooboo.Web.Api.Implementation.Mails.EmailAddressApi;

namespace Kooboo.Web.Api.Implementation.Mails
{
    public class EmailCalendar : IApi
    {
        public string ModelName
        {
            get
            {
                return "EmailCalendar";
            }
        }

        public bool RequireSite
        {
            get
            {
                return false;
            }
        }

        public bool RequireUser
        {
            get
            {
                return true;
            }
        }

        public string CalendarReplyKey = AppSettingsUtility.Get("CalendarReplyKey", "NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwi");
        private const int AcceptReply = 1;
        private const int DeclineReply = 2;
        private const int TentativeReply = 3;


        public bool IsAvailableInviteEmailAddress(ApiCall apiCall)
        {
            if (EmailForwardManager.RequireForward(apiCall.Context))
            {
                var dic = new Dictionary<string, string>();
                dic.Add("address", apiCall.GetValue<string>("address"));

                return EmailForwardManager.Get<bool>(this.ModelName, nameof(EmailCalendar.IsAvailableInviteEmailAddress), apiCall.Context.User, dic);
            }
            var orgDb = Kooboo.Mail.Factory.DBFactory.OrgDb(apiCall.Context.User.CurrentOrgId);
            var addresses = apiCall.GetValue<List<string>>("address");

            foreach (var address in addresses)
            {
                var find = orgDb.Email.Find(address);
                if (find != null)
                {
                    return find.UserId.Equals(apiCall.Context.User.Id);
                }
            }
            return true;
        }

        public void InviteDealing(ApiCall apiCall)
        {
            if (EmailForwardManager.RequireForward(apiCall.Context))
            {
                var dic = new Dictionary<string, string>();
                dic.Add("id", apiCall.GetValue<string>("id"));
                dic.Add("sourceId", apiCall.GetValue<string>("sourceId"));
                dic.Add("inviteConfirm", apiCall.GetValue<string>("inviteConfirm"));

                var json = apiCall.Context.Request.Body;

                EmailForwardManager.Post<string>(this.ModelName, nameof(EmailCalendar.InviteDealing), apiCall.Context.User, json, dic);
                return;
            }

            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(apiCall.Context.User);
            var messageId = apiCall.GetValue<int>("id");
            if (messageId == 0)
            {
                messageId = apiCall.GetValue<int>("sourceId");
            }
            var msg = maildb.Message2.Get(messageId);
            string sender = msg.RcptTo;
            int confirmStatus = apiCall.GetIntValue("inviteConfirm");
            ICalendarViewModel calendarViewModel = Kooboo.Lib.Helper.JsonHelper.Deserialize<List<ICalendarViewModel>>(apiCall.GetValue("calendar"))[0];
            var calendarId = calendarViewModel.Uid;
            var calendarFind = maildb.Calendar.GetScheduleById(calendarId);
            if (calendarFind != null)
            {
                foreach (var item in calendarFind.Attendees)
                {
                    var senderAddress = AddressUtility.GetAddress(sender);
                    var existAddress = AddressUtility.GetAddress(item.Value.ToString().Replace("mailto:", ""));
                    if (senderAddress.Equals(existAddress))
                    {
                        switch (confirmStatus)
                        {
                            case 3:
                                item.ParticipationStatus = "TENTATIVE";
                                break;
                            case 1:
                                item.ParticipationStatus = "ACCEPTED";
                                break;
                            case 2:
                                item.ParticipationStatus = "DECLINED";
                                break;
                        }
                    }
                }
                calendarFind.Status = confirmStatus;
                maildb.Calendar.AddOrUpdate(calendarFind);
            }

            var orgdb = DBFactory.OrgDb(apiCall.Context.User.CurrentOrgId);
            var emailAddress = orgdb.Email.Find(msg.RcptTo);
            string replySender = string.Empty;
            if (emailAddress != null)
            {
                if (string.IsNullOrEmpty(emailAddress.Name))
                {
                    var originAddress = msg.RcptTo.Replace("<", "").Replace(">", "");
                    var spans = originAddress.Split("@");
                    replySender = $"\"{spans[0]}\" <{originAddress}>";
                }
                else
                {
                    var addressItemModel = AddressItemModel.FromAddress(emailAddress);
                    replySender = $"\"{addressItemModel.Name}\" <{addressItemModel.Address.Replace("<", "").Replace(">", "")}>";
                }
            }
            else
            {
                var originAddress = msg.RcptTo.Replace("<", "").Replace(">", "");
                var spans = originAddress.Split("@");
                replySender = $"\"{spans[0]}\" <{originAddress}>";
            }
            string replyMessbody = Kooboo.Mail.Multipart.ReferenceComposer.ComposeCalendarPartStatReply(messageId, msg, confirmStatus, replySender, apiCall.Context);
            List<string> rcpto = new List<string>
                {
                    msg.From
                };
            var msginfo = Kooboo.Mail.Utility.MessageUtility.ParseMeta(replyMessbody);
            string fromaddress = Mail.Utility.AddressUtility.GetAddress(msginfo.From);
            Kooboo.Mail.Transport.Incoming.SaveSent(fromaddress, msginfo, replyMessbody, apiCall.Context.User);
            Kooboo.Mail.Transport.Incoming.Receive(fromaddress, rcpto, replyMessbody);

            Mail.Message mailMessage = maildb.Message2.Get(messageId);
            mailMessage.InviteConfirm = confirmStatus;
            maildb.Message2.Update(mailMessage);
        }

        public List<ScheduleComposeEventViewModel> CalendarSchedules(ApiCall apiCall)
        {
            if (EmailForwardManager.RequireForward(apiCall.Context))
            {
                var dic = new Dictionary<string, string>();
                dic.Add("Start", apiCall.GetValue("Start"));
                dic.Add("End", apiCall.GetValue("End"));
                return EmailForwardManager.Get<List<ScheduleComposeEventViewModel>>(this.ModelName, nameof(EmailCalendar.CalendarSchedules), apiCall.Context.User, dic);
            }

            string start = apiCall.GetValue("Start");
            string end = apiCall.GetValue("End");
            var calendarInfos = new List<CalendarInfo>();
            var viewModels = new List<ScheduleComposeEventViewModel>();
            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(apiCall.Context.User);
            var orgdb = Kooboo.Mail.Factory.DBFactory.OrgDb(apiCall.Context.User.CurrentOrgId);
            if (string.IsNullOrEmpty(start))
            {
                calendarInfos = maildb.Calendar.GetAllSchedules();
            }
            else
            {
                calendarInfos = maildb.Calendar.GetSchedulesByTime(start, end, apiCall.Context.User.Id.ToString());
                calendarInfos.ForEach(calendarInfo =>
                {
                    var viewModel = new ScheduleComposeEventViewModel();
                    viewModel.ConvertByCalendarInfo(calendarInfo);
                    var find = orgdb.Email.Find(viewModel.Organizer);
                    if (find == null)
                    {
                        viewModel.IsOrganizer = false;
                    }
                    else
                    {
                        viewModel.IsOrganizer = apiCall.Context.User.Id.Equals(find.UserId);
                    }
                    viewModels.Add(viewModel);
                });
            }

            return viewModels;
        }

        public CalendarInfo GetScheduleById(ApiCall apiCall, string id)
        {
            if (EmailForwardManager.RequireForward(apiCall.Context))
            {
                var dic = new Dictionary<string, string>();
                dic.Add("id", apiCall.GetValue<string>("id"));

                return EmailForwardManager.Get<CalendarInfo>(this.ModelName, nameof(EmailCalendar.GetScheduleById), apiCall.Context.User, dic);
            }
            if (string.IsNullOrEmpty(id))
            {
                return new CalendarInfo();
            }

            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(apiCall.Context.User);
            var info = maildb.Calendar.GetScheduleById(id);
            return info;
        }

        public ScheduleComposeEventViewModel SaveSchedule(ApiCall apiCall)
        {
            if (EmailForwardManager.RequireForward(apiCall.Context))
            {
                var json = apiCall.Context.Request.Body;
                return EmailForwardManager.Post<ScheduleComposeEventViewModel>(this.ModelName, nameof(EmailCalendar.SaveSchedule), apiCall.Context.User, json);
            }

            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(apiCall.Context.User);
            ScheduleComposeEventViewModel info = Kooboo.Lib.Helper.JsonHelper.Deserialize<ScheduleComposeEventViewModel>(apiCall.Context.Request.Body);
            info.Contact.Insert(0, MailboxAddress.Parse(info.Organizer).Address);
            info.Attendees = ScheduleComposeEventViewModel.ConvertByContactToAttendee(info.Contact, info.Organizer);

            Organizer organizer = new Organizer(AddressUtility.GetAddress(info.Organizer));
            organizer.CommonName = AddressUtility.GetDisplayName(info.Organizer);

            ComposeViewModel model = new ComposeViewModel();
            model.Calendar = new ICalendarModel()
            {
                Attendees = info.Attendees,
                Start = info.Start.ToUniversalTime(),
                End = info.End.ToUniversalTime(),
                Description = info.Mark,
                Summary = info.CalendarTitle,
                Location = info.Location,
                Organizer = organizer,
            };
            var calendarInfo = model.Calendar.ConvertToCalendarInfo();
            calendarInfo.Status = 1;
            if (info.Start.ToUniversalTime() <= DateTime.UtcNow)
            {
                maildb.Calendar.Add(calendarInfo);
                return info;
            }

            if (info.Attendees.Count == 0)
            {
                maildb.Calendar.Add(calendarInfo);
                return info;
            }

            if (info.Contact == null)
            {
                return new ScheduleComposeEventViewModel();
            }
            model.From = EmailAddress.ToId(AddressUtility.GetAddress(model.Calendar.Organizer.Value.ToString().Replace("mailto:", "")));
            //model.To = info.Contact;
            // filter organizor
            info.Contact = FilterOrganizerMail(info.Organizer, info.Contact);
            model.To = info.Contact;
            model.Subject = $"Invitation: {info.CalendarTitle}";
            var calendarUid = $"{Guid.NewGuid().ToString().Replace("-", "")}";
            model.Calendar.Uid = calendarUid;
            maildb.Calendar.Add(model.Calendar.ConvertToCalendarInfo());
            //Sent record
            string msgid = Kooboo.Mail.Utility.SmtpUtility.GenerateMessageId(model.FromAddress);
            model.Html = ComposeEventInvitingHtml(model, apiCall.Context.User, info.Organizer, msgid);
            string messagebody = Kooboo.Mail.Utility.ComposeUtility.ComposeMessageBody(model, apiCall.Context.User, false, msgid);
            string msgSaveSent = Kooboo.Mail.Utility.ComposeUtility.ComposeMessageBody(model, apiCall.Context.User, true, msgid);
            var msginfo = Kooboo.Mail.Utility.MessageUtility.ParseMeta(msgSaveSent);
            Kooboo.Mail.Transport.Incoming.SaveSent(info.Organizer, msginfo, msgSaveSent, apiCall.Context.User);
            if (model.To.Count > 0)
            {
                foreach (var item in model.To)
                {
                    var orgdb = Kooboo.Mail.Factory.DBFactory.OrgDb(apiCall.Context.User.CurrentOrgId);
                    var addressBook = maildb.AddBook.GetInfoByAddress(item);

                    model.Html = ComposeEventInvitingHtml(model, apiCall.Context.User, item, msgid);
                    messagebody = Kooboo.Mail.Utility.ComposeUtility.ComposeMessageBody(model, apiCall.Context.User, false, msgid);
                    var tempTo = new List<string>()
                    {
                        item
                    };
                    NotifyBySendMail(info.IsNotify, info.Organizer, msgSaveSent, messagebody, tempTo, apiCall.Context.User);

                    if (addressBook == null)
                    {
                        maildb.AddBook.Add(item);
                    }
                }
            }

            return info;
        }

        public CalendarInfo EditSchedule(ApiCall apiCall)
        {
            if (EmailForwardManager.RequireForward(apiCall.Context))
            {
                var dic = new Dictionary<string, string>();
                dic.Add("organizer", apiCall.GetValue("organizer"));

                var json = apiCall.Context.Request.Body;
                return EmailForwardManager.Post<CalendarInfo>(this.ModelName, nameof(EmailCalendar.EditSchedule), apiCall.Context.User, json, dic);
            }

            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(apiCall.Context.User);
            var info = Kooboo.Lib.Helper.JsonHelper.Deserialize<ScheduleComposeEventViewModel>(apiCall.Context.Request.Body);
            var fromOrganizer = apiCall.GetValue<string>("organizer");
            var originCalendar = maildb.Calendar.GetScheduleById(info.Id);

            var calendarInfo = ScheduleComposeEventViewModel.ConvertToCalendarInfo(info);
            if (info != null)
            {
                var addContact = ScheduleComposeEventViewModel.ConvertByContactToAttendee(info.AddContact, fromOrganizer);
                var resultContact = new List<AttendeeModel>();
                if (info.Contact != null)
                {
                    if (info.RemoveContact.Count > 0 && originCalendar.Attendees != null)
                    {
                        originCalendar.Attendees.ForEach(v =>
                        {
                            var existOriginAddress = AddressUtility.GetAddress(v.Value.ToString().Replace("mailto:", ""));
                            if (!info.RemoveContact.Contains(existOriginAddress, new MailAddressEqualityComparer()))
                            {
                                resultContact.Add(v);
                            }
                        });
                        resultContact.AddRange(addContact);
                        calendarInfo.Attendees = resultContact;
                    }
                    else
                    {
                        resultContact.AddRange(originCalendar.Attendees);
                        resultContact.AddRange(addContact);
                        calendarInfo.Attendees = resultContact;
                    }
                }

                if (!(resultContact[0]!.Value.Equals(originCalendar.Attendees[0]!.Value)))
                {
                    resultContact.Insert(0, originCalendar.Attendees[0]!);
                }
            }
            if (calendarInfo.Start <= DateTime.UtcNow)
            {
                return calendarInfo;
            }
            List<AttendeeModel> originAttendees = originCalendar.Attendees;
            if (info.RemoveContact.Count > 0)
            {
                var attendees = ScheduleComposeEventViewModel.ConvertByContactToAttendee(info.RemoveContact, originCalendar.Organizer);
                SendChangeMailToAttendees(info, fromOrganizer, apiCall.Context.User, 2, attendees);

                for (int i = 0; i < originAttendees.Count; i++)
                {
                    var orginAddress = originAttendees[i].CommonName;
                    if (info.RemoveContact.Contains(orginAddress))
                    {
                        originAttendees.Remove(originAttendees[i]);
                    }
                }
            }

            if (info != null)
            {
                maildb.Calendar.AddOrUpdate(calendarInfo);
            }

            if (info.AddContact.Count == 0 && info.RemoveContact.Count == 0 && originAttendees.Count == 0)
            {
                return calendarInfo;
            }

            if (CheckCalendarContainerChange(info, originCalendar))
            {
                SendChangeMailToAttendees(info, fromOrganizer, apiCall.Context.User, 1, originAttendees);
            }

            if (info.AddContact.Count > 0)
            {
                foreach (var item in info.AddContact)
                {
                    var addressBook = maildb.AddBook.GetInfoByAddress(item);

                    if (addressBook == null)
                    {
                        maildb.AddBook.Add(item);
                    }
                }

                var attendees = ScheduleComposeEventViewModel.ConvertByContactToAttendee(info.AddContact, originCalendar.Organizer);

                SendChangeMailToAttendees(info, fromOrganizer, apiCall.Context.User, 0, attendees);
            }
            return calendarInfo;
        }

        private bool CheckCalendarContainerChange(ScheduleComposeEventViewModel info, CalendarInfo calendarInfo)
        {
            if (!Kooboo.Lib.Helper.StringHelper.IsSameValue(info.CalendarTitle, calendarInfo.CalendarTitle))
            {
                return true;
            }

            if (!Kooboo.Lib.Helper.StringHelper.IsSameValue(info.Location, calendarInfo.Location))
            {
                return true;
            }

            if (!DateTime.Equals(info.Start.ToUniversalTime(), calendarInfo.Start))
            {
                return true;
            }

            if (!DateTime.Equals(info.End.ToUniversalTime(), calendarInfo.End))
            {
                return true;
            }

            return CompareCalendarDescriptionIsSameValue(info.Mark, calendarInfo.Mark);
        }

        private bool CompareCalendarDescriptionIsSameValue(string infoMark, string calendarInfoMark)
        {
            var removeHtmlInfoMark = Kooboo.Search.Utility.RemoveHtml(infoMark);
            var removeHtmlCalendarInfoMark = Kooboo.Search.Utility.RemoveHtml(calendarInfoMark);

            return !Kooboo.Lib.Helper.StringHelper.IsSameValue(removeHtmlInfoMark, removeHtmlCalendarInfoMark);
        }

        private void SendChangeMailToAttendees(ScheduleComposeEventViewModel info, string fromOrganizer, User user, int isNewOrUpdateOrCancel, List<AttendeeModel> changeAttendees = null)
        {
            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(user);
            var calendarInfo = maildb.Calendar.GetScheduleById(info.Id);

            List<AttendeeModel> attendees = new List<AttendeeModel>();
            if (changeAttendees is null)
            {
                attendees = calendarInfo.Attendees;
            }
            else
            {
                attendees = changeAttendees;
            }

            if (attendees != null)
            {
                ICalendarModel calendarModel = new ICalendarModel();

                List<string> notifyRcptTo = new List<string>();
                attendees.ForEach(v =>
                {
                    var address = AddressUtility.GetAddress(v.Value.ToString().Replace("mailto:", ""));
                    var name = v.CommonName;
                    notifyRcptTo.Add($"{name} <{address}>");
                });

                ComposeViewModel model = new ComposeViewModel();
                model.From = EmailAddress.ToId(fromOrganizer);
                model.To = notifyRcptTo;

                string messagebody = string.Empty;
                string msgSaveSent = string.Empty;
                string msgid = Kooboo.Mail.Utility.SmtpUtility.GenerateMessageId(model.FromAddress);

                model.Calendar = calendarModel.ConvertByCalendarInfo(calendarInfo);
                model.Calendar.Organizer = new Organizer(fromOrganizer)
                {
                    CommonName = fromOrganizer
                };

                model.Calendar.Start = model.Calendar.Start.ToLocalTime();
                model.Calendar.End = model.Calendar.End.ToLocalTime();
                //filter organizor sent record
                model.To = FilterOrganizerMail(info.Organizer, model.To);

                // if not partition don't send
                if (model.To.Count() == 0)
                {
                    return;
                }

                switch (isNewOrUpdateOrCancel)
                {
                    case 0:
                        // new invitation
                        model.Subject = $"Invitation: {info.CalendarTitle}";
                        model.Calendar.Attendees = calendarInfo.Attendees;
                        model.Html = ComposeEventInvitingHtml(model, user, info.Organizer, msgid);
                        msgSaveSent = Kooboo.Mail.Utility.ComposeUtility.ComposeMessageBody(model, user, true, msgid);
                        var msginfo = Kooboo.Mail.Utility.MessageUtility.ParseMeta(msgSaveSent);
                        Kooboo.Mail.Transport.Incoming.SaveSent(info.Organizer, msginfo, msgSaveSent, user);
                        model.To.ForEach(v =>
                        {
                            model.Html = ComposeEventInvitingHtml(model, user, v, msgid);
                            messagebody = Kooboo.Mail.Utility.ComposeUtility.ComposeMessageBody(model, user, false, msgid);
                            msgSaveSent = Kooboo.Mail.Utility.ComposeUtility.ComposeMessageBody(model, user, true, msgid);
                            var tempTo = new List<string>()
                            {
                                v
                            };

                            NotifyBySendMail(info.IsNotify, fromOrganizer, msgSaveSent, messagebody, tempTo, user);
                        });
                        break;
                    case 1:
                        //update invitation
                        model.Subject = $"Updated Invitation: {info.CalendarTitle}";
                        model.Html = ComposeEventUpdateHtml(model, user, info.Organizer, msgid);
                        msgSaveSent = Kooboo.Mail.Utility.ComposeUtility.ComposeUpdateOrCancelEventBody(model, user, true, msgid, false);
                        var editMsginfo = Kooboo.Mail.Utility.MessageUtility.ParseMeta(msgSaveSent);
                        Kooboo.Mail.Transport.Incoming.SaveSent(info.Organizer, editMsginfo, msgSaveSent, user);
                        model.To.ForEach(v =>
                        {
                            model.Html = ComposeEventUpdateHtml(model, user, v, msgid);
                            messagebody = Kooboo.Mail.Utility.ComposeUtility.ComposeUpdateOrCancelEventBody(model, user, false, msgid, false);
                            msgSaveSent = Kooboo.Mail.Utility.ComposeUtility.ComposeUpdateOrCancelEventBody(model, user, true, msgid, false);
                            var tempTo = new List<string>()
                            {
                                v
                            };

                            NotifyBySendMail(info.IsNotify, fromOrganizer, msgSaveSent, messagebody, tempTo, user);
                        });
                        break;
                    case 2:
                        //cancel invitation
                        model.Subject = $"Canceled: {calendarInfo.CalendarTitle}";
                        model.Html = ComposeEventCancelHtml(model, user, info.Organizer, msgid);
                        msgSaveSent = Kooboo.Mail.Utility.ComposeUtility.ComposeUpdateOrCancelEventBody(model, user, true, msgid, true);
                        var cancelMsginfo = Kooboo.Mail.Utility.MessageUtility.ParseMeta(msgSaveSent);
                        Kooboo.Mail.Transport.Incoming.SaveSent(info.Organizer, cancelMsginfo, msgSaveSent, user);
                        model.To.ForEach(v =>
                        {
                            model.Html = ComposeEventCancelHtml(model, user, v, msgid);
                            messagebody = Kooboo.Mail.Utility.ComposeUtility.ComposeUpdateOrCancelEventBody(model, user, false, msgid, true);
                            msgSaveSent = Kooboo.Mail.Utility.ComposeUtility.ComposeUpdateOrCancelEventBody(model, user, true, msgid, true);
                            var tempTo = new List<string>()
                            {
                                v
                            };

                            NotifyBySendMail(info.IsNotify, fromOrganizer, msgSaveSent, messagebody, tempTo, user);
                        });
                        break;
                }
            }
        }

        private List<string> FilterOrganizerMail(string fromOrganizer, List<string> rcptTos)
        {
            var originFrom = MailboxAddress.Parse(fromOrganizer).Address!;
            string target = string.Empty;
            rcptTos.ForEach(v =>
            {
                if (originFrom.Equals(MailboxAddress.Parse(v).Address!))
                {
                    target = v;
                }
            });
            if (!string.IsNullOrEmpty(target))
            {
                rcptTos.Remove(target);
            }
            return rcptTos;
        }

        public CalendarInfo DeleteSchedule(ApiCall apiCall)
        {
            if (EmailForwardManager.RequireForward(apiCall.Context))
            {
                var dic = new Dictionary<string, string>();
                dic.Add("organizer", apiCall.GetValue("organizer"));

                var json = apiCall.Context.Request.Body;
                return EmailForwardManager.Post<CalendarInfo>(this.ModelName, nameof(EmailCalendar.DeleteSchedule), apiCall.Context.User, json, dic);
            }
            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(apiCall.Context.User);
            var orgdb = Kooboo.Mail.Factory.DBFactory.OrgDb(apiCall.Context.User.CurrentOrgId);
            var info = Kooboo.Lib.Helper.JsonHelper.Deserialize<ScheduleComposeEventViewModel>(apiCall.Context.Request.Body);
            var fromOrganizer = apiCall.GetValue<string>("organizer");
            var calendarEvent = maildb.Calendar.GetScheduleById(info.Id);

            var orginEmail = orgdb.Email.Find(calendarEvent.Organizer);

            if (orginEmail != null)
            {
                var isOrgin = orginEmail.UserId.Equals(apiCall.Context.User.Id);
                if (calendarEvent.Attendees.Count > 0 && isOrgin)
                {
                    if (calendarEvent.Start > DateTime.UtcNow)
                    {
                        SendChangeMailToAttendees(info, fromOrganizer, apiCall.Context.User, 2);
                    }
                }
            }


            maildb.Calendar.Delete(calendarEvent);
            return calendarEvent;
        }

        private string UriConvertToMailAddress(Uri uri)
        {
            string attendeeAddress = string.Empty;
            if (uri != null)
            {
                attendeeAddress = $"{uri.UserInfo!}@{uri.Host!}";
            }
            else
            {
                attendeeAddress = uri.AbsoluteUri?.Replace("mailto:", "", StringComparison.OrdinalIgnoreCase);
            }

            return attendeeAddress;
        }

        private string ComposeEventInvitingHtml(ComposeViewModel model, User user, string toAddress, string smtpMessageId)
        {
            StringBuilder sbTo = new StringBuilder();
            var organAddress = UriConvertToMailAddress(model.Calendar.Organizer.Value);
            model.Calendar.Attendees.ForEach(v =>
            {
                var attendeeAddress = UriConvertToMailAddress(v.Value);
                if (!String.Equals(organAddress, attendeeAddress))
                {
                    sbTo.Append($"{attendeeAddress}<br>");
                }
            });

            string showDateTime = string.Empty;
            if (model.Calendar.Start.ToLocalTime().Day != model.Calendar.End.ToLocalTime().Day)
            {
                DateTime start = model.Calendar.Start.ToLocalTime();
                DateTime end = model.Calendar.End.ToLocalTime();
                string dayOfWeek = start.DayOfWeek.ToString();
                string endDayOfWeek = end.DayOfWeek.ToString();
                showDateTime = $"{start.ToString("yyyy MM-dd")} ({dayOfWeek}) {start.ToString("HH:mm")} — {end.ToString("yyyy MM-dd")} ({endDayOfWeek}) {end.ToString("HH:mm")}";
            }
            else
            {
                DateTime start = model.Calendar.Start.ToLocalTime();
                DateTime end = model.Calendar.End.ToLocalTime();
                string dayOfWeek = start.DayOfWeek.ToString();
                showDateTime = $"{start.ToString("yyyy MM-dd")} ({dayOfWeek}) {start.ToString("HH:mm")} — {end.ToString("HH:mm")}";
            }

            string content = $"<!DOCTYPE html>\r\n<html lang=\"en\">\r\n\r\n<head>\r\n    <meta http-equiv='Content-Type' content='text/html'>\r\n    <meta charset=\"UTF-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n    <title>Email Inviting</title>\r\n</head>\r\n<style>\r\n    a{{\r\n    text-decoration:none;\r\n    color:#999;}}\r\n    a:hover{{\r\n    text-decoration:underline;\r\n    }}\r\n    .first-title {{\r\n        font-size: 22px;\r\n        font-weight: bolder;\r\n    }}\r\n\r\n    .second-title {{\r\n        font-size: 18px;\r\n        font-weight: bolder;\r\n    }}\r\n\r\n    .content {{\r\n        font-size: 16px;\r\n    }}\r\n\r\n    .content-small{{\r\n        font-size: 14px;\r\n    }}\r\n\r\n    .font-title {{\r\n        font-weight: bolder;\r\n    }}\r\n\r\n    .font-black-normal {{\r\n        color: black;\r\n    }}\r\n\r\n    .font-gray {{\r\n        color: rgb(128, 132, 135)\r\n    }}\r\n\r\n    .container {{\r\n        padding: 20px 10px 40px 30px;\r\n        height: 100%;\r\n        width: auto;\r\n        border: 1px solid rgb(177, 181, 184);\r\n        border-radius: 10px;\r\n    }}\r\n\r\n    .footer{{\r\n        padding: 10px 0px 0px 20px;\r\n    }}\r\n\r\n.button-container a {{ border-radius: 24px;\r\ndisplay: inline-block;\r\nmargin: 10px 10px 0 0;\r\npadding: 8px 20px;\r\nborder: 1px solid #cecece;\r\ncolor: inherit;\r\ntext-decoration: none;\r\nline-height: 1;\r\n}}\r\n\r\n.button-container a:hover {{background-color: #c4e3fc4c;\r\ntext-decoration: none;\r\nborder: 1px solid #c4e3fc4c;\r\ntransition: 0.3s;\r\n}}\r\n\r\n .replyStatus:hover{{\r\n            color: white;\r\n        }}</style>\r\n\r\n<body>\r\n    <div class=\"container\">\r\n   <div style=\"display:none\">{model.Calendar.Summary}</div>       <table border=\"0\" style=\"padding-left:0px;padding-right:0px;\">\r\n            <tr>\r\n                <td align=\"left\">\r\n";
            if (!string.IsNullOrEmpty(model.Calendar.Description))
            {
                content += $"                <div class=\"second-title\">Description</div >\r\n                    <div>{model.Calendar.Description}</div>\r\n     <br>";
            }
            content += $"                    <div class=\"second-title\">When</div>\r\n                    <div class=\"content \">\r\n                        {showDateTime}\r\n                    </div>\r\n<br>";
            if (!string.IsNullOrEmpty(model.Calendar.Location))
            {
                content += $"   <div class=\"second-title\">Location</div >\r\n                    <div>{model.Calendar.Location}</div>\r\n       <br>";
            }

            content += $"                   <div class=\"second-title\">Participants</div>\r\n                    <div>{model.Calendar.Organizer.Value.ToString().Replace("mailto:", "")} - <t class=\"font-gray\">organizer</t></div>\r\n                    <div>{sbTo.ToString()}</div>\r\n  </td>\r\n            </tr>\r\n\r\n<tr class=\"hide-reply-calendar-button\">\r\n                    <td>\r\n                        <div class=\"second-title\"><br>Reply for {MailboxAddress.Parse(toAddress).Address}</div>\r\n                        <div>\r\n                        <div class=\"button-container\">\r\n                            <a href=\"{ApiHelper.MakeUrl(GenerateTokenToReplyCalendar(user, model.Calendar.Uid, toAddress, AcceptReply, smtpMessageId))}\">Yes</a>\r\n                 <a href=\"{ApiHelper.MakeUrl(GenerateTokenToReplyCalendar(user, model.Calendar.Uid, toAddress, DeclineReply, smtpMessageId))}\">No</a>\r\n                        <a href=\"{ApiHelper.MakeUrl(GenerateTokenToReplyCalendar(user, model.Calendar.Uid, toAddress, TentativeReply, smtpMessageId))}\">Maybe</a>\r\n                       </div>                    </td>\r\n                </tr>        </table>\r\n    </div>\r\n    <div class=\"footer\">\r\n        <p class=\"font-gray content-small\">Invitation from Kooboo Calendar</p >\r\n        </div>\r\n\r\n</body>\r\n\r\n</html>\r\n";

            return content;
        }

        private string ComposeEventCancelHtml(ComposeViewModel model, User user, string toAddress, string smtpMessageId)
        {
            StringBuilder sbTo = new StringBuilder();
            model.Calendar.Attendees.ForEach(v =>
            {
                string[] toAddress = v.CommonName.Split(' ');
                if (toAddress.Length < 2)
                {
                    string address = v.Value.ToString().Replace("mailto:", "").Replace("<", "").Replace(">", "");
                    sbTo.Append($"{address}<br>");
                }
                else
                {
                    string address = toAddress[1].Replace("<", "").Replace(">", "");
                    sbTo.Append($"{address}<br>");
                }
            });

            string showDateTime = string.Empty;
            if (model.Calendar.Start.ToLocalTime().Day != model.Calendar.End.ToLocalTime().Day)
            {
                DateTime start = model.Calendar.Start.ToLocalTime();
                DateTime end = model.Calendar.End.ToLocalTime();
                string dayOfWeek = start.DayOfWeek.ToString();
                string endDayOfWeek = end.DayOfWeek.ToString();
                showDateTime = $"{start.ToString("yyyy MM-dd")} ({dayOfWeek}) {start.ToString("HH:mm")} — {end.ToString("yyyy MM-dd")} ({endDayOfWeek}) {end.ToString("HH:mm")}";
            }
            else
            {
                DateTime start = model.Calendar.Start.ToLocalTime();
                DateTime end = model.Calendar.End.ToLocalTime();
                string dayOfWeek = start.DayOfWeek.ToString();
                showDateTime = $"{start.ToString("yyyy MM-dd")} ({dayOfWeek}) {start.ToString("HH:mm")} — {end.ToString("HH:mm")}";
            }

            string content = $"<!DOCTYPE html>\r\n<html lang=\"en\">\r\n\r\n<head>\r\n    <meta http-equiv='Content-Type' content='text/html'>\r\n    <meta charset=\"UTF-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n    <title>Email Inviting</title>\r\n</head>\r\n<style>\r\n li{{\r\n            list-style: none;\r\n        }}\r\n\r\n        .aButton {{        border: 1px solid #cecece;\r\n        display: inline-block;\r\n        margin-top: 10px;\r\n        margin-right: 10px;\r\n        border-radius: 18px;\r\n        height: 35px;\r\n        width: 55px;\r\n        text-align: center;\r\n        text-decoration: none;\r\n        padding: 0 10px;\r\n        outline: none;\r\n        color: inherit;\r\n    }}\r\n\r\n    .aButton:hover {{\r\n        background-color: #c4e3fc4c;\r\n        border: 1px solid #c4e3fc4c;\r\n        transition: 0.3s;\r\n    }}\r\n        .replyStatus:hover{{\r\n            color: white;\r\n        }}\r\n    .first-title {{\r\n        font-size: 22px;\r\n        font-weight: bolder;\r\n    }}\r\n\r\n         a{{\r\n    text-decoration:none;\r\n    color:#999;}}\r\n    a:hover{{\r\n    text-decoration:underline;\r\n    }}\r\n    .first-title {{\r\n        font-size: 22px;\r\n        font-weight: bolder;\r\n    }}\r\n\r\n    .second-title {{\r\n        font-size: 18px;\r\n        font-weight: bolder;\r\n    }}\r\n\r\n    .content {{\r\n        font-size: 16px;\r\n    }}\r\n\r\n    .content-small{{\r\n        font-size: 14px;\r\n    }}\r\n\r\n    .font-title {{\r\n        font-weight: bolder;\r\n    }}\r\n\r\n    .font-black-normal {{\r\n        color: black;\r\n    }}\r\n\r\n    .font-gray {{\r\n        color: rgb(128, 132, 135)\r\n    }}\r\n\r\n    .container {{\r\n        padding: 20px 10px 40px 30px;\r\n        height: 100%;\r\n        width: auto;\r\n        border: 1px solid rgb(177, 181, 184);\r\n        border-radius: 10px;\r\n    }}\r\n\r\n    .footer{{\r\n        padding: 10px 0px 0px 20px;\r\n    }}\r\n    .font-delete{{    font-weight: bolder;\r\n    }}\r\n    .delete-container{{    background-color: rgb(244,233,231);\r\n    padding: 10px 10px 10px 30px;\r\n    border-radius: 10px;\r\n    line-height: 1.5;\r\n    margin-top: 10px;\r\n    margin-bottom: 10px;\r\n}}</style>\r\n\r\n<body>\r\n  <div style=\"display:none\">{model.Calendar.Summary}</div>    <div class=\"delete-container\">\r\n        <div class=\"content font-delete\" style=\"color: black;\">This schedule has been canceled</div>\r\n    </div>\r\n<div class=\"container\">\r\n        <table border=\"0\" style=\"padding-left:0px;padding-right:0px;\">\r\n            <tr>\r\n                <td align=\"left\">\r\n";
            if (!string.IsNullOrEmpty(model.Calendar.Description))
            {
                content += $"                <div class=\"second-title\">Description</div >\r\n                    <div>{model.Calendar.Description}</div>\r\n     <br>";
            }
            content += $"                    <div class=\"second-title\">When</div>\r\n                    <div class=\"content \">\r\n                        {showDateTime}\r\n                    </div>\r\n           <br>";
            if (!string.IsNullOrEmpty(model.Calendar.Location))
            {
                content += $"   <div class=\"second-title\">Location</div >\r\n                    <div>{model.Calendar.Location}</div>\r\n       <br>";
            }

            content += $"                   <div class=\"second-title\">Participants</div>\r\n                    <div>{model.Calendar.Organizer.Value.ToString().Replace("mailto:", "")} - <t class=\"font-gray\">organizer</t></div>\r\n                    <div></div>\r\n  </td>\r\n            </tr>\r\n        </table>\r\n    </div>\r\n    <div class=\"footer\">\r\n        <p class=\"font-gray content-small\">Invitation from Kooboo Calendar</p >\r\n </div>\r\n\r\n</body>\r\n\r\n</html>\r\n";
            return content;
        }

        private string ComposeEventUpdateHtml(ComposeViewModel model, User user, string toAddress, string smtpMessageId)
        {
            StringBuilder sbTo = new StringBuilder();
            var organAddress = UriConvertToMailAddress(model.Calendar.Organizer.Value);
            model.Calendar.Attendees.ForEach(v =>
            {
                var attendeeAddress = UriConvertToMailAddress(v.Value);
                if (!String.Equals(organAddress, attendeeAddress))
                {
                    sbTo.Append($"{attendeeAddress}<br>");
                }
            });

            string showDateTime = string.Empty;
            if (model.Calendar.Start.ToLocalTime().Day != model.Calendar.End.ToLocalTime().Day)
            {
                DateTime start = model.Calendar.Start.ToLocalTime();
                DateTime end = model.Calendar.End.ToLocalTime();
                string dayOfWeek = start.DayOfWeek.ToString();
                string endDayOfWeek = end.DayOfWeek.ToString();
                showDateTime = $"{start.ToString("yyyy MM-dd")} ({dayOfWeek}) {start.ToString("HH:mm")} — {end.ToString("yyyy MM-dd")} ({endDayOfWeek}) {end.ToString("HH:mm")}";
            }
            else
            {
                DateTime start = model.Calendar.Start.ToLocalTime();
                DateTime end = model.Calendar.End.ToLocalTime();
                string dayOfWeek = start.DayOfWeek.ToString();
                showDateTime = $"{start.ToString("yyyy MM-dd")} ({dayOfWeek}) {start.ToString("HH:mm")} — {end.ToString("HH:mm")}";
            }

            string content = $"<!DOCTYPE html>\r\n<html lang=\"en\">\r\n\r\n<head>\r\n    <meta http-equiv='Content-Type' content='text/html'>\r\n    <meta charset=\"UTF-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n    <title>Email Inviting</title>\r\n</head>\r\n<style>\r\n    a{{\r\n    text-decoration:none;\r\n    color:#999;}}\r\n    a:hover{{\r\n    text-decoration:underline;\r\n    }}\r\n    .first-title {{\r\n        font-size: 22px;\r\n        font-weight: bolder;\r\n    }}\r\n\r\n    .second-title {{\r\n        font-size: 18px;\r\n        font-weight: bolder;\r\n    }}\r\n\r\n    .content {{\r\n        font-size: 16px;\r\n    }}\r\n\r\n    .content-small{{\r\n        font-size: 14px;\r\n    }}\r\n\r\n    .font-title {{\r\n        font-weight: bolder;\r\n    }}\r\n\r\n    .font-black-normal {{\r\n        color: black;\r\n    }}\r\n\r\n    .font-gray {{\r\n        color: rgb(128, 132, 135)\r\n    }}\r\n\r\n    .container {{\r\n        padding: 20px 10px 40px 30px;\r\n        height: 100%;\r\n        width: auto;\r\n        border: 1px solid rgb(177, 181, 184);\r\n        border-radius: 10px;\r\n    }}\r\n\r\n    .footer{{\r\n        padding: 10px 0px 0px 20px;\r\n    }}\r\n\r\n.button-container a {{ border-radius: 24px;\r\ndisplay: inline-block;\r\nmargin: 10px 10px 0 0;\r\npadding: 8px 20px;\r\nborder: 1px solid #cecece;\r\ncolor: inherit;\r\ntext-decoration: none;\r\nline-height: 1;\r\n}}\r\n\r\n.button-container a:hover {{background-color: #c4e3fc4c;\r\ntext-decoration: none;\r\nborder: 1px solid #c4e3fc4c;\r\ntransition: 0.3s;\r\n}}\r\n\r\n .replyStatus:hover{{\r\n            color: white;\r\n        }}</style>\r\n\r\n<body>\r\n    <div class=\"container\">\r\n   <div style=\"display:none\">{model.Calendar.Summary}</div>       <table border=\"0\" style=\"padding-left:0px;padding-right:0px;\">\r\n            <tr>\r\n                <td align=\"left\">\r\n";
            if (!string.IsNullOrEmpty(model.Calendar.Description))
            {
                content += $"                <div class=\"second-title\">Description</div >\r\n                    <div>{model.Calendar.Description}</div>\r\n     <br>";
            }
            content += $"                    <div class=\"second-title\">When</div>\r\n                    <div class=\"content \">\r\n                        {showDateTime}\r\n                    </div>\r\n <br>";
            if (!string.IsNullOrEmpty(model.Calendar.Location))
            {
                content += $"   <div class=\"second-title\">Location</div >\r\n                    <div>{model.Calendar.Location}</div>\r\n       <br>";
            }

            content += $"                   <div class=\"second-title\">Participants</div>\r\n                    <div>{model.Calendar.Organizer.Value.ToString().Replace("mailto:", "")} - <t class=\"font-gray\">organizer</t></div>\r\n                    <div>{sbTo.ToString()}</div>\r\n  </td>\r\n            </tr>\r\n\r\n<tr class=\"hide-reply-calendar-button\">\r\n                    <td>\r\n                        <div class=\"second-title\"><br>Reply for {MailboxAddress.Parse(toAddress).Address}</div>\r\n                        <div>\r\n                        <div class=\"button-container\">\r\n                            <a href=\"{ApiHelper.MakeUrl(GenerateTokenToReplyCalendar(user, model.Calendar.Uid, toAddress, AcceptReply, smtpMessageId))}\">Yes</a>\r\n                 <a href=\"{ApiHelper.MakeUrl(GenerateTokenToReplyCalendar(user, model.Calendar.Uid, toAddress, DeclineReply, smtpMessageId))}\">No</a>\r\n                        <a href=\"{ApiHelper.MakeUrl(GenerateTokenToReplyCalendar(user, model.Calendar.Uid, toAddress, TentativeReply, smtpMessageId))}\">Maybe</a>\r\n                       </div>                    </td>\r\n                </tr>        </table>\r\n    </div>\r\n    <div class=\"footer\">\r\n        <p class=\"font-gray content-small\">Invitation from Kooboo Calendar</p >\r\n        </div>\r\n\r\n</body>\r\n\r\n</html>\r\n";
            return content;

        }

        private string GenerateTokenToReplyCalendar(User user, string calendarId, string replyAddress, int replyStatus, string smtpMessageId)
        {
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            var mailBodyReplyModel = new MailBodyReplyModel()
            {
                User = user,
                CalendarId = calendarId,
                ReplyAddress = replyAddress,
                ReplyStatus = replyStatus,
                MessageId = smtpMessageId
            };

            string token = encoder.Encode(mailBodyReplyModel, CalendarReplyKey);
            return $"/_api/v2/EmailCalendarReply/MailBodyDealingCalendar?token={token}";
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

        private void NotifyBySendMail(bool isNotify, string fromOrganizer, string msgSaveSent, string messagebody, List<string> notifyRcptTo, User user)
        {
            if (isNotify)
            {
                var msginfo = Kooboo.Mail.Utility.MessageUtility.ParseMeta(msgSaveSent);
                msginfo.From = ComposeUtility.GetFrom(new ComposeViewModel() { From = EmailAddress.ToId(fromOrganizer) }, user);

                // save sent.. 
                // Kooboo.Mail.Transport.Incoming.SaveSent(fromOrganizer, msginfo, msgSaveSent, user);
                msginfo.Bcc = null;
                msginfo.Read = false;
                msginfo.Deleted = false;
                notifyRcptTo = FilterOrganizerMail(fromOrganizer, notifyRcptTo);

                Kooboo.Mail.Transport.Incoming.Receive(fromOrganizer, notifyRcptTo, messagebody, msginfo);
            }

        }

        class MailAddressEqualityComparer : IEqualityComparer<string>
        {
            public new bool Equals(string x, string y)
            {
                return AddressUtility.GetAddress(x).Equals(AddressUtility.GetAddress(y));
            }

            public int GetHashCode(string obj)
            {
                return obj.GetHashCode();
            }

        }
    }

    public class ScheduleComposeEventViewModel
    {
        public string Id { get; set; }
        public string CalendarTitle { get; set; }
        public string Organizer { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public List<AttendeeModel> Attendees { get; set; }
        public string Location { get; set; }
        public string Mark { get; set; }
        public int Status { get; set; }
        public List<string> Contact { get; set; }
        public List<string> AddContact { get; set; }
        public List<string> RemoveContact { get; set; }
        public bool IsNotify { get; set; }
        public bool IsOrganizer { get; set; }
        public int TotalCount { get; set; }
        public int AwaitingCount { get; set; }
        public int AcceptCount { get; set; }
        public int DeclineCount { get; set; }
        public int TentativeCount { get; set; }
        public List<Dictionary<string, object>> AttendeeStatus { get; set; }

        public ScheduleComposeEventViewModel ConvertByCalendarInfo(CalendarInfo calendarInfo)
        {
            Id = calendarInfo.Id;
            CalendarTitle = calendarInfo.CalendarTitle;
            Start = calendarInfo.Start.ToLocalTime();
            End = calendarInfo.End.ToLocalTime();
            Organizer = calendarInfo.Organizer;
            Contact = new List<string>();
            AcceptCount = 1;
            TotalCount = 1;
            var organizerAddress = AddressUtility.GetAddress(calendarInfo.Organizer);
            if (calendarInfo.Attendees != null)
            {
                AttendeeStatus = new List<Dictionary<string, object>>();
                calendarInfo.Attendees.ForEach(v =>
                {
                    string attendeeAddress = UriConvertToMailAddress(v);
                    if (organizerAddress != null && !organizerAddress.Equals(AddressUtility.GetAddress(attendeeAddress)))
                    {
                        switch (v.ParticipationStatus)
                        {
                            case "TENTATIVE":
                                var tentativeDic = new Dictionary<string, object>
                                {
                                    { "Address", attendeeAddress },
                                    { "ParticipationStatus", 0 }
                                };
                                AttendeeStatus.Add(tentativeDic);
                                TentativeCount++;
                                break;
                            case "ACCEPTED":
                                var acceptDic = new Dictionary<string, object>
                                {
                                    { "Address", attendeeAddress },
                                    { "ParticipationStatus", 1 }
                                };
                                AttendeeStatus.Add(acceptDic);
                                AcceptCount++;
                                break;
                            case "DECLINED":
                                var declinedDic = new Dictionary<string, object>
                                {
                                    { "Address", attendeeAddress },
                                    { "ParticipationStatus", 2 }
                                };
                                AttendeeStatus.Add(declinedDic);
                                DeclineCount++;
                                break;
                            default:
                                var awaitingDic = new Dictionary<string, object>
                                {
                                    { "Address", attendeeAddress },
                                    { "ParticipationStatus", -1 }
                                };
                                AttendeeStatus.Add(awaitingDic);
                                AwaitingCount++;
                                break;
                        }
                        TotalCount++;
                    }
                });
            }

            Location = calendarInfo.Location;
            bool isContain = ContainATag(calendarInfo.Mark);
            if (!string.IsNullOrEmpty(calendarInfo.Mark) && !isContain)
            {
                Mark = MatchUriConvertToHtml(calendarInfo.Mark);
            }
            else
            {
                Mark = calendarInfo.Mark;
            }

            Status = calendarInfo.Status;
            return this;
        }

        public static List<AttendeeModel> ConvertByContactToAttendee(List<string> contacts, string organizer, bool isRemove = false)
        {
            List<AttendeeModel> attendees = new List<AttendeeModel>();
            if (contacts != null)
            {
                contacts.ForEach(v =>
                {
                    AttendeeModel attendee = new AttendeeModel($"mailto:{AddressUtility.GetAddress(v)}")
                    {
                        Type = "INDIVIDUAL",
                        Role = "REQ-PARTICIPANT",
                        CommonName = AddressUtility.GetDisplayName(v),
                        Rsvp = true,
                    };

                    if (isRemove)
                    {
                        attendee.ParticipationStatus = "DECLINED";
                    }
                    else
                    {
                        attendee.ParticipationStatus = "NEEDS-ACTION";
                    }

                    if (AddressUtility.GetAddress(v).Equals(AddressUtility.GetAddress(organizer)))
                    {
                        attendee.ParticipationStatus = "ACCEPTED";
                    }

                    attendees.Add(attendee);
                });
            }
            return attendees;
        }

        public static CalendarInfo ConvertToCalendarInfo(ScheduleComposeEventViewModel viewModel)
        {
            CalendarInfo calendarInfo = new CalendarInfo();
            calendarInfo.Id = viewModel.Id;
            calendarInfo.CalendarTitle = viewModel.CalendarTitle;
            calendarInfo.Organizer = viewModel.Organizer;
            calendarInfo.Start = viewModel.Start.ToUniversalTime();
            calendarInfo.End = viewModel.End.ToUniversalTime();
            calendarInfo.Attendees = viewModel.Attendees;
            calendarInfo.Location = viewModel.Location;
            calendarInfo.Mark = viewModel.Mark;
            return calendarInfo;
        }

        private string UriConvertToMailAddress(AttendeeModel attendee)
        {
            var attendeeUri = attendee.Value;
            string attendeeAddress = string.Empty;
            if (attendeeUri != null)
            {
                attendeeAddress = $"{attendee.CommonName} <{attendeeUri.UserInfo!}@{attendeeUri.Host!}>";
            }
            else
            {
                attendeeAddress = attendee.CommonName;
            }

            var organAddress = AddressUtility.GetAddress(Organizer);
            if (organAddress != null && !organAddress.Equals(AddressUtility.GetAddress(attendeeAddress)))
            {
                Contact.Add(attendeeAddress);
            }

            return attendeeAddress;
        }

        private string MatchUriConvertToHtml(string mark)
        {
            var uriRegex = new Regex(@"(https?|ftp|file)://[-A-Za-z0-9+&@#/%?=~_|!:,.;]+[-A-Za-z0-9+&@#/%=~_|]");
            var collections = uriRegex.Matches(mark);
            foreach (var collection in collections)
            {
                mark = mark.Replace(collection.ToString(), $"<a href=\"{collection}\">{collection}</a>");
            }

            return $"<div>{mark}</div>";
        }

        private bool ContainATag(string mark)
        {
            if (string.IsNullOrEmpty(mark)) return false;
            var aTagRegex = new Regex(@"<\s*a[^>]*>(.*?)<\s*/\s*a>");
            var checkATag = aTagRegex.Matches(mark);
            return checkATag.Count > 0 ? true : false;
        }
    }
}
