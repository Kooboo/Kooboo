//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Lib.Helper.EncodingHelper;
using Kooboo.Mail.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Mail.Smtp
{
    public class SmtpSession
    {
        public SmtpSession()
        {

        }

        public SmtpSession(string clientIp)
        {
            this.ClientIP = clientIp;
        }

        public void ReSet()
        {
            //this.IsAuthenticated = false;
            //this.ClientIP = null; reset are only for the same Client. 
            //this.UserName = null;
            // this.Password = null;
            this._buffer = null;
            this._MessageBody = null;
            this.Log.Clear();
            this.State = CommandState.Body;
            // this.OrganizationId = default(Guid);
        }

        public bool IsAuthenticated { get; set; }

        public string ClientIP { get; set; }

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


        public SmtpResponse Data(byte[] data)
        {
            SmtpResponse response = new SmtpResponse();

            var encodingresult = Lib.Helper.EncodingDetector.GetEmailEncoding(data);

            string text = GetString(data, encodingresult);
            text = text.Replace("\r\n..", "\r\n.");

            if (!string.IsNullOrWhiteSpace(text))
            {
                this.MessageBody = text;
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

            this.Log.Add(new SmtpCommand() { CommandLine = ".", Name = SmtpCommandName.ENDDOT }, response);

            this.State = CommandState.Helo;
            return response;
        }

        public SmtpResponse ServiceReady()
        {
            // "220 Kooboo Smtp Server is ready"
            return new SmtpResponse() { Code = 220, Message = "Kooboo Smtp Server is ready" };
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
                else if (!ValidMailFrom(command.Value))
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
                if (string.IsNullOrEmpty(command.Value))
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

        internal bool ValidMailFrom(string fromAddress)
        {
            string address = Utility.AddressUtility.GetAddress(fromAddress);
            return Utility.AddressUtility.IsValidEmailAddress(address);
        }

        //internal bool ValidateRecipient(string rcptToAddress)
        //{
        //    string address = Utility.AddressUtility.GetAddress(rcptToAddress);
        //    var validate = Utility.AddressUtility.IsValidEmailAddress(address);
        //    if (!validate)
        //    {
        //        return false;
        //    }

        //    if (!this.IsAuthenticated)
        //    {
        //        // if (!Utility.AddressUtility.IsOrganizationOk(address))
        //        // { 
        //        // check if it is allowed server.   
        //        if (Lib.Helper.IPHelper.IsLocalIp(this.ClientIP) || Kooboo.Data.Helper.ApiHelper.IsOnlineSever(this.ClientIP))
        //        {
        //            this.IsAuthenticated = true;     
        //            return true;
        //        }

        //        else
        //        {

        //            if (Utility.AddressUtility.IsOrganizationOk(address))
        //            {                 
        //                return true;
        //            }
        //            else
        //            {
        //                return false;
        //            }  
        //        }
        //        //} 
        //    }


        //    return true;
        //}

        public RecipientValidationResult ValidateRecipient(string rcptToAddress)
        {
            RecipientValidationResult result = new RecipientValidationResult();

            string address = Utility.AddressUtility.GetAddress(rcptToAddress);
            result.IsValidEmailAddressFormat = Utility.AddressUtility.IsValidEmailAddress(address);
            if (!result.IsValidEmailAddressFormat)
            {
                result.IsOkToSend = false;
                result.ErrorMessage = "Invalid recipient address";
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
                    result.ErrorMessage = "mail from address not local";
                }

                return result;
            }

            if (Lib.Helper.IPHelper.IsLocalIp(this.ClientIP) || Kooboo.Data.Helper.ApiHelper.IsOnlineSever(this.ClientIP))
            {
                result.IsValidServer = true;
                result.IsOkToSend = true;
                return result;
            }

            result.IsOkToSend = false;
            result.ErrorMessage = "Invalid recipient address";

            return result;
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
                    response.Message = "Hello " + command.Value + "\r\n250-SIZE 20480000\r\n250-AUTH LOGIN\r\n250 OK";
                    this.State = CommandState.Body;
                    this.ClientHostName = command.Value;
                }
                else
                {
                    response.Code = 501;
                    response.Message = "Hostname required";
                }
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

                if (Kooboo.Data.Service.UserLoginProtection.CanTryLogin(username, this.ClientIP))
                { 

                    if (this.UserName.IndexOf("@") > -1 && Utility.AddressUtility.IsValidEmailAddress(this.UserName))
                    {
                        var emailaddressObj = Utility.AddressUtility.GetEmailAddress(this.UserName);

                        if (emailaddressObj != null)
                        {
                            username = Kooboo.Data.GlobalDb.Users.GetUserName(emailaddressObj.UserId);
                            this.OrganizationId = emailaddressObj.OrgId;
                        }
                    }

                    var user = Kooboo.Data.GlobalDb.Users.Validate(username, this.Password);

                    IsAuthenticated = user != null;

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
