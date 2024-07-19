//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Kooboo.Dom.CSS
{

    // http://dev.w3.org/csswg/cssom/#cssstyledeclaration

    /// <summary>
    /// One block contains one definiation of multiple property + values. 
    /// example.
    /// .class
    /// {
    /// font-size: 10px;    // = one CSS Declaration. 
    /// font-color:red; 
    /// }
    /// </summary>
    [Serializable]
    public class CSSDeclarationBlock
    {

        public CSSDeclarationBlock()
        {
            this.item = new List<CSSDeclaration>();
        }

        private string _cssText;

        /// <summary>
        /// Textual representation of the declaration block. Setting this attribute changes the style.
        /// this text should not contains the {}. 
        /// </summary>
        public string cssText
        {
            set
            {
                _cssText = value;
            }
            get
            {
                initializeCssText();
                return _cssText;
            }
        }

        private void initializeCssText()
        {
            if (string.IsNullOrEmpty(_cssText))
            {
                if (item.Count() > 0)
                {
                    foreach (var onedeclaration in item)
                    {
                        string declarationText = onedeclaration.propertyname + ": " + onedeclaration.value;
                        if (onedeclaration.important)
                        {
                            declarationText = declarationText + " !important";
                        }
                        declarationText = declarationText + ";";

                        _cssText += declarationText;
                    }
                }
                else
                {
                    _cssText = string.Empty;
                }
            }
        }

        public string GenerateCssText()
        {
            string result = string.Empty;

            if (item.Count() > 0)
            {
                foreach (var onedeclaration in item)
                {
                    if (onedeclaration == null) continue;
                    string declarationText = onedeclaration.propertyname + ": " + onedeclaration.value;
                    if (onedeclaration.important)
                    {
                        declarationText = declarationText + " !important";
                    }
                    declarationText = declarationText + ";";

                    result += declarationText;
                }
            }
            return result;

        }

        public List<CSSDeclaration> item;

        public string getPropertyValue(string propertyname)
        {
            propertyname = propertyname.ToLower();
            foreach (var onedeclaration in item)
            {
                if (onedeclaration.propertyname.ToLower() == propertyname)
                {
                    return onedeclaration.value;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Retrun true = !important.
        /// </summary>
        /// <param name="propertyname"></param>
        /// <returns></returns>
        public bool getPropertyPriority(string propertyname)
        {
            propertyname = propertyname.ToLower();

            foreach (var onedeclaration in item)
            {
                if (onedeclaration.propertyname.ToLower() == propertyname)
                {
                    return onedeclaration.important;
                }
            }

            return false;
        }

        /// <summary>
        /// Set/insert or update the property + value. 
        /// </summary>
        /// <param name="propertyname"></param>
        /// <param name="value"></param>
        /// <param name="important"></param>
        public void setProperty(string propertyname, string value, bool important)
        {
            propertyname = propertyname.ToLower();

            bool found = false;

            foreach (var onedeclaration in item)
            {
                if (onedeclaration != null && onedeclaration.propertyname.ToLower() == propertyname)
                {
                    if (onedeclaration.important)
                    {
                        if (important)
                        {
                            onedeclaration.value = value;
                            onedeclaration.important = important;
                        }
                    }
                    else
                    {
                        onedeclaration.value = value;
                        onedeclaration.important = important;
                    }

                    found = true;
                }
            }

            // not found
            if (!found)
            {
                item.Add(new CSSDeclaration() { propertyname = propertyname, value = value, important = important });
            }
        }

        public void removeProperty(string propertyname)
        {
            propertyname = propertyname.ToLower();

            List<CSSDeclaration> itemstoremove = new List<CSSDeclaration>();
            foreach (var onedeclaration in item)
            {
                if (onedeclaration.propertyname.ToLower() == propertyname)
                {
                    itemstoremove.Add(onedeclaration);
                }
            }

            foreach (var one in itemstoremove)
            {
                item.Remove(one);
            }
        }

        public void setPropertyValue(string propertyname, string value)
        {
            propertyname = propertyname.ToLower();
            bool found = false;

            foreach (var onedeclaration in item)
            {
                if (onedeclaration.propertyname.ToLower() == propertyname)
                {
                    onedeclaration.value = value;
                    found = true;
                }
            }
            // not found
            if (!found)
            {
                item.Add(new CSSDeclaration() { propertyname = propertyname, value = value });
            }
        }

        public void setPropertyPriority(string propertyname, bool important)
        {
            propertyname = propertyname.ToLower();

            foreach (var onedeclaration in item)
            {
                if (onedeclaration.propertyname.ToLower() == propertyname)
                {
                    onedeclaration.important = important;
                }
            }
            // not found
        }

        #region "non w3c"
        //below methods are created to update _cssText when a property is set or changed.

        /// <summary>
        /// Insert or update declaration, will also update the cssText. 
        /// </summary>
        /// <param name="declaration"></param>
        public void updateDeclaration(CSSDeclaration declaration)
        {
            if (declaration == null)
            {
                return;
            }
            bool exists = isPropertyExists(declaration);

            setProperty(declaration.propertyname, declaration.value, declaration.important);

            if (exists)
            {
                updateCssText(declaration);
            }
            else
            {
                appendCssText(declaration);
            }
        }

        /// <summary>
        /// CSSStyleDeclaration == CSSDeclarationBlock
        /// Merge two declaration blocks into one. 
        /// </summary>
        /// <param name="styleDeclaration"></param>
        public void merge(CSSStyleDeclaration styleDeclaration)
        {
            for (int i = 0; i < styleDeclaration.item.Count(); i++)
            {
                updateDeclaration(styleDeclaration.item[i]);
            }
        }

        private bool isPropertyExists(CSSDeclaration declaration)
        {
            string propertyname = declaration.propertyname.ToLower();

            foreach (var onedeclaration in item)
            {
                if (onedeclaration.propertyname.ToLower() == propertyname)
                {
                    return true;
                }
            }

            return false;
        }

        private void updateCssText(CSSDeclaration declaration)
        {
            string declarationText = declaration.propertyname + ": " + declaration.value;
            if (declaration.important)
            {
                declarationText = declarationText + " !important";
            }
            declarationText = declarationText + ";";

            // use regex is OK in this case, because all property name does not contains escape chars. 
            string pattern = declaration.propertyname + @"\s*\:\s*.*?(;|$)";

            _cssText = Regex.Replace(_cssText, pattern, declarationText);

        }
        /// <summary>
        /// add the new declaration to csstext.
        /// </summary>
        /// <param name="declaration"></param>
        private void appendCssText(CSSDeclaration declaration)
        {
            /// this append actually insert at the beginning to prevent, 
            ////// To serialize a CSS declaration with property name property, value and optionally an important flag set, follow these steps:
            //Let s be the empty string.
            //Append property to s.
            //Append ": " (U+003A U+0020) to s.
            //Append value to s.
            //If the important flag is set, append " !important" (U+0020 U+0021 U+0069 U+006D U+0070 U+006F U+0072 U+0074 U+0061 U+006E U+0074) to s.
            //Append ";" (U+003B) to s.
            //Return s.
            string declarationText = declaration.propertyname + ": " + declaration.value;
            if (declaration.important)
            {
                declarationText = declarationText + " !important";
            }
            declarationText = "\r\n" + declarationText + ";" + "\r\n";

            if (string.IsNullOrEmpty(_cssText))
            {
                initializeCssText();
            }

            if (string.IsNullOrEmpty(_cssText))
            {
                _cssText = declarationText;
                return;
            }

            int bracketIndex = _cssText.IndexOf("{");

            if (bracketIndex > 0)
            {
                string firststring = _cssText.Substring(0, bracketIndex + 1);

                string secondstring = _cssText.Substring(bracketIndex + 1);

                _cssText = firststring + declarationText + secondstring;
            }
            else
            {
                _cssText = declarationText + _cssText;
            }

        }


        public bool hasPartialProperty(string partialPropertyName)
        {
            partialPropertyName = partialPropertyName.ToLower();

            foreach (var onedeclaration in item)
            {
                if (onedeclaration.propertyname.ToLower().Contains(partialPropertyName))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

    }
}
