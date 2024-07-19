//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved. 
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Kooboo.Lib.Helper.EncodingHelper;
using Kooboo.Mail.Factory;
using Kooboo.Mail.Spam;

namespace Kooboo.Mail.Smtp
{
    public class SmtpSession
    {

        public int MaxRcptCount
        {
            get
            {
#if DEBUG
                return 2;
#endif
                return 50;
            }
        }

        public int MaxRcptError
        {
            get
            {
#if DEBUG
                return 1;
#endif
                return 3;
            }
        }

        public int RcptCounter = 0;
        public int RcptErrorCounter = 0;

        public SmtpSession(System.Net.IPAddress remoteIP)
        {
            this.ClientIP = remoteIP?.ToString();
            this.RemoteIp = remoteIP;
        }

        public void ReSet()
        {
            //this.IsAuthenticated = false;
            //this.ClientIP = null; reset are only for the same Client. 
            //this.UserName = null;
            //this.Password = null;
            this._buffer = null;
            this._MessageBody = null;
            this.Log.Clear();
            this.State = CommandState.Body;
            // this.OrganizationId = default(Guid);
            this.RcptCounter = 0;
            this.RcptErrorCounter = 0;
            this.TestResult = null;
        }

        public bool IsAuthenticated { get; set; }

        public string ClientIP { get; set; }

        public System.Net.IPAddress RemoteIp { get; set; }

        public string ClientHostName { get; set; }

        public string UserName { get; set; }

        public Guid OrganizationId { get; set; }

        public string Password { get; set; }

        public Dictionary<SmtpCommand, SmtpResponse> Log = new Dictionary<SmtpCommand, SmtpResponse>();

        private StringBuilder _buffer;
        private StringBuilder buffer
        {
            get
            {
                if (_buffer == null)
                {
                    _buffer = new StringBuilder();
                }
                return _buffer;
            }
            set
            {
                _buffer = value;
            }
        }

        private string _MessageBody;
        public string MessageBody
        {
            get
            {
                if (_MessageBody == null)
                {
                    if (_buffer != null)
                    {
                        string body = _buffer.ToString().Replace("\r\n..", "\r\n.");
                        _buffer = null;
                        if (!string.IsNullOrEmpty(body))
                        {
                            _MessageBody = body;
                        }
                    }
                }

                return _MessageBody;

            }
            set { _MessageBody = value; }
        }
        public bool HasMessageBody
        {
            get
            {
                return _MessageBody != null;
            }
        }

        public SessionResult TestResult { get; set; }

        public CommandState State { get; set; } = CommandState.Helo;

        public SmtpResponse Command(string CommandLine)
        {
            // regular state.. 
            if (this.State == CommandState.Data)
            {
                return DataCommand(CommandLine);
            }
            else if (this.State == CommandState.Auth || this.State == CommandState.AuthUser || this.State == CommandState.AuthPass)
            {
                return AuthCommand(CommandLine);
            }
            else if (this.State == CommandState.Body)
            {
                return BodyCommand(CommandLine);
            }
            else
            {
                return HeloCommand(CommandLine);
            }
        }

        private string GetString(byte[] data, EmailEncodingResult encodingresult)
        {
            System.Text.Encoding encoding = null;
            if (encodingresult != null && !string.IsNullOrEmpty(encodingresult.Charset))
            {
                encoding = System.Text.Encoding.GetEncoding(encodingresult.Charset);
            }

            if (encoding == null)
            {
                encoding = System.Text.Encoding.UTF8;
            }

            string text = encoding.GetString(data);

            if (text != null && encodingresult != null && !string.IsNullOrWhiteSpace(encodingresult.CharSetText))
            {
                text = text.Replace(encodingresult.CharSetText, "charset=utf-8");
            }
            return text;
        }


