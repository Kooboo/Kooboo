using System.Text;
using System.Text.Json;
using JWT;
using JWT.Serializers;
using Kooboo.Api;
using Kooboo.Api.ApiResponse;
using Kooboo.Data;
using Kooboo.Data.Context;
using Kooboo.Data.Models;
using Kooboo.Mail;
using Kooboo.Mail.Factory;
using Kooboo.Mail.Models;
using Kooboo.Mail.Utility;
using Kooboo.Mail.ViewModel;
using MimeKit;

namespace Kooboo.Web.Api.Implementation.Mails
{
    public class EmailCalendarReply : IApi
    {
        public string ModelName
        {
            get
            {
                return "EmailCalendarReply";
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
                return false;
            }
        }

        private const int AcceptReply = 1;
        private const int DeclineReply = 2;
        private const int TentativeReply = 3;

        private const string AcceptIcon = """<svg t="1710982741998" class="icon" style="vertical-align:middle" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="6832" width="14" height="14"><path d="M512 16a496 496 0 1 1 0 992A496 496 0 0 1 512 16z m0 96a400 400 0 1 0 0 800 400 400 0 0 0 0-800z m225.92 238.08a48 48 0 0 1 4.672 62.464l-4.672 5.376-255.936 256a48 48 0 0 1-62.464 4.672l-5.44-4.672-128-128a48 48 0 0 1 62.464-72.512l5.376 4.672L448 572.096l222.08-222.08a48 48 0 0 1 67.84 0z" fill="#00B42A" p-id="6833"></path></svg>""";
        private const string DeclineIcon = """<svg t="1710982851777" class="icon" style="vertical-align:middle" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="7811" width="14" height="14"><path d="M512 992C246.912 992 32 777.088 32 512 32 246.912 246.912 32 512 32c265.088 0 480 214.912 480 480 0 265.088-214.912 480-480 480z m0-64c229.76 0 416-186.24 416-416S741.76 96 512 96 96 282.24 96 512s186.24 416 416 416z" fill="#ef3d25" p-id="7812"></path><path d="M572.512 512l161.696 161.664-60.544 60.544L512 572.48l-161.664 161.696-60.544-60.544L451.52 512 288 348.512 348.512 288 512 451.488 675.488 288 736 348.512 572.512 512z" fill="#ef3d25" p-id="7813"></path></svg>""";
        private const string TentativeIcon = """<svg t="1710982641696" class="icon" style="vertical-align:middle" viewBox="0 0 1024 1024" version="1.1" xmlns="http://www.w3.org/2000/svg" p-id="4692" width="14" height="14"><path d="M512 64c247.424 0 448 200.576 448 448s-200.576 448-448 448S64 759.424 64 512 264.576 64 512 64z m0 85.333333C311.701333 149.333333 149.333333 311.701333 149.333333 512s162.368 362.666667 362.666667 362.666667 362.666667-162.368 362.666667-362.666667S712.298667 149.333333 512 149.333333z m0 533.333334c21.333333 0 42.666667 16 42.666667 42.666666s-21.333333 42.666667-42.666667 42.666667-42.666667-16-42.666667-42.666667 16-42.666667 42.666667-42.666666z m0-405.333334c80 0 149.333333 62.165333 149.333333 139.882667 0 51.818667-45.269333 102.613333-74.666666 124.352-10.069333 6.442667-29.653333 33.066667-31.808 53.461333l-0.192 3.52C554.666667 619.306667 538.666667 640 512 640c-21.333333 0-42.666667-15.552-42.666667-41.450667 0-50.133333 45.013333-105.130667 72.021334-122.709333l2.645333-1.621333c26.602667-15.552 32-36.266667 32-57.002667 0-36.266667-32-62.165333-64-62.165333-30.848 0-61.717333 24.085333-63.872 58.325333l-0.128 3.84c0 20.736-16 41.450667-42.666667 41.450667s-42.666667-20.714667-42.666666-41.450667C362.666667 339.498667 432 277.333333 512 277.333333z" fill="#1296db" p-id="4693"></path></svg>""";

