//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;

namespace Kooboo.Dom
{
    /// <summary>
    /// A DocumentFragment node can have an associated element named host.
    /// An object A is a host-including inclusive ancestor of an object B, 
    /// if either A is an inclusive ancestor of B, or if B's root has an associated host
    /// and A is a host-including inclusive ancestor of B's root's host.
    /// The DocumentFragment node's host concept is useful for HTML's template element
    /// and the ShadowRoot object and impacts the pre-insert and replace algorithms.
    /// </summary>
    [Serializable]
    public class DocumentFragment : Node
    {

    }
}
