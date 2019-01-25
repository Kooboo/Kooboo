//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Render.Commands
{
  public  class CommandManager
    {
        private static object _locker = new object(); 
        private static Dictionary<string, ICommand>  _List; 
       public static Dictionary<string, ICommand> List
        {
            get
            {
                lock (_locker)
                {
                    if (_List == null)
                    {
                        _List = new Dictionary<string, ICommand>(StringComparer.OrdinalIgnoreCase);
                        AddToList(_List, new LayoutCommand());
                        AddToList(_List, new IncludeCommand());  
                    }
                    return _List;
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

        public static bool IsCommand(string CommentLine)
        {
            string name = CommandParser.GetCommandName(CommentLine); 
            if (string.IsNullOrEmpty(name))
            {
                return false; 
            } 
            return List.ContainsKey(name); 
        }

        public static bool IsCommand(Comment comment)
        {
            if (comment == null)
            {
                return false; 
            }
            return IsCommand(comment.data); 
        }

        public static ICommand GetCommand(string Name)
        {
            if (List.ContainsKey(Name))
            {
                return List[Name]; 
            }
            return null; 
        }
    }
}
