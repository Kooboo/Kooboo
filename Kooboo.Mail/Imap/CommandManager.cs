//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kooboo.Mail.Utility;

namespace Kooboo.Mail.Imap.Commands
{
    public static class CommandManager
    {
        private static List<ICommand> _list;

        private static object _locker = new object();

        private static List<ICommand> List
        {
            get
            {
                if (_list == null)
                {
                    lock (_locker)
                    {
                        if (_list == null)
                        {
                            var newList = new List<ICommand>();
                            newList.Add(new LOGIN());
                            newList.Add(new CAPABILITY());
                            newList.Add(new STARTTLS());
                            newList.Add(new LOGOUT());
                            newList.Add(new CLOSE());
                            newList.Add(new NOOP());
                            newList.Add(new CHECK());

                            // Folder
                            newList.Add(new SELECT());
                            newList.Add(new EXAMINE());
                            newList.Add(new ListCommand());
                            newList.Add(new LSUB());
                            newList.Add(new STATUS());
                            newList.Add(new CREATE());
                            newList.Add(new RENAME());
                            newList.Add(new DELETE());
                            newList.Add(new SUBSCRIBE());
                            newList.Add(new UNSUBSCRIBE());

                            // Message
                            newList.Add(new UID());
                            newList.Add(new FETCH());
                            newList.Add(new STORE());
                            newList.Add(new COPY());
                            newList.Add(new APPEND());
                            newList.Add(new EXPUNGE());
                            newList.Add(new SEARCHCmd());
                            _list = newList;
                        }

                    }

                    // Common

                }
                return _list;
            }
        }


        public static ICommand GetCommand(string CmdName)
        {
            return List.Find(o => o.CommandName == CmdName.ToUpper());
        }

        public static async Task Execute(ImapSession session, string RequestTag, string CommandName, string arg)
        {
            var command = GetCommand(CommandName);

            if (command == null)
            {
                await ResponseEnd(session, RequestTag, "NO", CommandName + " Command Not Supported");
                return;
            }

            if (command.RequireAuth && !session.IsAuthenticated)
            {
                await ResponseEnd(session, RequestTag, "NO", CommandName + " Authentication required");
                return;
            }

            if (command.RequireFolder && session.SelectFolder == null)
            {
                await ResponseEnd(session, RequestTag, "NO", CommandName + " Command requires Select state");
                return;
            }

            if (command.RequireTwoPartCommand)
            {
                string[] parts = TextUtils.SplitQuotedString(arg, ' ', true);
                if (parts.Length != 2)
                {
                    await ResponseEnd(session, RequestTag, "BAD", "Error in arguments, " + arg);
                    return;
                }
            }

            try
            {
                var response = await command.Execute(session, arg);
                await SendResponse(session, response);

                var message = "";

                if (!String.IsNullOrEmpty(command.AdditionalResponse))
                {
                    message += command.AdditionalResponse + " ";
                }

                message += CommandName + " Command Completed!";

                await ResponseEnd(session, RequestTag, "OK", message);

            }
            //catch (StartTlsException ex)
            //{ 

            //    await session.Stream.WriteLineAsync($"{RequestTag} OK Begin TLS negotiation now");

            //    await session.StartSecureConnection();
            //}
            //catch (SessionCloseException ex)
            //{
            //    await SendResponse(session, ex.Response);
            //    await ResponseEnd(session, RequestTag, "OK", $"{CommandName} Command Completed!");
            //    throw;
            //}
            catch (CommandException ex)
            {
                if (ex.Tag == null)
                {
                    ex.Tag = "BAD";
                }
                await ResponseEnd(session, RequestTag, ex.Tag, ex.ErrorMessage);

            }
            catch (System.IO.IOException ex)
            {
                //{"Unable to write data to the transport connection: Զ������ǿ�ȹر���һ�����е�����
                //
                throw ex;
            }
            catch (Exception ex)
            {
                await ResponseEnd(session, RequestTag, "BAD", ex.Message);
                throw ex;
            }
        }


        public static Task ResponseEnd(ImapSession session, string RequestTag, string TagResult, string message)
        {
            return session.Stream.WriteLineAsync(RequestTag + " " + TagResult + " " + message);
        }

        public static async Task SendResponse(ImapSession session, ImapResponse response)
        {

            if (!string.IsNullOrWhiteSpace(response.Line))
            {
                await session.Stream.WriteLineAsync(response.Line);
            }
            else if (response.Binary != null)
            {
                await session.Stream.WriteAsync(response.Binary);
            }

        }

        public static async Task SendResponse(ImapSession session, List<ImapResponse> response)
        {
            if (response != null && response.Count() > 0)
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
