//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using System.Text;


namespace Kooboo.Search.Scanner
{
    public interface Itokenizer
    {
        List<Encoding> SupportEncodings { get; }
        Token ConsumeNext();
        void SetDoc(string document);
        void SetHtml(string Html);
        int Priority { get; }
        bool IsStopToken(Token token);

        bool IsSeperator(char input);
    }
}
