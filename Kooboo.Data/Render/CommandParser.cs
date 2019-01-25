//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Data.Render
{
  public  class CommandParser
    {
       public  static string GetCommandName(Comment comment)
        { 
            return GetCommandName(comment.data);
        }

        public static string GetCommandName(string CommentLine)
        {
            int index = 0;
            return GetName(CommentLine, ref index);
        }

       public static Command ParseCommand(Comment comment)
        { 
            return ParseCommand(comment.data);
        }

       public static Command ParseCommand(string ComentCommandLine)
        {
            Command command = new Command(); 

            int index = 0;
            command.Name = GetName(ComentCommandLine, ref index);

            if (string.IsNullOrEmpty(command.Name))
            {
                return null; 
            }
            int totallen = ComentCommandLine.Length; 

            var attribute = GetAttribute(ref ComentCommandLine, ref index, totallen); 
            while (!string.IsNullOrEmpty(attribute.Key))
            {
                command.Attributes.Add(attribute.Key, attribute.Value);
                attribute = GetAttribute(ref ComentCommandLine, ref index, totallen);
            }

            return command; 

        }

       private static string GetName(string input, ref int index)
        {
            int len = input.Length;
            string name = string.Empty; 

            bool found = false;   
            while(index < len)
            { 
               var current = input[index];

                if (found)
                {
                    if (Kooboo.Lib.Helper.CharHelper.isSpaceCharacters(current) || current == '-')
                    {
                        break;
                    }
                    else
                    {
                        name += current; 
                    }
                }
                else
                {
                    if (current == '#')
                    {
                        found = true;
                    }
                }
                index += 1; 
            }
           if (found & !string.IsNullOrEmpty(name))
            {
                return name; 
            }

            return null; 
        }

       public static KeyValuePair<string, string> GetAttribute(ref string input, ref int index, int len)
        {
            string name=string.Empty;
            string value = string.Empty; 
            var state = AttributeState.beforeName;

            char quote = default(char);
            bool hasquote = false; 

            while(index <len)
            { 
                var current = input[index]; 
                if (state == AttributeState.beforeName)
                {
                   if (!NonData(current)) 
                    {
                        name += current;
                        state = AttributeState.InName; 
                    }
                }
                else if (state == AttributeState.InName)
                {
                    if (NonData(current))
                    {
                        state = AttributeState.AfterName;
                        index--; 
                        if(index <0)
                        {
                            index = 0; 
                        }
                    }
                    else
                    {
                        name += current; 
                    }
                }
                else if (state == AttributeState.AfterName)
                {
                    if (current == '=')
                    {
                        state = AttributeState.BeforeValue; 
                    }
                    else
                    {
                        if (!NonData(current))
                        {
                            break; 
                        }
                    }
                }
                else if (state == AttributeState.BeforeValue)
                {
                     if (!NonData(current))
                    {
                        if (current =='\'' ||current == '\"' )
                        {
                            hasquote = true;
                            quote = current;
                            state = AttributeState.InValue; 
                        }
                        else
                        {
                            value += current;
                            state = AttributeState.InValue; 
                        }
                    }
                }
                else if (state == AttributeState.InValue)
                {
                    if (hasquote)
                    {
                        if (current == quote)
                        {
                            break; 
                        }
                        else
                        {
                            value += current; 
                        }
                    }
                    else
                    {
                        if (NonData(current))
                        {
                            break; 
                        }
                        else
                        {
                            value += current; 
                        }
                    }

                }
                else
                {
                    break;
                }

                index += 1; 
            }
            index += 1; 
           return new KeyValuePair<string, string>(name, value); 
        }
        
       private static bool NonData(char current)
        {
          if (Kooboo.Lib.Helper.CharHelper.isSpaceCharacters(current) || current == '-'|| current == '=' || current == '>')
            {
                return true; 
            }
            return false; 
        }
    }

    public enum AttributeState
    {
        beforeName=0,
        InName = 1,
        AfterName=2,
        BeforeValue = 3,
        InValue=4
    }
}
