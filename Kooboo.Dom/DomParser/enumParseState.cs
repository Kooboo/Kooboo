//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Dom
{
    public enum enumParseState
    {
        /// <summary>
        /// Initial state: Parsed Character Data (characters will be parsed).
        /// </summary>
        DATA,
        /// <summary>
        /// Optional state: Raw character data (characters will be parsed from a special table).
        /// </summary>
        RCDATA,
        /// <summary>
        /// Optional state: Just plain text data (chracters will be parsed matching the given ones).
        /// </summary>
        Plaintext,
        /// <summary>
        /// Optional state: Rawtext data (characters will not be parsed).
        /// </summary>
        RAWTEXT,
        /// <summary>
        /// Optional state: Script data.
        /// </summary>
        Script
    }
}
