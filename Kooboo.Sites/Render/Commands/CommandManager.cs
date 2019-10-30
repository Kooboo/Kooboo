//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.
using Kooboo.Dom;
using System;
using System.Collections.Generic;

namespace Kooboo.Sites.Render.Commands
{
    public class CommandManager
    {
        private static object _locker = new object();
        private static Dictionary<string, ICommand> _list;

        public static Dictionary<string, ICommand> List
        {
            get
            {
                lock (_locker)
                {
                    if (_list == null)
                    {
                        _list = new Dictionary<string, ICommand>(StringComparer.OrdinalIgnoreCase);
                        AddToList(_list, new LayoutCommand());
                        AddToList(_list, new IncludeCommand());
                    }
                    return _list;
                }
            }
        }

        private static void AddToList(Dictionary<string, ICommand> list, ICommand newcommand)
        {
            if (!list.ContainsKey(newcommand.Name))
            {
                list[newcommand.Name] = newcommand;
            }
        }

        public static bool IsCommand(string commentLine)
        {
            string name = CommandParser.GetCommandName(commentLine);
            return !string.IsNullOrEmpty(name) && List.ContainsKey(name);
        }

        public static bool IsCommand(Comment comment)
        {
            return comment != null && IsCommand(comment.data);
        }

        public static ICommand GetCommand(string name)
        {
            return List.ContainsKey(name) ? List[name] : null;
        }
    }
}