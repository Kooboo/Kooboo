//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Dom
{

    //The output of the tokenization step is a series of zero or more of the following tokens:
    //DOCTYPE, start tag, end tag, comment, character, end-of-file.
    // DOCTYPE tokens have a name, a public identifier, a system identifier, and a force-quirks flag. 
    //When a DOCTYPE token is created, its name, public identifier, and system identifier must be marked as missing 
    //(which is a distinct state from the empty string), and the force-quirks flag must be set to off 
    //(its other state is on). Start and end tag tokens have a tag name, a self-closing flag, and a list of attributes, 
    // each of which has a name and a value. When a start or end tag token is created, 
    //its self-closing flag must be unset (its other state is that it be set), 
    // and its attributes list must be empty. Comment and character tokens have data.

    public class HtmlToken
    {
        //public HtmlToken()
        //{
        //    type = enumHtmlTokenType.Tag; // default is the tag. 
        //}

        public HtmlToken(enumHtmlTokenType tokenType)
        {
            type = tokenType;
        }

        /// <summary>
        /// For comment and character tokens.
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="character"></param>
        public HtmlToken(enumHtmlTokenType tokenType, char character)
        {
            type = tokenType;
            data = character.ToString();
        }

        public enumHtmlTokenType type;

        // for character and comment.
        public string data;

        // for doctype
        public string name;

        /// <summary>
        /// public identifier
        /// </summary>
        public string publicId;

        /// <summary>
        /// system identifier.
        /// </summary>
        public string systemId;
        public bool forceQuirks;

        // for start, end tag.
        public string tagName;
        public Dictionary<string, string> attributes = new Dictionary<string, string>();

        public string currentAttributeName;

        public string currentAttributeValue
        {
            get
            {
                if (sb != null)
                {
                    return sb.ToString();
                }
                else
                {
                    return null;
                }
            }
            set
            {
                appendAttributeString(value);
            }
        }



        public void startNewAttribute()
        {
            if (!string.IsNullOrEmpty(currentAttributeName))
            {

                //When the user agent leaves the attribute name state (and before emitting the tag token, if appropriate), the complete attribute's name must be compared to the other attributes on the same token; if there is already an attribute on the token with the exact same name, then this is a parse error and the new attribute must be removed from the token.

                if (attributes.ContainsKey(currentAttributeName))
                {
                    ///attributes[currentAttributeName] = currentAttributeValue;
                    ///TODO: this is a prase error. 
                }
                else
                {
                    attributes.Add(currentAttributeName, currentAttributeValue);
                    CleanAttributeValue();
                }
            }

            // currentAttributeValue = string.Empty;
            CleanAttributeValue();
            currentAttributeName = string.Empty;

        }

        public bool isSelfClosing;

        //the location of this token within the html string. 
        public int startIndex;   // the indexof position of start tag in the string. 
        public int endIndex;   // the last position of end tag in the string. 

        private StringBuilder sb;

        internal void appendAttributeChar(char chr)
        {
            if (sb == null)
            {
                sb = new StringBuilder();
            }

            sb.Append(chr);
        }

        internal void appendAttributeString(string input)
        {
            if (sb == null)
            {
                sb = new StringBuilder();
            }

            sb.Append(input);

        }

        /// <summary>
        /// clean and make the attribute value to be empty string. 
        /// </summary>
        internal void CleanAttributeValue()
        {
            if (sb != null)
            {
                sb.Clear();
            }
        }

    }
}
