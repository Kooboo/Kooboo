//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Dom
{
    ///note: 
    /// 1. the user agent is to do something "using the rules for the m insertion mode", 
    /// where m is one of these modes, the user agent must use the rules described under 
    /// the m insertion mode's section, but must leave the insertion mode unchanged
    /// unless the rules in m themselves switch the insertion mode to a new value.


    public enum enumInsertionMode
    {
        Initial = 1,
        beforeHtml = 2,
        beforeHead = 3,
        inHead = 4,
        inHeadNoScript = 5,
        afterHead = 6,
        inBody = 7,
        text = 8,
        inTable = 9,
        inTableText = 10,
        inCaption = 11,
        inColumnGroup = 12,
        inTableBody = 13,
        inRow = 14,
        inCell = 15,
        inSelect = 16,
        inSelectInTable = 17,
        inTemplate = 18,
        afterBody = 19,
        inFrameset = 20,
        afterFrameset = 21,
        afterAfterBody = 22,
        afterAfterFrameset = 23,
    }

}
