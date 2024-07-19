using System.Collections.Generic;

namespace Kooboo.IndexedDB.WORM
{
    public class NodePath
    {
        public void AppendChain(Node node, int pointerIndex)
        {
            NodeChain chain = new NodeChain();
            chain.PointerIndex = pointerIndex;
            chain.Node = node;
            this.Chain.Add(chain);
        }

        //the full chain, the first one is the leaf..the last one is the highest.
        public List<NodeChain> Chain { get; set; } = new List<NodeChain>();

        public Node Leaf { get; set; }  // leaf should be the last item in chain. 

        // when it is full..
    }

    public class NodeChain
    {
        public Node Node { get; set; }

        public int PointerIndex { get; set; }
    }
}
