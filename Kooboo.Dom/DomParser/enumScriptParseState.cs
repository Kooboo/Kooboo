//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Dom
{
    public enum enumScriptParseState
    {
        initial = 0,
        dataEscapeStart = 1,
        dataEscapeStartDash = 2,
        dataEscapedDashDash = 3,
        dataEscaped = 4,
        dataDoubleEscapeStart = 5,
        dataEscapedDash = 6,
        dataDoubleEscapedDashDash = 7,
        dataDoubleEscapedLessThanSign = 8,
        dataDoubleEscaped = 9,
        dataDoubleEscapedDash = 10,
        dataDoubleEscapeEnd = 11
    }
}
