//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Mail.Smtp
{

    public class SmtpCommand
    {
        public SmtpCommandName Name { get; set; }

        public string Value { get; set; }

        public string CommandLine { get; set; }

        public static SmtpCommand Parse(string CommandLine)
        {
            if (string.IsNullOrWhiteSpace(CommandLine))
            {
                return new SmtpCommand() { Name = SmtpCommandName.UNKNOWN, CommandLine = CommandLine };
            }

            var scanner = new CommandScanner(CommandLine);
            var token = scanner.ConsumeNext();

            if (token == null)
            {
                return new SmtpCommand() { Name = SmtpCommandName.UNKNOWN, CommandLine = CommandLine };
            }
            token = token.ToUpper();

            var result = new SmtpCommand() { CommandLine = CommandLine, Name = SmtpCommandName.UNKNOWN };

            if (token == "HELO")
            {
                result.Name = SmtpCommandName.HELO;
                result.Value = scanner.ConsumeRest();
            }
            else if (token == "EHLO")
            {
                result.Name = SmtpCommandName.EHLO;
                result.Value = scanner.ConsumeRest();
            }
            else if (token == "AUTH")
            {
                var next = scanner.ConsumeNext();
                if (next != null && next.ToUpper() == "LOGIN")
                {
                    result.Name = SmtpCommandName.AUTHLOGIN;
                    result.Value = scanner.ConsumeRest();
                }
            }
            else if (token == "MAIL")
            {
                var next = scanner.ConsumeNext();
                if (next != null && next.ToUpper() == "FROM")
                {
                    result.Name = SmtpCommandName.MAILFROM;
                    result.Value = scanner.ConsumeRestAfterComma();
                }
            }
            else if (token == "RCPT")
            {
                var next = scanner.ConsumeNext();
                if (next != null && next.ToUpper() == "TO")
                {
                    result.Name = SmtpCommandName.RCPTTO;
                    result.Value = scanner.ConsumeRestAfterComma();
                }
            }
            else if (token == "DATA")
            {
                result.Name = SmtpCommandName.DATA;
                result.Value = scanner.ConsumeRest();
            }
            else if (token == "QUIT")
            {
                result.Name = SmtpCommandName.QUIT;
                result.Value = scanner.ConsumeRest();
            }
            else if (token == "RSET")
            {
                result.Name = SmtpCommandName.RSET;
                result.Value = scanner.ConsumeRest();
            }
            else if (token == "HELP")
            {
                result.Name = SmtpCommandName.HELP;
                result.Value = scanner.ConsumeRest();
            }
            else if (token == "VRFY")
            {
                result.Name = SmtpCommandName.VRFY;
                result.Value = scanner.ConsumeRest();
            }
            else if (token == "STARTTLS")
            {
                result.Name = SmtpCommandName.STARTTLS;
                result.Value = scanner.ConsumeRest();
            }

            return result;
        }

    }

    public enum SmtpCommandName
    {
        UNKNOWN = 0,
        HELO = 1,
        EHLO = 2,
        MAILFROM = 3,
        RCPTTO = 4,
        DATA = 5,
        RSET = 6,
        TURN = 7,
        SIZE = 8,
        PIPELINING = 9,
        VRFY = 10,
        HELP = 11,
        AUTHLOGIN = 12,
        QUIT = 13,
        BDAT = 14,
        ENDDOT = 15,
        STARTTLS = 16
    }
}



//HELO
//Sent by a client to identify itself, usually with a domain name.
//EHLO
//Enables the server to identify its support for Extended Simple Mail Transfer Protocol (ESMTP) commands.
//MAIL FROM
//Identifies the sender of the message; used in the form MAIL FROM:.
//RCPT TO
//Identifies the message recipients; used in the form RCPT TO:.
//TURN
//Allows the client and server to switch roles and send mail in the reverse direction without having to establish a new connection.
//ATRN
//The ATRN(Authenticated TURN) command optionally takes one or more domains as a parameter.The ATRN command must be rejected if the session has not been authenticated.
//SIZE
//Provides a mechanism by which the SMTP server can indicate the maximum size message supported. Compliant servers must provide size extensions to indicate the maximum size message that can be accepted. Clients should not send messages that are larger than the size indicated by the server.
//ETRN
//An extension of SMTP.ETRN is sent by an SMTP server to request that another server send any e-mail messages that it has.
//PIPELINING
//Provides the ability to send a stream of commands without waiting for a response after each command.
//CHUNKING
//An ESMTP command that replaces the DATA command.So that the SMTP host does not have to continuously scan for the end of the data, this command sends a BDAT command with an argument that contains the total number of bytes in a message. The receiving server counts the bytes in the message and, when the message size equals the value sent by the BDAT command, the server assumes it has received all of the message data.
//DATA
//Sent by a client to initiate the transfer of message content.
//DSN
//An ESMTP command that enables delivery status notifications.
//RSET
//Nullifies the entire message transaction and resets the buffer.
//VRFY
//Verifies that a mailbox is available for message delivery; for example, vrfy ted verifies that a mailbox for Ted resides on the local server.This command is off by default in Exchange implementations.
//HELP
//Returns a list of commands that are supported by the SMTP service.
//QUIT
//Terminates the session.