        public PlainResponse MailBodyDealingCalendar(ApiCall apiCall)
        {
            if (EmailForwardManager.RequireForward(apiCall.Context))
            {
                var dic = new Dictionary<string, string>();
                dic.Add("token", apiCall.GetValue("token"));
                EmailForwardManager.Get<string>(this.ModelName, nameof(EmailCalendarReply.MailBodyDealingCalendar), apiCall.Context.User, dic);

            }

            var token = apiCall.GetValue("token");
            string payload = DecodeJwtToGetPayload(token);
            string html = GenereateExpireHtml(); ;
            var reply = JsonSerializer.Deserialize<MailBodyReplyModel>(payload);
            if (reply != null)
            {
                var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(reply.User);
                var existCalendar = maildb.Calendar.GetScheduleById(reply.CalendarId);
                if (existCalendar != null && existCalendar.Attendees != null)
                {
                    existCalendar.Attendees.ForEach(v =>
                    {
                        var currentEmail = MailboxAddress.Parse(v.Value.ToString().Replace("mailto:", ""));
                        var attendeeEmail = MailboxAddress.Parse(reply.ReplyAddress);
                        if (currentEmail.Address.Equals(attendeeEmail.Address))
                        {
                            v.ParticipationStatus = SignParticipationStatus(reply.ReplyStatus);
                            html = GenerateReplyInfoHtml(reply.ReplyStatus, existCalendar, reply.ReplyAddress);
                            ReplyMessageToOrganizer(reply.MessageId, existCalendar.Organizer, reply.ReplyAddress, reply.ReplyStatus, reply.User);
                        }
                    });

                    maildb.Calendar.AddOrUpdate(existCalendar);
                }

            }

            var response = new PlainResponse()
            {
                ContentType = "text/html",
                Success = true,
                Content = html
            };

            return response;

        }