        public async Task<SmtpResponse> Data(byte[] data)
        {
            SmtpResponse response = new SmtpResponse();

            var encodingresult = Lib.Helper.EncodingDetector.GetEmailEncoding(data);
            string text = GetString(data, encodingresult);

            text = text.Replace("\r\n..", "\r\n.");

            // valide email message.  

            if (text.Length > Setting.MaxSmtpDataSize * 1.3)
            {
                response.Code = 550;
                response.Message = "email size too big, max size: " + Setting.MaxSmtpSizeString;
            }
            else if (this.IsAuthenticated && Kooboo.Mail.SecurityControl.SpamTest.instance.IsSpam(text))
            {
                // Authenticated == this is for sending email out using IMAP client. 
                response.Code = 550;
                response.Message = "Our spam filter detect that this may be an SPAM, if this is a mistake, let us know!";

                response.Close = true;
                // SecurityControl.BlackList.AddViolation(this.ClientIP);
            }

            else if (!string.IsNullOrWhiteSpace(text))
            {
                this.MessageBody = text;
                this._buffer = null;

                this.TestResult = await SessionVerifier.Instance.ValidateSession(this);

                response.Code = 250;
                response.Message = "Message queued";
                response.SessionCompleted = true;
            }
            else
            {
                response.Code = 550;
                response.Message = "Message body not found";
            }

            this.Log.Add(new SmtpCommand() { CommandLine = ".", Name = SmtpCommandName.ENDDOT }, response);

            this.State = CommandState.Helo;
            return response;
        }

        public SmtpResponse ServiceReady()
        {
            // "220 Kooboo Smtp Server is ready"
            return new SmtpResponse() { Code = 220, Message = "Kooboo Smtp Server is ready" };
        }

        internal SmtpResponse StartTls()
        {
            /// Once the response 220 is received from the server, the SMTP client should send HELO or EHLO to launch the session.In the case of a negative response(454), the client must decide whether to continue the SMTP session or not.

            // We always support now.

            var response = new SmtpResponse();

            response.Code = 220;
            response.Message = "Go Ahead with SSL";

            response.StartTls = true;
            response.SendResponse = true;

            return response;
        }

        internal SmtpResponse BodyCommand(string CommandLine)
        {
            var response = new SmtpResponse();
            var command = SmtpCommand.Parse(CommandLine);

            if (command.Name == SmtpCommandName.AUTHLOGIN)
            {
                if (string.IsNullOrEmpty(command.Value))
                {
                    this.State = CommandState.Auth;
                    response.Code = 334;
                    response.Message = Convert.ToBase64String(Encoding.ASCII.GetBytes("Username:"));
                }
                else
                {
                    this.State = CommandState.AuthUser;
                    this.UserName = command.Value;
                    response.Code = 334;
                    response.Message = Convert.ToBase64String(Encoding.ASCII.GetBytes("Password:"));
                }
            }
            else if (command.Name == SmtpCommandName.MAILFROM)
            {
                if (string.IsNullOrEmpty(command.Value))
                {
                    response.Code = 550;
                    response.Message = "Sender address must be specified";
                }
                else if (!Utility.SmtpUtility.IsValidMailFrom(command.Value))
                {
                    response.Code = 550;
                    response.Message = "Invalid Sender address";
                }
                else
                {
                    response.Code = 250;
                    response.Message = "Sender OK";
                }
            }
            else if (command.Name == SmtpCommandName.RCPTTO)
            {
                this.RcptCounter += 1;

                if (this.RcptCounter > this.MaxRcptCount)
                {
                    Kooboo.Mail.SecurityControl.BlackList.AddViolation(this.ClientIP);
                    response.Code = 550;
                    response.Message = "Recipient address too many";
                    response.Close = true;
                }
                else if (this.RcptErrorCounter >= this.MaxRcptError)
                {
                    Kooboo.Mail.SecurityControl.BlackList.AddViolation(this.ClientIP);
                    response.Code = 550;
                    response.Message = "Too many recipient address failure";
                    response.Close = true;
                }

                else if (string.IsNullOrEmpty(command.Value))
                {
                    response.Code = 550;
                    response.Message = "Recipient address must be specified";
                }
                else
                {
                    var validateResult = ValidateRecipient(command.Value);
                    if (validateResult.IsOkToSend)
                    {
                        response.Code = 250;
                        response.Message = "Recipient OK";
                    }
                    else
                    {
                        this.RcptErrorCounter += 1;
                        response.Code = 550;
                        response.Message = validateResult.ErrorMessage;
                    }
                }
            }

            else if (command.Name == SmtpCommandName.DATA)
            {
                //public static string DataStart = "354 "; 
                this.State = CommandState.Data;
                response.Code = 354;
                response.Message = "End data with <CLRF>.<CLRF>";
            }
            else if (command.Name == SmtpCommandName.QUIT)
            {
                response.Code = 221;
                response.Message = "Goodbye";
                response.Close = true;
            }
            else if (command.Name == SmtpCommandName.STARTTLS)
            {
                return StartTls();
            }
            else
            {
                response.Code = 550;
                response.Message = "Invalid command";
            }
            this.Log.Add(command, response);
            return response;
        }

