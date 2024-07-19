using System;
using System.Collections.Generic;
using System.Linq;
using Kooboo.Mail.Models;
using MimeKit;

namespace Kooboo.Mail.Repositories.Sqlite
{
    public class SmtpReportRepo
    {
        private string TableName
        {
            get
            {
                return "SmtpReport";
            }
        }

        private MailDb maildb { get; set; }

        public SmtpReportRepo(MailDb db)
        {
            this.maildb = db;
        }

        public SmtpReport Get(string messageId)
        {
            var id = SmtpReport.ToGuid(messageId);
            return this.Get(id);
        }


        public SqlWhere<SmtpReport> Query
        {
            get
            {
                return new SqlWhere<SmtpReport>(this.maildb.SqliteHelper, this.TableName);
            }
        }


        public SmtpReport Get(Guid Id)
        {
            return this.maildb.SqliteHelper.Get<SmtpReport>(this.TableName, nameof(SmtpReport.Id), Id);
        }


        [Obsolete]
        public void Add(SmtpReport report)
        {
            report.RcptTo = TrimAddress(report.RcptTo);

            var item = this.Get(report.Id);

            this.maildb.SqliteHelper.AddOrUpdate<SmtpReport>(report, this.TableName, nameof(SmtpReport.Id));
        }

        public void AddReport(SmtpReportIn ReportIn)
        {
            var Existing = this.Get(ReportIn.MessageId);

            if (Existing == null)
            {
                Existing = new SmtpReport() { MessageId = ReportIn.MessageId, HeaderFrom = ReportIn.HeaderFrom, Subject = ReportIn.Subject };
            }

            AppendReport(Existing, ReportIn);
            this.maildb.SqliteHelper.AddOrUpdate<SmtpReport>(Existing, this.TableName, nameof(SmtpReport.Id));
        }

        public void AppendReport(SmtpReport currentReport, SmtpReportIn Incoming)
        {
            Incoming.RcptTo = TrimAddress(Incoming.RcptTo);

            List<SingleDelivery> currentDelivery = DeserializeDelivery(currentReport.Json);

            var Single = SingleDelivery.FromReportIn(Incoming);

            currentDelivery.Add(Single);

            currentReport.Json = System.Text.Json.JsonSerializer.Serialize(currentDelivery);

        }


        public SmtpReportViewModel GetReports(string messageid, string To, string CC, string BCC, DateTime UtcCreationTime)
        {
            var timeSpan = DateTime.UtcNow - UtcCreationTime;

            bool IsRecent = timeSpan.Minutes < 20;

            var report = this.Get(messageid);
            if (report != null)
            {
                var AddList = GetRcptToList(To, CC, BCC);

                return SmtpReportViewModel.FromSmtpReport(report, AddList, IsRecent);
            }

            return null;
        }

        public List<string> GetRcptToList(string ToLine, string CCLine, string BccLine)
        {
            HashSet<string> addresses = new HashSet<string>();

            var to = GetAddressFromLine(ToLine);
            if (to != null)
            {
                foreach (var item in to)
                {
                    addresses.Add(item);
                }
            }

            var cc = GetAddressFromLine(CCLine);
            if (cc != null)
            {
                foreach (var item in cc)
                {
                    addresses.Add(item);
                }
            }

            var bcc = GetAddressFromLine(BccLine);
            if (bcc != null)
            {
                foreach (var item in bcc)
                {
                    addresses.Add(item);
                }
            }

            return addresses.ToList();
        }

        public HashSet<string> GetAddressFromLine(string AddressLine)
        {
            if (!string.IsNullOrWhiteSpace(AddressLine))
            {
                var tolist = Kooboo.Mail.Utility.MailKitUtility.GetMailKitAddressList(AddressLine);
                if (tolist != null)
                {
                    HashSet<string> list = new HashSet<string>();
                    foreach (var item in tolist)
                    {
                        var addrsss = GetAddress(item);
                        if (addrsss != null)
                        {
                            list.Add(addrsss);
                        }
                    }

                    return list;
                }
            }

            return null;
        }

