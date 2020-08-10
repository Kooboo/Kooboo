//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;

namespace Kooboo.IndexedDB.Dynamic
{

    public class SyntaxScanner
    {
        private static HashSet<char> _singleoperator;
        public static HashSet<char> SingleOperator
        {
            get
            {
                if (_singleoperator == null)
                {
                    _singleoperator = new HashSet<char>();
                    _singleoperator.Add('>');
                    _singleoperator.Add('<');
                    _singleoperator.Add('=');
                    _singleoperator.Add('!');
                    _singleoperator.Add('&');
                }
                return _singleoperator;
            }
        }

        private static HashSet<string> _doubleoperator;
        public static HashSet<string> DoubleOperator
        {
            get
            {
                if (_doubleoperator == null)
                {
                    _doubleoperator = new HashSet<string>();
                    _doubleoperator.Add(">=");
                    _doubleoperator.Add("<=");
                    _doubleoperator.Add("==");
                    _doubleoperator.Add("!=");
                    _doubleoperator.Add("&&");
                }
                return _doubleoperator;
            }
        }

        private bool IsSeperator(char current)
        {
            return SingleOperator.Contains(current);
        }

        public bool IsTwoCharSperator(string input)
        {
            return DoubleOperator.Contains(input);
        }

        public SyntaxScanner(string text)
        {
            this.Source = text;
            this.totalLength = text.Length;
            this.currentIndex = 0;
        }

        private string Source { get; set; }

        private int currentIndex { get; set; }

        private string currentValue { get; set; }

        private int totalLength { get; set; }

        public string nexttoken { get; set; }

        public bool KeepStringQuote { get; set; }

        public TokenResult ConsumeNext()
        {
            while (currentIndex < totalLength)
            {
                var currentChar = this.Source[currentIndex];

                if (Helper.StringHelper.isSpaceCharacters(currentChar))
                {
                    if (!string.IsNullOrEmpty(currentValue))
                    {
                        string newvalue = currentValue;
                        currentValue = string.Empty;
                        currentIndex += 1;
                        return newvalue;
                    }
                    else
                    {
                        currentIndex += 1;
                    }
                }
                else if (IsSeperator(currentChar))
                {
                    if (!string.IsNullOrEmpty(currentValue))
                    {
                        string newvalue = currentValue;
                        currentValue = string.Empty;
                        return newvalue;
                    }
                    else
                    {
                        var next = LookAhead();

                        if (next != default(char))
                        {
                            string combine = currentChar.ToString() + next.ToString();
                            if (DoubleOperator.Contains(combine))
                            {
                                currentIndex += 2;
                                return combine;
                            }
                            else
                            {
                                currentIndex += 1;
                                return currentChar.ToString();
                            }
                        }
                        currentIndex += 1;
                        return currentChar.ToString();
                    }
                }

                else if (currentChar == '"' || currentChar == '\'')
                {
                    if (!string.IsNullOrEmpty(currentValue))
                    {
                        string newvalue = currentValue;
                        currentValue = string.Empty;
                        return newvalue;
                    }
                    else
                    {
                        string value = LookTill(currentChar);
                        return new TokenResult { Value = value, IsQuoted = true };
                    }
                }


                else
                {
                    this.currentValue += currentChar;
                    currentIndex += 1;
                }

            }

            if (!string.IsNullOrEmpty(this.currentValue))
            {
                string newvalue = this.currentValue;

                this.currentValue = string.Empty;

                return newvalue;
            }
            return null;
        }

        private char LookAhead()
        {
            int next = this.currentIndex + 1;

            if (next < this.totalLength)
            {
                return this.Source[next];
            }

            return default(char);
        }

        public string LookTill(char endchar)
        {
            string value = string.Empty;

            int next = this.currentIndex + 1;

            while (next < this.totalLength)
            {
                var current = this.Source[next];
                if (current == endchar)
                {
                    break;
                }
                else
                {
                    value += current;
                }
                next += 1;
            }

            this.currentIndex = next + 1;
            return value;
        }


    }

    public class TokenResult
    {
        public string Value { get; set; }

        public bool IsQuoted { get; set; }

        public static implicit operator TokenResult(string str)
        {
            return new TokenResult { Value = str, IsQuoted = false };
        }
    }
}
