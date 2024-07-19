//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Dom
{

    [Flags]
    public enum enumWhatToShow
    {
        ELEMENT = 1 << 1,
        ATTRIBUTE = 1 << 2,
        TEXT = 1 << 3,
        CDATA_SECTION = 1 << 4,
        ENTITY_REFERENCE = 1 << 5,
        ENTITY = 1 << 6,
        PROCESSING_INSTRUCTION = 1 << 7,
        COMMENT = 1 << 8,
        DOCUMENT = 1 << 9,
        DOCUMENT_TYPE = 1 << 10,
        DOCUMENT_FRAGMENT = 1 << 11,
        NOTATION = 1 << 12,
        All = ELEMENT | ATTRIBUTE | TEXT | CDATA_SECTION | ENTITY_REFERENCE | ENTITY | PROCESSING_INSTRUCTION | COMMENT | DOCUMENT | DOCUMENT_TYPE | DOCUMENT_FRAGMENT | NOTATION


    }
}
