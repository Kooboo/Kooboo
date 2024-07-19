//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using Kooboo.Dom.CSS.Tokens;

namespace Kooboo.Dom.CSS.rawmodel
{
    /// <summary>
    /// Any token produced by the tokenizer except for function-tokens, {-tokens, (-tokens, and [-tokens.
    /// </summary>
    public class PreservedToken : ComponentValue
    {

        //Note: The non-preserved tokens listed above are always consumed into higher-level objects, either functions or simple blocks, and so never appear in any parser output themselves.

        //Note: The tokens <}-token>s, <)-token>s, <]-token>, <bad-string-token>, and <bad-url-token> are always parse errors, but they are preserved in the token stream by this specification to allow other specs, such as Media Queries, to define more fine-grainted error-handling than just dropping an entire declaration or block.

        public PreservedToken()
        {
            this.Type = CompoenentValueType.preservedToken;
            this.startindex = -1;
            this.endindex = -1;
        }

        public cssToken token;
    }
}