        private bool HasFromToAddress()
        {
            bool hasfrom = false;
            bool hasto = false;
            foreach (var item in Log)
            {
                if (item.Key.Name == SmtpCommandName.MAILFROM && item.Value.Code == 250)
                {
                    hasfrom = true;
                }

                if (item.Key.Name == SmtpCommandName.RCPTTO && item.Value.Code == 250)
                {
                    hasto = true;
                }
            }

            return hasfrom && hasto;
        }



        public RecipientValidationResult ValidateRecipient(string rcptToAddress)
        {
            RecipientValidationResult result = new RecipientValidationResult();

            string address = Utility.AddressUtility.GetAddress(rcptToAddress);
            result.IsValidEmailAddressFormat = Utility.AddressUtility.IsValidEmailAddress(address);
            if (!result.IsValidEmailAddressFormat)
            {
                result.IsOkToSend = false;
                result.ErrorMessage = "Invalid recipient address format";
                return result;
            }

            if (Utility.AddressUtility.IsLocalEmailAddress(address))
            {
                result.IsOkToSend = true;
                result.IsLocalEmail = true;
                return result;
            }

            if (this.IsAuthenticated)
            {
                // valid the mail from... MUST be local email.   
                var mailfrom = Transport.Incoming.GetMailFrom(this);

                if (Utility.AddressUtility.IsLocalEmailAddress(mailfrom))
                {
                    result.IsAuthenticated = true;
                    result.IsOkToSend = true;
                }
                else
                {
                    result.IsAuthenticated = true;
                    result.IsOkToSend = false;
                    result.ErrorMessage = "mail from address not hosted at our server";
                }
                return result;
            }

            if (Lib.Helper.IPHelper.IsLocalIp(this.ClientIP) || Kooboo.Data.Helper.ApiHelper.IsOnlineSever(this.ClientIP))
            {
                result.IsValidServer = true;
                result.IsOkToSend = true;
                return result;
            }
            else
            {
                result.IsOkToSend = false;
                result.ErrorMessage = "Relay not allowed";
                return result;
            }
        }

        internal SmtpResponse HeloCommand(string CommandLine)
        {
            var response = new SmtpResponse();
            var command = SmtpCommand.Parse(CommandLine);

            // TODO validate the incoming domain... 
            if (command.Name == SmtpCommandName.HELO)
            {
                if (!string.IsNullOrEmpty(command.Value))
                {
                    response.Code = 250;
                    response.Message = "Hello " + command.Value + " Kooboo Smtp Server";
                    this.State = CommandState.Body;
                    this.ClientHostName = command.Value;
                }
                else
                {
                    response.Code = 501;
                    response.Message = "Hostname required";
                }
            }
            else if (command.Name == SmtpCommandName.EHLO)
            {
                if (!string.IsNullOrEmpty(command.Value))
                {
                    response.Code = 250;
                    response.Seperator = '-';
                    response.Message = "Hello " + command.Value + "\r\n250-SIZE " + Setting.MaxSmtpSizeString + "\r\n250-STARTTLS\r\n250-AUTH LOGIN\r\n250 OK";
                    this.State = CommandState.Body;
                    this.ClientHostName = command.Value;
                }
                else
                {
                    response.Code = 501;
                    response.Message = "Hostname required";
                }
            }
            else if (command.Name == SmtpCommandName.STARTTLS)
            {
                return StartTls();
            }
            else if (command.Name == SmtpCommandName.UNKNOWN)
            {
                response.Code = 550;
                response.Message = "bad command";
            }
            else if (command.Name == SmtpCommandName.QUIT)
            {
                response.Code = 221;
                response.Message = "Goodbye";
                response.Close = true;
            }
            else
            {
                response.Code = 502;
                response.Message = "HELO or EHLO first";
            }
            this.Log.Add(command, response);
            return response;
        }

