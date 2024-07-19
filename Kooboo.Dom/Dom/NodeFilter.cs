//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Dom
{
    [Serializable]
    public class NodeFilter
    {

        public enumWhatToShow WhatToShow;

        public enumNodeFilterAcceptNode AcceptNode(Node node)
        {
            switch (node.nodeType)
            {
                case enumNodeType.ELEMENT:
                    if (this.WhatToShow.HasFlag(enumWhatToShow.ELEMENT))
                    {
                        return enumNodeFilterAcceptNode.FILTER_ACCEPT;
                    }
                    else
                    {
                        return enumNodeFilterAcceptNode.FILTER_SKIP;
                    }

                case enumNodeType.ATTRIBUTE:
                    if (this.WhatToShow.HasFlag(enumWhatToShow.ATTRIBUTE))
                    {
                        return enumNodeFilterAcceptNode.FILTER_ACCEPT;
                    }
                    else
                    {
                        return enumNodeFilterAcceptNode.FILTER_REJECT;
                    }

                case enumNodeType.TEXT:
                    if (this.WhatToShow.HasFlag(enumWhatToShow.TEXT))
                    {
                        return enumNodeFilterAcceptNode.FILTER_ACCEPT;
                    }
                    else
                    {
                        return enumNodeFilterAcceptNode.FILTER_REJECT;
                    }
                case enumNodeType.CDATA_SECTION:
                    if (this.WhatToShow.HasFlag(enumWhatToShow.CDATA_SECTION))
                    {
                        return enumNodeFilterAcceptNode.FILTER_ACCEPT;
                    }
                    else
                    {
                        return enumNodeFilterAcceptNode.FILTER_REJECT;
                    }
                case enumNodeType.ENTITY_REFERENCE:
                    if (this.WhatToShow.HasFlag(enumWhatToShow.ENTITY_REFERENCE))
                    {
                        return enumNodeFilterAcceptNode.FILTER_ACCEPT;
                    }
                    else
                    {
                        return enumNodeFilterAcceptNode.FILTER_REJECT;
                    }
                case enumNodeType.ENTITY:
                    if (this.WhatToShow.HasFlag(enumWhatToShow.ENTITY))
                    {
                        return enumNodeFilterAcceptNode.FILTER_ACCEPT;
                    }
                    else
                    {
                        return enumNodeFilterAcceptNode.FILTER_REJECT;
                    }
                case enumNodeType.PROCESSING_INSTRUCTION:
                    if (this.WhatToShow.HasFlag(enumWhatToShow.PROCESSING_INSTRUCTION))
                    {
                        return enumNodeFilterAcceptNode.FILTER_ACCEPT;
                    }
                    else
                    {
                        return enumNodeFilterAcceptNode.FILTER_REJECT;
                    }
                case enumNodeType.COMMENT:
                    if (this.WhatToShow.HasFlag(enumWhatToShow.COMMENT))
                    {
                        return enumNodeFilterAcceptNode.FILTER_ACCEPT;
                    }
                    else
                    {
                        return enumNodeFilterAcceptNode.FILTER_REJECT;
                    }
                case enumNodeType.DOCUMENT:
                    if (this.WhatToShow.HasFlag(enumWhatToShow.DOCUMENT))
                    {
                        return enumNodeFilterAcceptNode.FILTER_ACCEPT;
                    }
                    else
                    {
                        return enumNodeFilterAcceptNode.FILTER_SKIP;
                    }
                case enumNodeType.DOCUMENT_TYPE:
                    if (this.WhatToShow.HasFlag(enumWhatToShow.DOCUMENT_TYPE))
                    {
                        return enumNodeFilterAcceptNode.FILTER_ACCEPT;
                    }
                    else
                    {
                        return enumNodeFilterAcceptNode.FILTER_REJECT;
                    }
                case enumNodeType.DOCUMENT_FRAGMENT:
                    if (this.WhatToShow.HasFlag(enumWhatToShow.DOCUMENT_FRAGMENT))
                    {
                        return enumNodeFilterAcceptNode.FILTER_ACCEPT;
                    }
                    else
                    {
                        return enumNodeFilterAcceptNode.FILTER_REJECT;
                    }
                case enumNodeType.NOTATION:
                    if (this.WhatToShow.HasFlag(enumWhatToShow.NOTATION))
                    {
                        return enumNodeFilterAcceptNode.FILTER_ACCEPT;
                    }
                    else
                    {
                        return enumNodeFilterAcceptNode.FILTER_REJECT;
                    }
                default:
                    return enumNodeFilterAcceptNode.FILTER_ACCEPT;
            }


        }

    }


    /// <summary>
    /// FILTER_ACCEPT	Value returned by the NodeFilter.acceptNode() method when a node should be accepted.
    ///FILTER_REJECT	Value to be returned by the NodeFilter.acceptNode() method when a node should be rejected. The children of rejected nodes are not visited by the NodeIterator or TreeWalker object; this value is treated as "skip this node and all its children".
    ///FILTER_SKIP	Value to be returned by NodeFilter.acceptNode() for nodes to be skipped by the NodeIterator or TreeWalker object. The children of skipped nodes are still considered. This is treated as "skip this node but not its children".
    /// </summary>
    public enum enumNodeFilterAcceptNode
    {
        FILTER_ACCEPT = 1,
        FILTER_REJECT = 2,
        FILTER_SKIP = 3
    }
}
