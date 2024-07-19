using System;
using System.Collections.Generic;
using System.Linq;
using MimeKit;

namespace Kooboo.Mail.ViewModel
{
    public class MessageViewModel
    {
        public int Id { get; set; }

        public int AddressId { get; set; }

        public string AddressName { get; set; }

        public int FolderId { get; set; }

        public string FolderName { get; set; }

        public AddressModel From { get; set; }

        public List<AddressModel> To { get; set; }

        public List<AddressModel> Cc { get; set; }

        public List<AddressModel> Bcc { get; set; }

        public string Subject { get; set; }

        public string Summary { get; set; }

        public int Size { get; set; }

        public bool Read { get; set; }

        public bool Answered { get; set; }

        public bool Deleted { get; set; }

        public bool Flagged { get; set; }

        public bool Recent { get; set; } = true;


        private DateTime _creationtime;

        public DateTime CreationTime
        {
            get
            {
                if (_creationtime == default(DateTime))
                {
                    _creationtime = DateTime.UtcNow;
                }
                return _creationtime;
            }
            set
            {
                _creationtime = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                _creationTimetick = default(long);
            }
        }


        private long _creationTimetick;

        public long CreationTimeTick
        {
            get
            {
                if (_creationTimetick == default(long))
                {
                    _creationTimetick = this.CreationTime.Ticks;
                }
                return _creationTimetick;
            }
            set
            {
                _creationTimetick = value;
            }
        }

