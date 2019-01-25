//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kooboo.Sites.Service
{
  public static  class GroupService
    {
        public static string GetUrl(string name, byte constType)
        {
            string url = "/_kbgroup/" + name; 
            if (constType == ConstObjectType.Style)
            {
                url += ".css"; 
            }
            else if (constType == ConstObjectType.Script)
            {
                url += ".js"; 
            }
            return url;
        }  


        public static bool IsGroupUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }
            string lower = url.ToLower(); 
            if (!lower.StartsWith("/"))
            {
                lower = "/" + lower; 
            }

            if (lower.StartsWith("/_kbgroup/"))
            {
                if (lower.EndsWith(".css") || lower.EndsWith(".js"))
                {
                    return true; 
                }
            } 
            return false; 
        }
    }

    public class GroupModel
    {
        public string Name { get; set; }

        public Guid Id { get; set; }

        public byte ConstType { get; set; }
    } 
}
