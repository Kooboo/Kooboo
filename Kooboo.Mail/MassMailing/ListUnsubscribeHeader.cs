using System;

namespace Kooboo.Mail.MassMailing
{
    public class ListUnsubscribeHeader
    {

        public ListUnsubscribeHeader(string http, string email, string emailSubject, string emailBody)
        {
            this.UnsubscribeEmail = email;
            this.HttpUnsubscribeLink = http;
            this.EmailSimpleBody = emailBody;
            this.EmailSubject = emailSubject;
        }

        public ListUnsubscribeHeader()
        {

        }

        public string HttpUnsubscribeLink { get; set; }

        public string UnsubscribeEmail { get; set; }

        public string EmailSimpleBody { get; set; } = "unsubscribe list";

        public string EmailSubject { get; set; } = "unsubscribe";

        public string ToHeader(bool emailOnly = false)
        {
            string email = null;
            if (this.UnsubscribeEmail != null)
            {
                email = "mailto:" + this.UnsubscribeEmail;
                if (this.EmailSubject != null)
                {
                    email += "?subject=" + System.Net.WebUtility.UrlEncode(this.EmailSubject);
                    if (this.EmailSimpleBody != null)
                    {
                        email += "&body=" + System.Net.WebUtility.UrlEncode(this.EmailSimpleBody);
                    }
                }
            }

            if (emailOnly && !string.IsNullOrEmpty(email))
            {
                return "List-Unsubscribe: <" + email + ">";
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(this.HttpUnsubscribeLink) && !string.IsNullOrEmpty(email))
                {
                    return "List-Unsubscribe: <" + this.HttpUnsubscribeLink + ">, <" + email + ">";
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.HttpUnsubscribeLink) && string.IsNullOrEmpty(email))
                    {
                        return "List-Unsubscribe: <" + this.HttpUnsubscribeLink + ">";
                    }
                    else if (string.IsNullOrWhiteSpace(this.HttpUnsubscribeLink) && !string.IsNullOrWhiteSpace(email))
                    {
                        return "List-Unsubscribe: <" + email + ">";
                    }
                }
            }

            return null;

        }

        public static ListUnsubscribeHeader FromHeaderField(string headerValue)
        {

            ListUnsubscribeHeader Result = new ListUnsubscribeHeader();

            if (string.IsNullOrWhiteSpace(headerValue))
            {
                return Result;
            }

            if (headerValue.StartsWith("List-Unsubscribe"))
            {
                var index = headerValue.IndexOf(":");

                if (index > -1)
                {
                    headerValue = headerValue.Substring(index + 1);
                }
            }

            var parts = headerValue.Split(',', StringSplitOptions.RemoveEmptyEntries);

            if (parts != null)
            {
                foreach (var item in parts)
                {
                    if (item != null)
                    {
                        var itemValue = item.Trim();

                        if (itemValue.StartsWith("<"))
                        {
                            itemValue = itemValue.Substring(1, itemValue.Length - 2);
                        }


                        if (itemValue.ToLower().StartsWith("mailto"))
                        {

                            var index = itemValue.IndexOf(':');
                            if (index > -1)
                            {
                                itemValue = itemValue.Substring(index + 1);
                            }

                            var QMark = itemValue.IndexOf("?");

                            if (QMark > -1)
                            {
                                Result.UnsubscribeEmail = itemValue.Substring(0, QMark);
                                var query = itemValue.Substring(QMark + 1);

                                var queryString = System.Web.HttpUtility.ParseQueryString(query);
                                foreach (var queryKey in queryString.AllKeys)
                                {
                                    if (queryKey.ToLower() == "subject")
                                    {
                                        Result.EmailSubject = queryString.Get(queryKey);
                                        if (Result.EmailSubject != null)
                                        {
                                            Result.EmailSubject = System.Web.HttpUtility.UrlDecode(Result.EmailSubject);
                                        }
                                    }

                                    if (queryKey.ToLower() == "body")
                                    {
                                        Result.EmailSimpleBody = queryString.Get(queryKey);
                                        if (Result.EmailSimpleBody != null)
                                        {
                                            Result.EmailSimpleBody = System.Web.HttpUtility.UrlDecode(Result.EmailSimpleBody);
                                        }
                                    }
                                }

                            }
                            else
                            {
                                Result.UnsubscribeEmail = itemValue;
                            }

                        }

                        else if (itemValue.ToLower().StartsWith("http"))
                        {
                            Result.HttpUnsubscribeLink = itemValue;
                        }
                    }
                }
            }

            return Result;

        }
    }
}



/*
 * 
 * 3.2. List-Unsubscribe

   The List-Unsubscribe field describes the command (preferably using
   mail) to directly unsubscribe the user (removing them from the list).

   Examples:

     List-Unsubscribe: <mailto:list@host.com?subject=unsubscribe>
     List-Unsubscribe: (Use this command to get off the list)
         <mailto:list-manager@host.com?body=unsubscribe%20list>
     List-Unsubscribe: <mailto:list-off@host.com>



Neufeld & Baer              Standards Track                     [Page 4]

RFC 2369                  URLs as Meta-Syntax                  July 1998


     List-Unsubscribe: <http://www.host.com/list.cgi?cmd=unsub&lst=list>,
         <mailto:list-request@host.com?subject=unsubscribe>
 * 
 * 
 * 
 * 
 */ 