        private DateTime _date;
        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                _date = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            }
        }

        public bool Draft
        {
            get
            {
                return this.FolderId == Folder.ToId("Draft");
            }
        }

        private List<ViewModel.AttachmentViewModel> _attachments;
        public List<ViewModel.AttachmentViewModel> Attachments
        {
            get
            {
                if (_attachments == null)
                {
                    _attachments = new List<AttachmentViewModel>();
                }
                return _attachments;
            }
            set
            {
                _attachments = value;
            }
        }

        public bool HasAttachment
        {
            get
            {
                if (_attachments != null && _attachments.Count() > 0)
                {
                    return true;
                }
                return false;
            }
        }

        /// public Kooboo.Mail.Models.SmtpReport SentLog { get; set; }

        public Repositories.Sqlite.SmtpReportViewModel DeliveryLog { get; set; }


        public string TmpDebug { get; set; }

        public static MessageViewModel FromMessage(Message msg, MailDb maildb, OrgDb orgdb)
        {
            MessageViewModel model = new MessageViewModel();

            model.Id = (int)msg.MsgId;
            model.AddressId = msg.AddressId;

            if (!string.IsNullOrEmpty(msg.Body))
            {
                //var mime = Utility.MessageUtility.ParseMessage(msg.Body);
                //var fromMailboxAddress = mime.From.First() as MailboxAddress;
                //model.From = new AddressModel()
                //{
                //    Name = fromMailboxAddress.Name,
                //    Address = fromMailboxAddress.Address
                //};
                //var mailFromInfo = orgdb.Email.Get(model.From.Address);
                //if (mailFromInfo != null && mailFromInfo.AddressType == EmailAddressType.Group)
                //{
                //    var from = MessageUtility.GetHeaderValue(mime, "Sender");
                //    model.From = MessageUtility.GetAddressModel(from);
                //}
                //else
                //{
                //    var from = MailboxAddress.Parse(mime.From.First().ToString());
                //    var sender = mime.Sender;
                //    if (model.From is null && sender != null && from != sender)
                //        model.From = MessageUtility.GetAddressModel(sender.ToString());
                //}
                //if (mime != null)
                //    mime.Dispose();
                var mime = Utility.MessageUtility.ParseMessage(msg.Body);
                var fromMailboxAddress = mime.From.FirstOrDefault() as MailboxAddress;

                var mailFromInfo = orgdb.Email.Get(fromMailboxAddress?.Address);
                if (mailFromInfo != null && mailFromInfo.AddressType == EmailAddressType.Group)
                {
                    var from = Utility.MessageUtility.GetHeaderValue(mime, "Sender");
                    model.From = Utility.MessageUtility.GetAddressModel(from);
                }
                else
                {
                    var from = Utility.MessageUtility.GetHeaderValue(mime, "From");
                    model.From = Utility.MessageUtility.GetAddressModel(from);
                }
            }
            if (model.From == null)
            {
                model.From = Utility.MessageUtility.GetAddressModel(msg.From);
            }

            model.To = Utility.MessageUtility.GetAddressModels(msg.To);
            model.Subject = msg.Subject;
            model.Summary = msg.Summary;
            model.Cc = Utility.MessageUtility.GetAddressModels(msg.Cc);
            model.Bcc = Utility.MessageUtility.GetAddressModels(msg.Bcc);

            model.Answered = msg.Answered;
            model.Read = msg.Read;
            model.Recent = msg.Recent;
            model.Attachments = AttachmentViewModel.FromAttachments(msg.MsgId, msg.Attachments);
            model.Date = msg.Date;
            if (model.Date == default(DateTime))
            {
                model.Date = msg.CreationTime;
            }

            model.FolderId = msg.FolderId;

            if (Folder.ReservedFolder.TryGetValue(msg.FolderId, out var result))
            {
                model.FolderName = result.ToLower();
                if (result.Equals(Folder.Inbox, StringComparison.OrdinalIgnoreCase) || result.Equals(Folder.Sent, StringComparison.OrdinalIgnoreCase))
                {
                    var addressInfo = orgdb.Email.Get(model.AddressId);
                    if (addressInfo is not null)
                        model.AddressName = addressInfo.Address;
                }
            }
            else
            {
                var folder = maildb.Folder.Get(model.FolderId);
                if (folder is not null)
                    model.FolderName = folder.Name;
            }

            if (msg.FolderId == Folder.ToId(Folder.Sent))
            {
                // model.SentLog = maildb.SmtpDelivery.Get(msg.SmtpMessageId);
                model.DeliveryLog = maildb.SmtpDelivery.GetReports(msg.SmtpMessageId, msg.To, msg.Cc, msg.Bcc, msg.CreationTime);

                // TEMP Debug
                //var tmpReport = maildb.SmtpDelivery.Get(msg.SmtpMessageId);
                //string debug = null; 
                //if (tmpReport != null)
                //{
                //    debug = System.Text.Json.JsonSerializer.Serialize(tmpReport);
                //}
                //else
                //{
                //    debug = "No report,  msgid: " + msg.SmtpMessageId + " id:" + SmtpReport.ToGuid(msg.SmtpMessageId).ToString(); 
                //}

                //if (model.DeliveryLog == null || model.DeliveryLog.Items == null || !model.DeliveryLog.Items.Any())
                //{
                //    model.DeliveryLog = new Repositories.Sqlite.SmtpReportViewModel() { Debug = debug };
                //}
                //else
                //{
                //    model.DeliveryLog.Debug = debug;
                //}




                ////try search.
                //if (model.SentLog == null)
                //{
                //    var tickLower = msg.Date.ToUniversalTime().AddDays(-1).Ticks;
                //    var tickHigher = msg.Date.ToUniversalTime().AddDays(1).Ticks;

                //    var logs = maildb.SmtpDelivery.Query.Where(o => o.DateTick > tickLower && o.DateTick < tickHigher && o.Subject == msg.Subject).SelectAll();


                //    if (logs != null)
                //    {
                //        // compare headers... 

                //        foreach (var log in logs)
                //        {
                //            if (!string.IsNullOrWhiteSpace(log.Json))
                //            {
                //                var headers = System.Text.Json.JsonSerializer.Deserialize<SmtpReportHeaders>(log.Json);

                //                // From Remote...
                //                // SmtpReportHeaders header = new SmtpReportHeaders(); 
                //                // header.HeaderFrom = msg.From.ToString();
                //                // header.Subject = msg.Subject;
                //                // header.Date = msg.Date.ToUniversalTime().ToString("yyyy-MM-dd:hh:mm");
                //                // header.To = msg.To.ToString(); 
                //                if (msg.To.ToString() == headers.To)
                //                {
                //                    var msgDate = msg.Date.ToUniversalTime().ToString("yyyy-MM-dd:hh:mm");
                //                    if (msgDate == headers.Date)
                //                    {
                //                        // found.
                //                        log.MessageId = msg.SmtpMessageId;
                //                        log.Id = default(Guid);
                //                        maildb.SmtpDelivery.Add(log);
                //                        model.SentLog = maildb.SmtpDelivery.Get(msg.SmtpMessageId);
                //                        break;
                //                    }
                //                }

                //            }
                //        }

                //    }


                //}


            }

            model.CreationTime = msg.CreationTime;
            model.CreationTimeTick = msg.CreationTimeTick;

            return model;
        }

    }
}
