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
  public static   class CommandManager
    {
        private static List<ICommand> _list; 

        private static List<ICommand> List
        {
            get
            {
                if (_list == null)
                {
                    // Common
                    _list = new List<ICommand>();
                    _list.Add(new LOGIN());
                    _list.Add(new CAPABILITY());
                    _list.Add(new STARTTLS());
                    _list.Add(new LOGOUT());
                    _list.Add(new CLOSE());
                    _list.Add(new NOOP());
                    _list.Add(new CHECK());

                    // Folder
                    _list.Add(new SELECT()); 
                    _list.Add(new ListCommand());
                    _list.Add(new LSUB());  
                    _list.Add(new STATUS());
                    _list.Add(new CREATE());
                    _list.Add(new RENAME());
                    _list.Add(new DELETE());
                    _list.Add(new SUBSCRIBE());
                    _list.Add(new UNSUBSCRIBE());

                    // Message
                    _list.Add(new UID());
                    _list.Add(new FETCH());
                    _list.Add(new STORE());
                    _list.Add(new COPY());
                    _list.Add(new APPEND());
                    _list.Add(new EXPUNGE());
                    _list.Add(new SEARCHCmd());

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
                await ResponseEnd(session, RequestTag, "NO",  CommandName +  " Command Not Supported");
                return; 
            }

           if (command.RequireAuth && !session.IsAuthenticated)
            {
                await ResponseEnd(session, RequestTag, "NO", CommandName +  " Authentication required");
                return; 
            }

           if (command.RequireFolder &&  session.SelectFolder == null)
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

                var message = new StringBuilder();
                if (!String.IsNullOrEmpty(command.AdditionalResponse))
                {
                    message.Append(command.AdditionalResponse).Append(" ");
                }
                message.Append(CommandName).Append(" Command Completed!");

                await ResponseEnd(session, RequestTag, "OK", message.ToString());

            }
            catch (StartTlsException)
            {
                await session.Stream.WriteLineAsync($"{RequestTag} OK Begin TLS negotiation now");

                await session.StartSecureConnection();
            }
            catch (SessionCloseException ex)
            {
                await SendResponse(session, ex.Response);
                await ResponseEnd(session, RequestTag, "OK", $"{CommandName} Command Completed!");
                throw;
            }
            catch (CommandException ex)
            {
                if (ex.Tag == null)
                {
                    ex.Tag = "BAD"; 
                } 
                await ResponseEnd(session, RequestTag, ex.Tag, ex.ErrorMessage);
            }
            catch (Exception ex)
            {
                await ResponseEnd(session, RequestTag, "BAD", ex.Message);
            } 
        }

         
        public static Task ResponseEnd(ImapSession session, string RequestTag, string TagResult, string message)
        {
            return session.Stream.WriteLineAsync(RequestTag + " " + TagResult + " " + message);
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
