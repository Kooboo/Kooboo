//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Dom
{

    /// <summary>
    /// /DocumentType nodes are simply known as doctypes.
    /// Doctypes have an associated name, public ID, and system ID.
    /// </summary>
    [Serializable]
    public class DocumentType : Node
    {

        public DocumentType()
        {
            nodeType = enumNodeType.DOCUMENT_TYPE;
        }

        public string name;
        public string publicId;
        public string systemId;

    }
}