        internal SmtpResponse DataCommand(string CommandLine)
        {
            if (CommandLine == ".")
            {
                SmtpResponse response = new SmtpResponse();

                string msgbody = null;

                if (_buffer != null)
                {
                    msgbody = _buffer.ToString().Replace("\r\n..", "\r\n.");
                }

                if (!string.IsNullOrEmpty(msgbody))
                {
                    this.MessageBody = msgbody;
                    this._buffer = null;
                    response.Code = 250;
                    response.Message = "Message queued";
                    response.SessionCompleted = true;
                }
                else
                {
                    response.Code = 550;
                    response.Message = "Message body not found";
                }

                this.Log.Add(new SmtpCommand() { CommandLine = CommandLine, Name = SmtpCommandName.ENDDOT }, response);

                this.State = CommandState.Helo;
                return response;
            }
            else
            {
                if (CommandLine == "\r\n")
                {
                    this.buffer.AppendLine();
                }
                else
                {
                    this.buffer.AppendLine(CommandLine);
                }
                return new SmtpResponse() { SendResponse = false };
            }

        }

        internal SmtpResponse AuthCommand(string CommandLine)
        {
            var response = new SmtpResponse();

            if (this.State == CommandState.Auth)
            {
                this.State = CommandState.AuthUser;

                this.UserName = Encoding.ASCII.GetString(Convert.FromBase64String(CommandLine));
                response.Code = 334;
                response.Message = Convert.ToBase64String(Encoding.ASCII.GetBytes("Password:"));
            }
            else if (this.State == CommandState.AuthUser)
            {
                this.State = CommandState.Body;
                this.Password = Encoding.ASCII.GetString(Convert.FromBase64String(CommandLine));

                string username = this.UserName;

                if (Kooboo.Data.Service.UserLoginProtection.CanTryLogin(username, this.ClientIP, this.Password))
                {
                    var emailAddress = Utility.AddressUtility.GetEmailAddress(username);
                    var orgdb = DBFactory.OrgDb(emailAddress.OrgId);
                    var validate = orgdb.Email.LoginByAuthorizationCode(emailAddress.Address, this.Password);
                    var user = Kooboo.Data.GlobalDb.Users.Get(emailAddress.UserId);

                    IsAuthenticated = validate;

                    if (this.OrganizationId == default(Guid) && user != null)
                    {
                        this.OrganizationId = user.CurrentOrgId;
                    }
                }

                if (IsAuthenticated)
                {
                    response.Code = 235;
                    response.Message = "Authentication completed";

                    Kooboo.Data.Service.UserLoginProtection.AddLoginOK(username, this.ClientIP);
                }
                else
                {
                    response.Code = 535;
                    response.Message = "Authentication failed";

                    Kooboo.Data.Service.UserLoginProtection.AddLoginFail(username, this.ClientIP);
                }

            }

            return response;
        }

        public enum CommandState
        {
            Helo = 0,
            Body = 2,
            Auth = 3,
            AuthUser = 4,
            AuthPass = 5,
            Data = 7
        }


        public class RecipientValidationResult
        {
            public bool IsOkToSend { get; set; }

            public bool IsValidServer { get; set; }

            public bool IsLocalEmail { get; set; }

            public bool IsAuthenticated { get; set; }

            public bool IsValidEmailAddressFormat { get; set; }

            public string ErrorMessage { get; set; }

        }
    }

}
