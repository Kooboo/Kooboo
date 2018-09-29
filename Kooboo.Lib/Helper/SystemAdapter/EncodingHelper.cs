using System;
using System.Collections.Generic;
using System.Text;

namespace Kooboo
{
    public class TextEncodingHelper
    {
        public static void RegisterEncoding()
        {
            //register encoding like gb18030,because Encoding don't contain these encoding in dotnet standard by default
#if NETSTANDARD2_0
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
        }
    }
}
