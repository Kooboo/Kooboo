//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Mail.Smtp
{

    public class CommandScanner
    {
        public CommandScanner(string commandLine)
        {
            this.CommmandLine = commandLine;
            this.totalLength = commandLine == null ? 0 : commandLine.Length;
        }

        private string CommmandLine { get; set; }

        private int currentIndex { get; set; }

        private string currentValue { get; set; }

        private int totalLength { get; set; }

        public string ConsumeRest()
        {
            while (currentIndex < totalLength)
            {
                var currentChar = this.CommmandLine[currentIndex];
                if (!Lib.Helper.CharHelper.isSpaceCharacters(currentChar))
                {
                    return this.CommmandLine.Substring(currentIndex).Trim();
                }
                currentIndex += 1;
            }
            return null;
        }

        public string ConsumeRestAfterComma()
        {
            while (currentIndex < totalLength)
            {
                var currentChar = this.CommmandLine[currentIndex];
                if (currentChar == ':')
                {
                    this.currentIndex += 1;
                    return ConsumeRest();
                }
                currentIndex += 1;
            }
            return null;
        }

        public string ConsumeNext()
        {
            while (currentIndex < totalLength)
            {
                var currentChar = this.CommmandLine[currentIndex];

                if (Lib.Helper.CharHelper.IsAscii(currentChar))
                {
                    this.currentValue += currentChar;
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.currentValue))
                    {
                        string returnvalue = this.currentValue;
                        this.currentValue = string.Empty;

                        return returnvalue;
                    }
                }

                currentIndex += 1;
            }

            if (!string.IsNullOrEmpty(this.currentValue))
            {
                return this.currentValue;
            }
            return null;
        }
    }
}
