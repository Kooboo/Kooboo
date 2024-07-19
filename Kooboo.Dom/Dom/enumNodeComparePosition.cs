//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Dom
{
    public enum enumNodeComparePosition
    {
        DOCUMENT_POSITION_DISCONNECTED = 1,
        DOCUMENT_POSITION_PRECEDING = 2,    // other node before current. 
        DOCUMENT_POSITION_FOLLOWING = 3,     // current node before other node. 
        DOCUMENT_POSITION_CONTAINS = 4,
        DOCUMENT_POSITION_CONTAINED_BY = 5,
        DOCUMENT_POSITION_IMPLEMENTATION_SPECIFIC = 6,

        DOCUMENT_POSITION_SAME_NODE = 7

    }
}