        public string GetAddress(MailboxAddress add)
        {
            if (add != null && add.Address != null)
            {
                var address = add.Address.ToLower();

                var index = address.IndexOf('<');
                if (index > -1)
                {
                    var end = address.IndexOf('>', index + 1);

                    return address.Substring(index + 1, end - index - 1);
                }
                return address;
            }
            return null;
        }

        public string TrimAddress(string singleAddress)
        {
            var mailbox = Utility.MailKitUtility.ParseAddress(singleAddress);

            return GetAddress(mailbox);
        }

        public static List<SingleDelivery> DeserializeDelivery(string json)
        {
            if (json != null)
            {
                try
                {
                    var result = System.Text.Json.JsonSerializer.Deserialize<List<SingleDelivery>>(json);
                    return result == null ? new List<SingleDelivery>() : result;

                }
                catch (Exception)
                {
                }
            }

            return new List<SingleDelivery>();
        }
    }


    public record SingleDelivery
    {
        public string To { get; set; }

        public bool IsSuccess { get; set; }

        public bool IsSending { get; set; }

        public DateTime DeliveryTime { get; set; }

        public string Log { get; set; }


        public static SingleDelivery FromReportIn(SmtpReportIn incoming)
        {
            SingleDelivery single = new SingleDelivery();
            single.To = incoming.RcptTo;
            single.IsSuccess = incoming.IsSuccess;
            single.Log = incoming.Log;
            single.DeliveryTime = new DateTime(incoming.DateTick, DateTimeKind.Utc);
            if (single.DeliveryTime == DateTime.MinValue)
                single.DeliveryTime = DateTime.UtcNow;
            return single;
        }
    }


    public record SmtpReportViewModel
    {
        public bool IsSuccess { get; set; }
        public bool IsSending { get; set; }
        public List<SingleDelivery> Items { get; set; }

        public string Debug { get; set; }

        public static SmtpReportViewModel FromSmtpReport(SmtpReport report, List<string> RcptToList, bool IsRecent)
        {
            SmtpReportViewModel model = new SmtpReportViewModel();

            // string reportJson = System.Text.Json.JsonSerializer.Serialize(report);


            model.Items = new List<SingleDelivery>();

            if (report != null && report.Json != null && report.Json.Contains("["))
            {
                var DbItems = SmtpReportRepo.DeserializeDelivery(report.Json);

                foreach (var item in RcptToList)
                {
                    SingleDelivery delivery = new SingleDelivery();
                    delivery.To = item;

                    var find = DbItems.Find(o => o.To == item);

                    if (find == null && item != null)
                    {
                        find = DbItems.Find(o => o != null && o.To.ToLower() == item.ToLower());
                    }

                    if (find != null)
                    {
                        delivery.IsSuccess = find.IsSuccess;
                        delivery.Log = find.Log;
                        delivery.DeliveryTime = find.DeliveryTime;
                    }
                    else
                    {
                        delivery.IsSuccess = false;
                        delivery.IsSending = IsRecent;
                        delivery.Log = "Delivery record not found";
                        if (find is not null)
                            delivery.DeliveryTime = find.DeliveryTime;
                    }

                    model.Items.Add(delivery);
                }
            }
            else
            {
                //if (report.IsSuccess)
                //{
                // old report. not details.
                var Items = new List<SingleDelivery>();
                Items.Add(new SingleDelivery() { IsSuccess = report.IsSuccess, DeliveryTime = new DateTime(report.DateTick, DateTimeKind.Utc), Log = report.Log, });
                return new SmtpReportViewModel() { IsSuccess = report.IsSuccess, Items = Items, Debug = "Old format" };
                //}
                //return null; 
            }

            var notSuccess = model.Items.Find(o => o.IsSuccess == false);
            var isSending = model.Items.Find(o => o.IsSending);

            model.IsSuccess = notSuccess == null;
            model.IsSending = isSending != null;

            //if (model.Debug ==null)
            //{
            //    model.Debug = report.Json; 
            //} 
            // model.Debug = reportJson; 
            return model;
        }



    }
}
