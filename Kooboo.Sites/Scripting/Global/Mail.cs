//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Data.Context;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Kooboo.Sites.Scripting.Global
{
    public class Mail
    {
        private RenderContext context { get; set; }

        public Mail(RenderContext context)
        {
            this.context = context;
        }

        public void Send(object value)
        {
            var maildata = PrepareData(value);
            if (maildata != null)
            {
                if (Kooboo.Data.AppSettings.IsOnlineServer && !isLocalFromAddress(maildata))
                {
                    throw new Exception("From address not local");
                }

                if (this.context.WebSite == null)
                {
                    throw new Exception("invalid context");
                }

                var orgid = this.context.WebSite.OrganizationId;

                string messagebody = null;
                if (maildata.HtmlBody != null)
                {
                    messagebody = Kooboo.Mail.Utility.ComposeUtility.ComposeHtmlTextEmailBody(maildata.From, maildata.originalToString, maildata.Subject, maildata.HtmlBody, maildata.TextBody);
                }
                else
                {
                    messagebody = Kooboo.Mail.Utility.ComposeUtility.ComposeTextEmailBody(maildata.From, maildata.originalToString, maildata.Subject, maildata.TextBody);
                }

                List<string> allrcptos = new List<string>();
                allrcptos.AddRange(maildata.To);
                if (maildata.Cc != null && maildata.Cc.Any())
                {
                    allrcptos.AddRange(maildata.Cc);
                }
                if (maildata.Bcc != null && maildata.Bcc.Any())
                {
                    allrcptos.AddRange(maildata.Bcc);
                }

                // check if org allowed to send.
                if (!Kooboo.Data.Infrastructure.InfraManager.Test(orgid, Data.Infrastructure.InfraType.Email, allrcptos.Count()))
                {
                    throw new Exception("No enough email sending credits");
                }
                else
                {
                    Kooboo.Mail.Transport.Incoming.Receive(maildata.From, allrcptos, messagebody);

                    Kooboo.Data.Infrastructure.InfraManager.Add(orgid, Data.Infrastructure.InfraType.Email, allrcptos.Count(), string.Join(",", allrcptos));
                }
            }
            else
            {
                throw new Exception("Invalid mail object");
            }
        }

        private bool isLocalFromAddress(MailObject mailobj)
        {
            var from = Kooboo.Mail.Utility.AddressUtility.GetAddress(mailobj.From);
            return Kooboo.Mail.Utility.AddressUtility.IsLocalEmailAddress(from);
        }

        internal MailObject PrepareData(object dataobj)
        {
            Dictionary<string, object> data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            if (dataobj is IDictionary idict)
            {
                foreach (var item in idict.Keys)
                {
                    var value = idict[item];
                    if (value != null)
                    {
                        data.Add(item.ToString(), value.ToString());
                    }
                }
            }
            else
            {
                if (dataobj is IDictionary<string, object> dynamicobj)
                {
                    foreach (var item in dynamicobj.Keys)
                    {
                        var value = dynamicobj[item];
                        if (value != null)
                        {
                            data.Add(item.ToString(), value.ToString());
                        }
                    }
                }
            }

            MailObject result = new MailObject();
            char[] sep = ";".ToCharArray();

            if (data.ContainsKey("to"))
            {
                var value = data["to"];
                if (value != null)
                {
                    string values = value.ToString();
                    result.To = values.Split(sep, StringSplitOptions.RemoveEmptyEntries).ToList();
                    result.originalToString = values;
                }
            }

            if (data.ContainsKey("from"))
            {
                var value = data["from"];
                if (value != null)
                {
                    result.From = value.ToString();
                }
            }

            if (data.ContainsKey("subject"))
            {
                var value = data["subject"];
                if (value != null)
                {
                    result.Subject = value.ToString();
                }
            }

            if (data.ContainsKey("textbody"))
            {
                var value = data["textbody"];
                if (value != null)
                {
                    result.TextBody = value.ToString();
                }
            }

            if (data.ContainsKey("htmlbody"))
            {
                var value = data["htmlbody"];
                if (value != null)
                {
                    result.HtmlBody = value.ToString();
                }
            }

            if (result.HtmlBody == null && result.TextBody == null)
            {
                if (data.ContainsKey("body"))
                {
                    var value = data["body"];
                    if (value != null)
                    {
                        result.HtmlBody = value.ToString();
                    }
                }
            }

            if (result.To == null || result.To.Count == 0 || result.From == null)
            {
                return null;
            }

            if (result.HtmlBody == null && result.TextBody == null)
            {
                return null;
            }

            if (result.Subject == null)
            {
                result.Subject = "undefined";
            }

            return result;
        }

        public void SmtpSend(object smtpServer, object mailMessage)
        {
            var server = GetSmtpServer(smtpServer);
            if (!string.IsNullOrEmpty(server.Host))
            {
                var mailobj = PrepareData(mailMessage);

                if (!string.IsNullOrWhiteSpace(mailobj.From) && mailobj.To != null && mailobj.To.Any())
                {
                    System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage
                    {
                        From = new System.Net.Mail.MailAddress(mailobj.From)
                    };

                    foreach (var item in mailobj.To)
                    {
                        msg.To.Add(item);
                    }

                    msg.Subject = mailobj.Subject;

                    if (!string.IsNullOrEmpty(mailobj.HtmlBody))
                    {
                        msg.Body = mailobj.HtmlBody;
                        msg.IsBodyHtml = true;
                    }
                    else
                    {
                        msg.Body = mailobj.TextBody;
                    }

                    if (msg.Body == null)
                    {
                        return;
                    }

                    if (msg.Body.IndexOf("<") == -1 && msg.Body.IndexOf(">") == -1)
                    {
                        msg.IsBodyHtml = false;
                    }

                    System.Net.Mail.SmtpClient client;

                    client = server.port > 0 ? new System.Net.Mail.SmtpClient(server.Host, server.port) : new System.Net.Mail.SmtpClient(server.Host);

                    if (server.username != null && server.password != null)
                    {
                        client.UseDefaultCredentials = false;
                        client.Credentials = new NetworkCredential(server.username, server.password);
                    }

                    if (server.Ssl)
                    {
                        client.EnableSsl = true;
                    }

                    client.Send(msg);
                }
            }
        }

        internal SmtpServer GetSmtpServer(object dataobj)
        {
            Dictionary<string, object> data = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            if (dataobj is IDictionary idict)
            {
                foreach (var item in idict.Keys)
                {
                    var value = idict[item];
                    if (value != null)
                    {
                        data.Add(item.ToString(), value.ToString());
                    }
                }
            }
            else
            {
                if (dataobj is IDictionary<string, object> dynamicobj)
                {
                    foreach (var item in dynamicobj.Keys)
                    {
                        var value = dynamicobj[item];
                        if (value != null)
                        {
                            data.Add(item.ToString(), value.ToString());
                        }
                    }
                }
            }

            SmtpServer result = new SmtpServer();

            if (data.ContainsKey("host"))
            {
                var value = data["host"];
                if (value != null)
                {
                    result.Host = value.ToString();
                }
            }

            if (data.ContainsKey("ssl"))
            {
                var value = data["ssl"];
                if (value != null)
                {
                    bool.TryParse(value.ToString(), out var ssl);
                    result.Ssl = ssl;
                }
            }

            if (data.ContainsKey("username"))
            {
                var value = data["username"];
                if (value != null)
                {
                    result.username = value.ToString();
                }
            }

            if (data.ContainsKey("password"))
            {
                var value = data["password"];
                if (value != null)
                {
                    result.password = value.ToString();
                }
            }

            if (data.ContainsKey("port"))
            {
                var value = data["port"];
                if (value != null)
                {
                    if (int.TryParse(value.ToString(), out var port))
                    {
                        if (port > 0)
                        {
                            result.port = port;
                        }
                    }
                }
            }

            return result;
        }
    }

    public class MailObject
    {
        public string From { get; set; }

        public List<string> To { get; set; }

        public string originalToString { get; set; }

        public List<string> Cc { get; set; }

        public List<string> Bcc { get; set; }

        public string Subject { get; set; }

        public string TextBody { get; set; }

        public string HtmlBody { get; set; }
    }

    public class SmtpServer
    {
        public string Host { get; set; }

        public bool Ssl { get; set; }

        public string username { get; set; }

        public string password { get; set; }

        public int port { get; set; }
    }
}