        private string GenereateExpireHtml()
        {
            var builder = new StringBuilder();
            string htmlContent = "<!DOCTYPE html>\r\n<html lang=\"en\">\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n    <title>Kooboo Calendar Invite Result</title>\r\n    <style>\r\n        .tip{\r\n            width: 450px;\r\n            background-color: #8883;\r\n            padding:15px;         border-radius: 10px;\r\n            margin-top: 5px;\r\n}\r\n\r\n        .content{\r\n            font-size: 16px;\r\n        }\r\n\r\n        .leftTopic{\r\n            color:#888\r\n        }\r\n        tr{\r\n            height: 30px;\r\n            vertical-align: top;\r\n        }\r\n    </style>\r\n</head>\r\n<body>\r\n    <div>\r\n        <div class=\"header\">\r\n            <div class=\"logo\">\r\n                <svg width=\"122px\" height=\"35px\" viewBox=\"0 -5 122 30\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\">\r\n                    <defs>\r\n                        <rect id=\"path-1\" x=\"0\" y=\"0\" width=\"1440\" height=\"80\"></rect>\r\n                    </defs>\r\n                    <g id=\"页面排布\" stroke=\"none\" stroke-width=\"1\" fill=\"none\" fill-rule=\"evenodd\">\r\n                        <g id=\"nav/kooboo_white\" transform=\"translate(-40.000000, -25.000000)\">\r\n                            <g id=\"kooboo/主logo_white\" transform=\"translate(40.000000, 25.000000)\">\r\n                                <path d=\"M25.2467059,24.7872093 L14.6587571,17.8189876 L14.6587571,5.12607273 C14.6587571,4.50481571 14.1526349,4 13.5281258,4 C12.9044311,4 12.3966802,4.50481571 12.3966802,5.12607273 L12.3966802,25.8399483 C12.3966802,26.4623211 12.9044311,26.9668325 13.5281258,26.9668325 C14.1526349,26.9668325 14.6587571,26.4623211 14.6587571,25.8399483 L14.6587571,20.5180324 L23.918288,26.6125385 C24.4239011,26.9770769 25.1317799,26.8644899 25.4988508,26.3621085 C25.8659217,25.8576985 25.752319,25.1531677 25.2467059,24.7872093 M114.154306,23.1679853 C112.833116,24.481703 111.018426,25.2926336 109.005338,25.2926336 C106.990622,25.2926336 105.175932,24.481703 103.854843,23.1679853 C102.536198,21.8518332 101.723369,20.0434428 101.723369,18.0363515 C101.723369,16.0301731 102.536198,14.2215798 103.854843,12.9048192 C105.175932,11.5910001 106.990622,10.7811852 109.005338,10.7807795 C111.018426,10.7811852 112.833116,11.5910001 114.154306,12.9048192 C115.472647,14.2215798 116.28629,16.0301731 116.287308,18.0363515 C116.28629,20.0434428 115.472647,21.8518332 114.154306,23.1679853 M109.005338,9.07280449 C104.035528,9.07392021 100.009964,13.0859727 100.008844,18.0363515 C100.009964,22.9878461 104.035528,26.9993914 109.005338,27 C113.973214,26.9993914 118,22.9878461 118,18.0363515 C118,13.0859727 113.973214,9.07392021 109.005338,9.07280449 M95.449368,23.1679853 C94.1282793,24.481703 92.3135891,25.2926336 90.2998908,25.2926336 C88.287414,25.2926336 86.4716042,24.481703 85.1493957,23.1679853 C83.8304447,21.8518332 83.018532,20.0434428 83.018532,18.0363515 C83.018532,16.0301731 83.8304447,14.2215798 85.1493957,12.9048192 C86.4716042,11.5910001 88.287414,10.7811852 90.2998908,10.7807795 C92.3135891,10.7811852 94.1282793,11.5910001 95.449368,12.9048192 C96.7680136,14.2215798 97.5808425,16.0301731 97.5824712,18.0363515 C97.5808425,20.0434428 96.7680136,21.8518332 95.449368,23.1679853 M90.2998908,9.07280449 C85.3305899,9.07392021 81.3050255,13.0859727 81.3050255,18.0363515 C81.3050255,22.9878461 85.3305899,26.9993914 90.2998908,27 C95.2686828,26.9993914 99.2948579,22.9878461 99.2948579,18.0363515 C99.2948579,13.0859727 95.2686828,9.07392021 90.2998908,9.07280449 M59.7292792,23.1679853 C58.4078851,24.481703 56.593195,25.2926336 54.5795985,25.2926336 C52.5659001,25.2926336 50.75121,24.481703 49.4295105,23.1679853 C48.1114757,21.8518332 47.2983414,20.0434428 47.2983414,18.0363515 C47.2983414,16.0301731 48.1114757,14.2215798 49.4295105,12.9048192 C50.75121,11.5910001 52.5659001,10.7811852 54.5795985,10.7807795 C56.593195,10.7811852 58.4078851,11.5910001 59.7292792,12.9048192 C61.0479248,14.2215798 61.8617717,16.0301731 61.8626878,18.0363515 C61.8617717,20.0434428 61.0479248,21.8518332 59.7292792,23.1679853 M54.5795985,9.07280449 C49.6108065,9.07392021 45.5852422,13.0859727 45.5841224,18.0363515 C45.5852422,22.9878461 49.6108065,26.9993914 54.5795985,27 C59.5488994,26.9993914 63.5749727,22.9878461 63.5749727,18.0363515 C63.5749727,13.0859727 59.5488994,9.07392021 54.5795985,9.07280449 M41.0241371,23.1679853 C39.7030484,24.481703 37.8888672,25.2926336 35.8763904,25.2926336 C33.8627939,25.2926336 32.0463733,24.481703 30.7257935,23.1679853 C29.4060282,21.8518332 28.5931993,20.0434428 28.5931993,18.0363515 C28.5931993,16.0301731 29.4060282,14.2215798 30.7257935,12.9048192 C32.0463733,11.5910001 33.8627939,10.7811852 35.8763904,10.7807795 C37.8888672,10.7811852 39.7030484,11.5910001 41.0241371,12.9048192 C42.3430881,14.2215798 43.1567313,16.0301731 43.1575457,18.0363515 C43.1567313,20.0434428 42.3430881,21.8518332 41.0241371,23.1679853 M35.8763904,9.07280449 C30.9065805,9.07392021 26.8809144,13.0859727 26.8792857,18.0363515 C26.8809144,22.9878461 30.9065805,26.9993914 35.8763904,27 C40.8437573,26.9993914 44.870136,22.9878461 44.870136,18.0363515 C44.870136,13.0859727 40.8437573,9.07392021 35.8763904,9.07280449 M71.328149,25.0995114 C69.7349636,25.0995114 68.2690214,24.5693382 67.0912593,23.6790896 L67.0912593,12.5137062 C68.2690214,11.6229505 69.7349636,11.0932845 71.328149,11.0932845 C75.2107939,11.0932845 78.3574647,14.2284771 78.3574647,18.0968036 C78.3574647,21.9645216 75.2107939,25.0995114 71.328149,25.0995114 M71.328149,9.22494906 C69.7915613,9.22494906 68.3506605,9.61829351 67.0912593,10.3034513 L67.0912593,5.12607273 C67.0912593,4.50481571 66.5838138,4 65.9602209,4 C65.3357118,4 64.8275537,4.50481571 64.8275537,5.12607273 L64.8275537,25.8399483 C64.8275537,26.4623211 65.3357118,26.9668325 65.9602209,26.9668325 C66.5678321,26.9668325 67.0637748,26.4874756 67.088918,25.8872146 C68.3488282,26.572778 69.7905434,26.9668325 71.328149,26.9668325 C76.2445168,26.9668325 80.2290579,22.9962647 80.2290579,18.0968036 C80.2290579,13.196734 76.2445168,9.22494906 71.328149,9.22494906\" id=\"Fill-1\" fill=\"currentColor\"></path>\r\n                                <path d=\"M13.3779077,15.2844618 C8.25357379,19.2552325 4.56596711,23.4440769 3.99998982,25.7150883 C4.89751171,23.5807028 7.94859754,20.2189162 12.0615015,17.0324002 C17.6681377,12.6865425 23.1189265,10.3197814 24.233067,11.7496362 C25.2940708,13.108693 22.044893,17.4141816 16.8973499,21.5728001 C17.1874642,21.3564505 17.4770695,21.136348 17.7699322,20.910464 C24.0863812,16.0143501 28.2248356,10.7849381 27.0129724,9.232252 C25.7998876,7.67845015 19.6954765,10.3875365 13.3779077,15.2844618\" id=\"Fill-3\" fill=\"currentColor\"></path>\r\n                            </g>\r\n                        </g>\r\n                    </g>\r\n                </svg>\r\n                <t style=\"color:#666; font-size: 28px;\">Calendar</t>\r\n            </div>";

            builder.Append(htmlContent);
            builder.Append(TipHtmlContent(-1));

            return builder.ToString();
        }

