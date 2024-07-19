//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Dom
{
    [Serializable]
    public class Text : CharacterData
    {

        public Text()
        {
            base.nodeType = enumNodeType.TEXT;
        }

        public Text(string data)
        {
            //this.data = data;
            this.appendData(data);
            base.nodeType = enumNodeType.TEXT;
        }

        //public Text Text()
        //{
        //    Text newtext = new Text(this.data);
        //    return newtext;
        //}

        public Text splitText(int offset)
        {
            Text newtext = new Text(this.data.Substring(offset));
            return newtext;
        }

        /// <summary>
        /// The wholeText attribute must return a concatenation of the data of the contiguous Text nodes of the context object, in tree order.
        /// </summary>
        /// <returns></returns>
        public string wholeText()
        {
            throw new NotImplementedException();
        }
    }
}
