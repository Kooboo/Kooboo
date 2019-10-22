//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using LumiSoft.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Mail.Imap.Commands
{
    public static class CommandManager
    {
        private static List<ICommand> _list;

        private static List<ICommand> List
        {
            get
            {
                return _list ?? (_list = new List<ICommand>
                {
                    new LOGIN(),
                    new CAPABILITY(),
                    new STARTTLS(),
                    new LOGOUT(),
                    new CLOSE(),
                    new NOOP(),
                    new CHECK(),
                    new SELECT(),
                    new ListCommand(),
                    new LSUB(),
                    new STATUS(),
                    new CREATE(),
                    new RENAME(),
                    new DELETE(),
                    new SUBSCRIBE(),
                    new UNSUBSCRIBE(),
                    new UID(),
                    new FETCH(),
                    new STORE(),
                    new COPY(),
                    new APPEND(),
                    new EXPUNGE(),
                    new SEARCHCmd()
                });
            }
        }

        public static ICommand GetCommand(string cmdName)
        {
            return List.Find(o => o.CommandName == cmdName.ToUpper());
        }

        public static async Task Execute(ImapSession session, string requestTag, string commandName, string arg)
        {
            var command = GetCommand(commandName);

            if (command == null)
            {
                await ResponseEnd(session, requestTag, "NO", commandName + " Command Not Supported");
                return;
            }

            if (command.RequireAuth && !session.IsAuthenticated)
            {
                await ResponseEnd(session, requestTag, "NO", commandName + " Authentication required");
                return;
            }

            if (command.RequireFolder && session.SelectFolder == null)
            {
                await ResponseEnd(session, requestTag, "NO", commandName + " Command requires Select state");
                return;
            }

            if (command.RequireTwoPartCommand)
            {
                string[] parts = TextUtils.SplitQuotedString(arg, ' ', true);
                if (parts.Length != 2)
                {
                    await ResponseEnd(session, requestTag, "BAD", "Error in arguments, " + arg);
                    return;
                }
            }

            try
            {
                var response = await command.Execute(session, arg);
                await SendResponse(session, response);

                var message = new StringBuilder();
                if (!String.IsNullOrEmpty(command.AdditionalResponse))
                {
                    message.Append(command.AdditionalResponse).Append(" ");
                }
                message.Append(commandName).Append(" Command Completed!");

                await ResponseEnd(session, requestTag, "OK", message.ToString());
            }
            catch (StartTlsException)
            {
                await session.Stream.WriteLineAsync($"{requestTag} OK Begin TLS negotiation now");

                await session.StartSecureConnection();
            }
            catch (SessionCloseException ex)
            {
                await SendResponse(session, ex.Response);
                await ResponseEnd(session, requestTag, "OK", $"{commandName} Command Completed!");
                throw;
            }
            catch (CommandException ex)
            {
                if (ex.Tag == null)
                {
                    ex.Tag = "BAD";
                }
                await ResponseEnd(session, requestTag, ex.Tag, ex.ErrorMessage);
            }
            catch (Exception ex)
            {
                await ResponseEnd(session, requestTag, "BAD", ex.Message);
            }
        }

        public static Task ResponseEnd(ImapSession session, string requestTag, string tagResult, string message)
        {
            return session.Stream.WriteLineAsync(requestTag + " " + tagResult + " " + message);
        }

        public static async Task SendResponse(ImapSession session, ImapResponse response)
        {
            if (response.Line == null)
            {
                await session.Stream.WriteAsync(response.Binary);
            }
            else
            {
                await session.Stream.WriteLineAsync(response.Line);
            }
        }

        public static async Task SendResponse(ImapSession session, List<ImapResponse> response)
        {
            if (response != null && response.Any())
            {
                foreach (var item in response)
                {
                    // send partial response..
                    await SendResponse(session, item);
                }
            }
        }
    }
}