        private string GenerateReplyInfoHtml(int replyStatus, CalendarInfo calendarInfo, string replyAddress)
        {
            string htmlContent = "<!DOCTYPE html>\r\n<html lang=\"en\">\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n    <title>Kooboo Calendar Invite Result</title>\r\n    <style>\r\n        .tip{\r\n            width: 450px;\r\n           background-color: #f1e5d696;\r\n            padding: 15px;\r\n            border-radius: 10px;\r\n            margin-top: 5px;\r\n}\r\n\r\n        .content{\r\n            font-size: 16px;\r\n        }\r\n\r\n        .leftTopic{\r\n            color:#888\r\n        }\r\n        .main{ display:flex;\r\n flex-direction:column;\r\n gap:20px;\r\n}\r\n</style>\r\n</head>\r\n<body>\r\n    <div>\r\n        <div class=\"main\">\r\n            <div class=\"logo\">\r\n                <svg width=\"122px\" height=\"35px\" viewBox=\"0 -5 122 30\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\">\r\n                    <defs>\r\n                        <rect id=\"path-1\" x=\"0\" y=\"0\" width=\"1440\" height=\"80\"></rect>\r\n                    </defs>\r\n                    <g id=\"页面排布\" stroke=\"none\" stroke-width=\"1\" fill=\"none\" fill-rule=\"evenodd\">\r\n                        <g id=\"nav/kooboo_white\" transform=\"translate(-40.000000, -25.000000)\">\r\n                            <g id=\"kooboo/主logo_white\" transform=\"translate(40.000000, 25.000000)\">\r\n                                <path d=\"M25.2467059,24.7872093 L14.6587571,17.8189876 L14.6587571,5.12607273 C14.6587571,4.50481571 14.1526349,4 13.5281258,4 C12.9044311,4 12.3966802,4.50481571 12.3966802,5.12607273 L12.3966802,25.8399483 C12.3966802,26.4623211 12.9044311,26.9668325 13.5281258,26.9668325 C14.1526349,26.9668325 14.6587571,26.4623211 14.6587571,25.8399483 L14.6587571,20.5180324 L23.918288,26.6125385 C24.4239011,26.9770769 25.1317799,26.8644899 25.4988508,26.3621085 C25.8659217,25.8576985 25.752319,25.1531677 25.2467059,24.7872093 M114.154306,23.1679853 C112.833116,24.481703 111.018426,25.2926336 109.005338,25.2926336 C106.990622,25.2926336 105.175932,24.481703 103.854843,23.1679853 C102.536198,21.8518332 101.723369,20.0434428 101.723369,18.0363515 C101.723369,16.0301731 102.536198,14.2215798 103.854843,12.9048192 C105.175932,11.5910001 106.990622,10.7811852 109.005338,10.7807795 C111.018426,10.7811852 112.833116,11.5910001 114.154306,12.9048192 C115.472647,14.2215798 116.28629,16.0301731 116.287308,18.0363515 C116.28629,20.0434428 115.472647,21.8518332 114.154306,23.1679853 M109.005338,9.07280449 C104.035528,9.07392021 100.009964,13.0859727 100.008844,18.0363515 C100.009964,22.9878461 104.035528,26.9993914 109.005338,27 C113.973214,26.9993914 118,22.9878461 118,18.0363515 C118,13.0859727 113.973214,9.07392021 109.005338,9.07280449 M95.449368,23.1679853 C94.1282793,24.481703 92.3135891,25.2926336 90.2998908,25.2926336 C88.287414,25.2926336 86.4716042,24.481703 85.1493957,23.1679853 C83.8304447,21.8518332 83.018532,20.0434428 83.018532,18.0363515 C83.018532,16.0301731 83.8304447,14.2215798 85.1493957,12.9048192 C86.4716042,11.5910001 88.287414,10.7811852 90.2998908,10.7807795 C92.3135891,10.7811852 94.1282793,11.5910001 95.449368,12.9048192 C96.7680136,14.2215798 97.5808425,16.0301731 97.5824712,18.0363515 C97.5808425,20.0434428 96.7680136,21.8518332 95.449368,23.1679853 M90.2998908,9.07280449 C85.3305899,9.07392021 81.3050255,13.0859727 81.3050255,18.0363515 C81.3050255,22.9878461 85.3305899,26.9993914 90.2998908,27 C95.2686828,26.9993914 99.2948579,22.9878461 99.2948579,18.0363515 C99.2948579,13.0859727 95.2686828,9.07392021 90.2998908,9.07280449 M59.7292792,23.1679853 C58.4078851,24.481703 56.593195,25.2926336 54.5795985,25.2926336 C52.5659001,25.2926336 50.75121,24.481703 49.4295105,23.1679853 C48.1114757,21.8518332 47.2983414,20.0434428 47.2983414,18.0363515 C47.2983414,16.0301731 48.1114757,14.2215798 49.4295105,12.9048192 C50.75121,11.5910001 52.5659001,10.7811852 54.5795985,10.7807795 C56.593195,10.7811852 58.4078851,11.5910001 59.7292792,12.9048192 C61.0479248,14.2215798 61.8617717,16.0301731 61.8626878,18.0363515 C61.8617717,20.0434428 61.0479248,21.8518332 59.7292792,23.1679853 M54.5795985,9.07280449 C49.6108065,9.07392021 45.5852422,13.0859727 45.5841224,18.0363515 C45.5852422,22.9878461 49.6108065,26.9993914 54.5795985,27 C59.5488994,26.9993914 63.5749727,22.9878461 63.5749727,18.0363515 C63.5749727,13.0859727 59.5488994,9.07392021 54.5795985,9.07280449 M41.0241371,23.1679853 C39.7030484,24.481703 37.8888672,25.2926336 35.8763904,25.2926336 C33.8627939,25.2926336 32.0463733,24.481703 30.7257935,23.1679853 C29.4060282,21.8518332 28.5931993,20.0434428 28.5931993,18.0363515 C28.5931993,16.0301731 29.4060282,14.2215798 30.7257935,12.9048192 C32.0463733,11.5910001 33.8627939,10.7811852 35.8763904,10.7807795 C37.8888672,10.7811852 39.7030484,11.5910001 41.0241371,12.9048192 C42.3430881,14.2215798 43.1567313,16.0301731 43.1575457,18.0363515 C43.1567313,20.0434428 42.3430881,21.8518332 41.0241371,23.1679853 M35.8763904,9.07280449 C30.9065805,9.07392021 26.8809144,13.0859727 26.8792857,18.0363515 C26.8809144,22.9878461 30.9065805,26.9993914 35.8763904,27 C40.8437573,26.9993914 44.870136,22.9878461 44.870136,18.0363515 C44.870136,13.0859727 40.8437573,9.07392021 35.8763904,9.07280449 M71.328149,25.0995114 C69.7349636,25.0995114 68.2690214,24.5693382 67.0912593,23.6790896 L67.0912593,12.5137062 C68.2690214,11.6229505 69.7349636,11.0932845 71.328149,11.0932845 C75.2107939,11.0932845 78.3574647,14.2284771 78.3574647,18.0968036 C78.3574647,21.9645216 75.2107939,25.0995114 71.328149,25.0995114 M71.328149,9.22494906 C69.7915613,9.22494906 68.3506605,9.61829351 67.0912593,10.3034513 L67.0912593,5.12607273 C67.0912593,4.50481571 66.5838138,4 65.9602209,4 C65.3357118,4 64.8275537,4.50481571 64.8275537,5.12607273 L64.8275537,25.8399483 C64.8275537,26.4623211 65.3357118,26.9668325 65.9602209,26.9668325 C66.5678321,26.9668325 67.0637748,26.4874756 67.088918,25.8872146 C68.3488282,26.572778 69.7905434,26.9668325 71.328149,26.9668325 C76.2445168,26.9668325 80.2290579,22.9962647 80.2290579,18.0968036 C80.2290579,13.196734 76.2445168,9.22494906 71.328149,9.22494906\" id=\"Fill-1\" fill=\"currentColor\"></path>\r\n                                <path d=\"M13.3779077,15.2844618 C8.25357379,19.2552325 4.56596711,23.4440769 3.99998982,25.7150883 C4.89751171,23.5807028 7.94859754,20.2189162 12.0615015,17.0324002 C17.6681377,12.6865425 23.1189265,10.3197814 24.233067,11.7496362 C25.2940708,13.108693 22.044893,17.4141816 16.8973499,21.5728001 C17.1874642,21.3564505 17.4770695,21.136348 17.7699322,20.910464 C24.0863812,16.0143501 28.2248356,10.7849381 27.0129724,9.232252 C25.7998876,7.67845015 19.6954765,10.3875365 13.3779077,15.2844618\" id=\"Fill-3\" fill=\"currentColor\"></path>\r\n                            </g>\r\n                        </g>\r\n                    </g>\r\n                </svg>\r\n                <t style=\"color:#666; font-size: 28px;\">Calendar</t>\r\n            </div>";
            htmlContent += TipHtmlContent(replyStatus);
            htmlContent += $" <div>\r\n <f style=\"font-weight: bolder; font-size: 1.17em; word-break: break-all\">{calendarInfo.CalendarTitle}</f></div>";
            htmlContent += MarkHtmlContent(calendarInfo.Mark);
            htmlContent += DateTimeHtmlContent(calendarInfo.Start, calendarInfo.End);
            htmlContent += LocationHtmlContent(calendarInfo.Location);
            htmlContent += $"<tr>\r\n                    <td class=\"leftTopic\">\r\n                        Calendar\r\n                    </td>\r\n                    <td>\r\n                        {MailboxAddress.Parse(replyAddress).Address}\r\n                    </td>\r\n                </tr>";

            int total = 1;
            int accept = 1;
            int decline = 0;
            int tentative = 0;
            int awaiting = 0;

            string attendees = string.Empty;
            if (calendarInfo.Attendees != null)
            {
                calendarInfo.Attendees.ForEach(v =>
                {
                    var orginAddress = MailboxAddress.Parse(calendarInfo.Organizer).Address;
                    var currentAddress = MailboxAddress.Parse(v.Value.ToString().Replace("mailto:", "")).Address;

                    if (!orginAddress.Equals(currentAddress))
                    {
                        total++;
                        switch (v.ParticipationStatus)
                        {
                            case "ACCEPTED":
                                accept++;
                                attendees += $"<div>{AcceptIcon} {currentAddress}</div>";
                                break;
                            case "DECLINED":
                                decline++;
                                attendees += $"<div>{DeclineIcon} {currentAddress}</div>";
                                break;
                            case "TENTATIVE":
                                tentative++;
                                attendees += $"<div>{TentativeIcon} {currentAddress}</div>";
                                break;
                            default:
                                awaiting++;
                                attendees += $"<div><f style=\"font-size: 10px;visibility:hidden;\" > {AcceptIcon} {currentAddress}</div>";
                                break;
                        }
                    }
                });
            }

            htmlContent += $"<tr>\r\n                    <td class=\"leftTopic\" style=\"vertical-align:top\">\r\n                        Who\r\n                    </td>\r\n                    <td>                        <div class=\"leftTopic\">{(total > 0 ? $"Total:{total}" : string.Empty)} {(accept > 0 ? $"Yes:{accept}" : string.Empty)} {(decline > 0 ? $"No:{decline}" : string.Empty)} {(tentative > 0 ? $"Maybe:{tentative}" : string.Empty)} {(awaiting > 0 ? $"Awaiting:{awaiting}" : string.Empty)}</div><div>{AcceptIcon} {calendarInfo.Organizer} <f style=\"color: #666;\">- organizer</f></div>{attendees} </td>\r\n                </tr>\r\n            </table>";
            htmlContent += "\r\n        </div>\r\n    </div>\r\n</body>\r\n</html>";
            return htmlContent;
        }

