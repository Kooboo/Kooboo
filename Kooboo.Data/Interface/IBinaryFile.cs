//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com
//All rights reserved.

namespace Kooboo.Data.Interface
{
    public interface IBinaryFile : IExtensionable
    {
        byte[] ContentBytes { get; set; }

        int Size { get; set; }
    }
}