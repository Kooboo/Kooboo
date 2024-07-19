using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.IndexedDB.WORM
{
    public class Debugger
    {

        public static NodeMap LoadMap<TValue>(WormDb<TValue> worm)
        {
            return ConvertToMap<TValue>(worm, worm.RootNode);
        }

        private static NodeMap ConvertToMap<TValue>(WormDb<TValue> worm, Node node)
        {
            if (node == null)
            {
                return null;
            }

            NodeMap map = new NodeMap();
            map.node = node; ;
            var pointer = node.Pointer[0];
            if (pointer != null)
            {
                map.Id = pointer.Id;
            }
            map.DiskPosition = node.DiskPosition;
            map.Pointer = node.Pointer;
            map.SubCount = node.Pointer.Where(o => o != null).Count();
            map.prev = node.PreviousNode;
            map.next = node.NextNode;

            if (!node.Isleaf)
            {
                map.Children = new List<NodeMap>();

                foreach (var item in node.Pointer)
                {
                    if (item != null && !item.IsDeleted)
                    {
                        var subNode = worm.LoadNode(item.Position);
                        var subMap = ConvertToMap<TValue>(worm, subNode);
                        map.Children.Add(subMap);
                    }
                }
            }

            return map;

        }


        public static List<Node> LeafChain<TValue>(WormDb<TValue> worm)
        {
            var leaf = worm.RootNode;

            while (!leaf.Isleaf)
            {
                var pointer = leaf.Pointer[0];
                if (pointer != null)
                {
                    leaf = worm.LoadNode(pointer.Position);
                }
            }

            List<Node> result = new();

            result.Add(leaf);

            while (leaf != null && leaf.NextNode > 0)
            {
                leaf = worm.LoadNode(leaf.NextNode);
                result.Add(leaf);
            }

            return result;
        }

    }

    public class NodeMap
    {
        public Node node { get; set; }

        public long Id { get; set; }

        public long DiskPosition { get; set; }

        public long prev { get; set; }
        public long next { get; set; }

        public int SubCount { get; set; }

        public PositionPointer[] Pointer { get; set; }

        public List<NodeMap> Children { get; set; }

    }
}