        private string LocationHtmlContent(string location)
        {
            if (!string.IsNullOrEmpty(location))
            {
                return $"<tr>\r\n                    <td class=\"leftTopic\">\r\n                        Where\r\n                    </td>\r\n                    <td>\r\n                        {location}\r\n                    </td>\r\n                </tr>";
            }

            return string.Empty;
        }

        private string MarkHtmlContent(string mark)
        {
            if (!string.IsNullOrEmpty(mark))
            {
                return $" <div style=\"width: 400px;\">\r\n            {mark}</div>";
            }

            return string.Empty;
        }

        private string DateTimeHtmlContent(DateTime originStart, DateTime originEnd)
        {
            string showDateTime = string.Empty;

            if (originStart.ToLocalTime().Day != originEnd.ToLocalTime().Day)
            {
                DateTime start = originStart.ToLocalTime();
                DateTime end = originEnd.ToLocalTime();
                string dayOfWeek = start.DayOfWeek.ToString();
                string endDayOfWeek = end.DayOfWeek.ToString();
                showDateTime = $"{start.ToString("yyyy MM-dd")} ({dayOfWeek}) {start.ToString("HH:mm")} — {end.ToString("yyyy MM-dd")} ({endDayOfWeek}) {end.ToString("HH:mm")}";
            }
            else
            {
                DateTime start = originStart.ToLocalTime();
                DateTime end = originEnd.ToLocalTime();
                string dayOfWeek = start.DayOfWeek.ToString();
                showDateTime = $"{start.ToString("yyyy MM-dd")} ({dayOfWeek}) {start.ToString("HH:mm")} — {end.ToString("HH:mm")}";
            }

            string htmlContent = $"<table>\r\n                <tr>\r\n                    <td class=\"leftTopic\" width=\"80px\">\r\n                        When\r\n                    </td>\r\n                    <td>\r\n                        {showDateTime} \r\n                    </td>\r\n                </tr>";
            return htmlContent;
        }

