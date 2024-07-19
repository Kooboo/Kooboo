//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.Text;

namespace Kooboo.Search.Scanner
{
    public class DefaultTokenizer : Itokenizer
    {
        private bool IsHtml { get; set; }

        private string Source { get; set; }

        private int currentIndex { get; set; }

        private string currentValue { get; set; }

        private int totalLength { get; set; }

        public List<Encoding> SupportEncodings
        {
            get
            {
                return new List<Encoding>();
            }
        }

        public int Priority
        {
            get
            {
                return 10000;
            }
        }


        public Token ConsumeNext()
        {
            char lastchar = char.MinValue;
            while (currentIndex < totalLength)
            {
                var currentChar = this.Source[currentIndex];

                if ((currentChar >= 128 && currentChar < 256) || Lib.Helper.CharHelper.isAlphanumeric(currentChar))
                {
                    if (lastchar >= 256 && !string.IsNullOrEmpty(this.currentValue))
                    {
                        //unicode. 
                        lastchar = char.MinValue;
                        var token = new Token(this.currentValue);
                        this.currentValue = string.Empty;
                        if (IsStopToken(token))
                        {
                            return ConsumeNext();
                        }
                        return token;
                    }
                    this.currentValue += currentChar;
                    lastchar = currentChar;
                }
                else
                {
                    if (currentChar >= 256)
                    {
                        if (lastchar < 256 && lastchar != char.MinValue && !string.IsNullOrEmpty(this.currentValue))
                        {
                            lastchar = char.MinValue;
                            var token = new Token(this.currentValue);
                            this.currentValue = string.Empty;
                            if (IsStopToken(token))
                            {
                                return ConsumeNext();
                            }
                            return token;
                        }

                        this.currentValue += currentChar;
                        lastchar = currentChar;

                    }

                    if (!string.IsNullOrEmpty(this.currentValue))
                    {
                        var token = new Token(this.currentValue);
                        this.currentValue = string.Empty;
                        currentIndex += 1;
                        if (IsStopToken(token))
                        {
                            return ConsumeNext();
                        }
                        return token;
                    }
                }
                currentIndex += 1;
            }

            if (!string.IsNullOrEmpty(this.currentValue))
            {
                var token = new Token(this.currentValue);
                this.currentValue = string.Empty;
                if (IsStopToken(token))
                {
                    return ConsumeNext();
                }
                return token;
            }

            return null;
        }

        public void SetDoc(string document)
        {
            this.totalLength = document.Length;
            this.Source = document;
            this.currentIndex = 0;
        }

        public void SetHtml(string Html)
        {
            string CleanText = Kooboo.Search.Utility.RemoveHtml(Html);
            SetDoc(CleanText);
        }

        public bool IsStopToken(Token token)
        {
            return false;
        }

        public bool IsSeperator(char input)
        {
            if (input < 128 && !Lib.Helper.CharHelper.isAlphanumeric(input))
            {
                return true;
            }
            return false;
        }
    }
}
