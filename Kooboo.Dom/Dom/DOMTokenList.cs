//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Dom
{
    [Serializable]
    public class DOMTokenList
    {

        public List<string> item = new List<string>();

        public int length
        {
            get
            {
                return item.Count();
            }
        }

        public bool contains(string token)
        {
            return item.Contains(token);
        }

        public void add(params string[] tokens)
        {
            foreach (var tokenitem in tokens)
            {
                if (!item.Contains(tokenitem))
                {
                    item.Add(tokenitem);
                }
            }
        }

        public void remove(params string[] tokens)
        {
            foreach (var tokenitem in tokens)
            {
                item.Remove(tokenitem);
            }
        }


        /// <summary>
        ///  "toggles" token, removing it if it's present and adding it if it's not.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Returns true if token is now present, and false otherwise.</returns>
        public bool toggle(string token)
        {
            //Throws a "SyntaxError" exception if token is empty.
            //Throws an "InvalidCharacterError" exception if token contains any spaces.

            if (string.IsNullOrEmpty(token))
            {
                throw new Exception("SyntaxError");
            }

            if (token.IndexOf(" ") > 0)
            {
                throw new Exception("InvalidCharacterError: " + token);
            }

            if (item.Contains(token))
            {
                item.Remove(token);
                return false;
            }
            else
            {
                item.Add(token);
                return true;
            }
        }

        /// <summary>
        /// If force is true, adds token (same as add()). If force is false, removes token (same as remove()).
        /// </summary>
        /// <param name="token"></param>
        /// <param name="force"></param>
        /// <returns>Returns true if token is now present, and false otherwise.</returns>
        public bool toggle(string token, bool force)
        {
            if (force)
            {
                if (!item.Contains(token))
                {
                    item.Add(token);
                }

                return true;
            }
            else
            {
                if (item.Contains(token))
                {
                    item.Remove(token);
                }

                return false;
            }


        }


    }
}