        private string TipHtmlContent(int replyStatus)
        {
            switch (replyStatus)
            {
                case AcceptReply:
                    return "<div class=\"tip\">\r\n                You have accepted this invitation.\r\n            </div>";
                case DeclineReply:
                    return "<div class=\"tip\">\r\n                You have declined this invitation.\r\n            </div>";
                case TentativeReply:
                    return "<div class=\"tip\">\r\n                You have tentatively accepted this invitation.\r\n            </div>";
                default:
                    return "<div class=\"tip\">\r\n                The invitation has expired.\r\n            </div>";
            }
        }

        private void ReplyMessageToOrganizer(string messageId, string organizer, string replyAddress, int participationStatus, User user)
        {
            var maildb = Kooboo.Mail.Factory.DBFactory.UserMailDb(user);
            var orgdb = DBFactory.OrgDb(user.CurrentOrgId);
            var replyAddressOrgdb = DBFactory.OrgDb(AddressUtility.GetAddress(replyAddress));

            var msg = maildb.Message2.GetBySmtpMessageId(messageId);
            var context = new RenderContext()
            {
                User = user
            };

            List<string> rcpto = new List<string>
                {
                    organizer
                };

            //sender logicial
            var addressName = AddressUtility.GetDisplayName(replyAddress);
            EmailAddress emailAddress = null;
            if (replyAddressOrgdb != null)
            {
                emailAddress = replyAddressOrgdb.Email.Find(AddressUtility.GetAddress(replyAddress));
            }
            string replySender = string.Empty;

            //have not addressName
            if (string.IsNullOrEmpty(addressName))
            {
                if (emailAddress != null && !string.IsNullOrEmpty(emailAddress.Name))
                {
                    var replySenderUserId = emailAddress.UserId;
                    var replySenderUser = GlobalDb.Users.Get(replySenderUserId);
                    if (!string.IsNullOrEmpty(replySenderUser?.FirstName) && !string.IsNullOrEmpty(replySenderUser?.LastName))
                    {
                        // have fullName
                        replySender = $"\"{replySenderUser.FirstName} {replySenderUser.LastName}\" <{AddressUtility.GetAddress(replyAddress)}>";
                    }
                    else
                    {
                        // have not fullName
                        replySender = $"\"{emailAddress.Name}\" <{AddressUtility.GetAddress(replyAddress)}>";
                    }
                }
                else
                {
                    // have not fullName
                    var originAddress = AddressUtility.GetAddress(replyAddress);
                    var spans = originAddress.Split("@");
                    replySender = $"\"{spans[0]}\" <{originAddress}>";
                }
            }
            else
            {
                // have addressName
                replySender = $"\"{addressName}\" <{AddressUtility.GetAddress(replyAddress)}>";
            }

            string replyMessbody = Kooboo.Mail.Multipart.ReferenceComposer.ComposeCalendarPartStatReply(0, msg, participationStatus, replySender, context);
            var msginfo = Kooboo.Mail.Utility.MessageUtility.ParseMeta(replyMessbody);
            var replyUser = GlobalDb.Users.GetByEmail(msginfo.From);
            if (replyUser != null)
                msginfo.From = ComposeUtility.GetFrom(new ComposeViewModel() { From = EmailAddress.ToId(msginfo.From) }, replyUser);

            string fromaddress = Mail.Utility.AddressUtility.GetAddress(msginfo.From);
            Kooboo.Mail.Transport.Incoming.Receive(fromaddress, rcpto, replyMessbody);
        }

        private string DecodeJwtToGetPayload(string token)
        {
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            JwtDecoder decoder = new JwtDecoder(serializer, urlEncoder);
            string payload = decoder.Decode(token);
            return payload;
        }

        private string SignParticipationStatus(int replyStatus)
        {
            switch (replyStatus)
            {
                case AcceptReply:
                    return "ACCEPTED";
                case DeclineReply:
                    return "DECLINED";
                case TentativeReply:
                    return "TENTATIVE";
                default:
                    return "NEED-ACTION";
            }
        }
    }

    public class MailBodyReplyModel
    {
        public User User { get; set; }
        public string MessageId { get; set; }
        public string CalendarId { get; set; }
        public string ReplyAddress { get; set; }
        public int ReplyStatus { get; set; }
    }
}