//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

namespace Kooboo.Mail.Smtp
{
    public class CommandScanner
    {
        public CommandScanner(string commandLine)
        {
            this.CommandLine = commandLine;
            this.TotalLength = commandLine?.Length ?? 0;
        }

        private string CommandLine { get; set; }

        private int CurrentIndex { get; set; }

        private string CurrentValue { get; set; }

        private int TotalLength { get; set; }

        public string ConsumeRest()
        {
            while (CurrentIndex < TotalLength)
            {
                var currentChar = this.CommandLine[CurrentIndex];
                if (!Lib.Helper.CharHelper.isSpaceCharacters(currentChar))
                {
                    return this.CommandLine.Substring(CurrentIndex).Trim();
                }
                CurrentIndex += 1;
            }
            return null;
        }

        public string ConsumeRestAfterComma()
        {
            while (CurrentIndex < TotalLength)
            {
                var currentChar = this.CommandLine[CurrentIndex];
                if (currentChar == ':')
                {
                    this.CurrentIndex += 1;
                    return ConsumeRest();
                }
                CurrentIndex += 1;
            }
            return null;
        }

        public string ConsumeNext()
        {
            while (CurrentIndex < TotalLength)
            {
                var currentChar = this.CommandLine[CurrentIndex];

                if (Lib.Helper.CharHelper.IsAscii(currentChar))
                {
                    this.CurrentValue += currentChar;
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.CurrentValue))
                    {
                        string returnvalue = this.CurrentValue;
                        this.CurrentValue = string.Empty;

                        return returnvalue;
                    }
                }

                CurrentIndex += 1;
            }

            return !string.IsNullOrEmpty(this.CurrentValue) ? this.CurrentValue : null;
        }
    }
}