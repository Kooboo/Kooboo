//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace SassAndCoffee.JavaScript.ActiveScript {

    public enum ScriptStatistic : uint {
        /// <summary>
        /// Return the number of statements executed since the script started or the statistics were reset.
        /// </summary>
        StatementCount = 1,
        InstructionCount = 2,
        InstructionTime = 3,
        TotalTime = 4,
    }
}
