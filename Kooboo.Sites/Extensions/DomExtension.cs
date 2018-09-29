//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System.Collections.Generic;
using Kooboo.Dom;

namespace Kooboo.Sites.Extensions
{
    public static class DomExtension
    {  
        public static NodeIterator getIterator(this NodeList nodes)
        {
            NodeIterator iterator = new NodeIterator(nodes);
            return iterator;
        }
    }
    
    public class NodeIterator : IEnumerable<Node>
    {

        private NodeList rootnodelist;


        public NodeIterator(NodeList nodelist)
        {
            this.rootnodelist = nodelist;
        }

        IEnumerator<Node> GetEnumerator()
        {
            return new Enumerator(this.rootnodelist);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        IEnumerator<Node> IEnumerable<Node>.GetEnumerator()
        {
            return this.GetEnumerator();
        }


    }


    public class Enumerator : IEnumerator<Node>
    {

        private NodeList nodelist;

        private Node currentnode;
        private Node nextnode;

        private Node lastnode;

        private int rootnodecount;

        public Enumerator(NodeList nodelist)
        {
            this.nodelist = nodelist;
            this.rootnodecount = nodelist.item.Count;

            if (rootnodecount > 0)
            {
                this.lastnode = nodelist.item[rootnodecount - 1];
            }


        }

        public Node Current
        {
            get
            {
                return this.currentnode;
            }
        }

        public void Dispose()
        {
            this.nodelist = null;
        }


        public bool MoveNext()
        {

            if (this.currentnode == null)
            {
                if (this.rootnodecount == 0)
                {
                    return false;
                }

                this.currentnode = this.nodelist.item[0];

                return true;
            }

            // first go down to child. 
            nextnode = this.currentnode.firstChild();

            if (this.nextnode != null)
            {
                this.currentnode = nextnode;
                return true;
            }

            /// check the next sibling. 
            nextnode = this.currentnode.nextSibling();

            if (nextnode != null)
            {
                this.currentnode = nextnode;
                return true;
            }

            //go to parent next sibling, until reach the lastnode. 

            nextnode = getParentNextSibling(this.currentnode);

            if (nextnode != null)
            {
                this.currentnode = nextnode;
                return true;
            }

            return false;
        }

        private Node getParentNextSibling(Node checknode)
        {

            if (checknode.isEqualNode(this.lastnode))
            {
                return null;
            }

            Node parent = checknode.parentNode;

            if (parent == null || parent.isEqualNode(this.lastnode))
            {
                return null;
            }

            Node nextsibling = parent.nextSibling();

            if (nextsibling != null)
            {
                return nextsibling;
            }
            else
            {
                return getParentNextSibling(parent);
            }

        }

        /// <summary>
        /// Sorry,not reset available or needed here. 
        /// </summary>
        public void Reset()
        {
            this.currentnode = null;
            this.nextnode = null;
        }

        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

    }


    public class ElementWalker
    {

        private NodeList nodelist;

        private Node currentnode;
        private Node nextnode;

        private Node lastnode;

        private int rootnodecount;

        private bool skipsub;

        public ElementWalker(NodeList nodelist)
        {
            this.nodelist = nodelist;
            this.rootnodecount = nodelist.item.Count;

            if (rootnodecount > 0)
            {
                this.lastnode = nodelist.item[rootnodecount - 1];
            }
            this.skipsub = false;
        }

        public void SkipSub()
        {
            this.skipsub = true;
        }

        public Node ReadNext()
        {
            if (this.currentnode == null)
            {
                if (this.rootnodecount == 0)
                {
                    return null;
                }
                this.currentnode = this.nodelist.item[0];
                return this.currentnode;
            }

            // first go down to child. 

            if (!skipsub)
            {
                nextnode = this.currentnode.firstChild();
                if (this.nextnode != null)
                {
                    this.currentnode = nextnode;
                    return this.currentnode;
                }
            }
            else
            {
                skipsub = false;
            }
            /// check the next sibling. 
            nextnode = this.currentnode.nextSibling();

            if (nextnode != null)
            {
                this.currentnode = nextnode;
                return this.currentnode;
            }

            //go to parent next sibling, until reach the lastnode. 

            nextnode = getParentNextSibling(this.currentnode);

            if (nextnode != null)
            {
                this.currentnode = nextnode;
                return this.currentnode;
            }

            return null;
        }


        public void Dispose()
        {
            this.nodelist = null;
            this.currentnode = null;
            this.nextnode = null;
        }

        private Node getParentNextSibling(Node checknode)
        {

            if (checknode.isEqualNode(this.lastnode))
            {
                return null;
            }

            Node parent = checknode.parentNode;

            if (parent == null || parent.isEqualNode(this.lastnode))
            {
                return null;
            }

            Node nextsibling = parent.nextSibling();

            if (nextsibling != null)
            {
                return nextsibling;
            }
            else
            {
                return getParentNextSibling(parent);
            }

        }

        /// <summary>
        /// Sorry,not reset available or needed here. 
        /// </summary>
        public void Reset()
        {
            this.currentnode = null;
            this.nextnode = null;
        }

    }


}
