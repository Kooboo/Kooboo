//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Dom
{
    [Serializable]
    public class HTMLCollection
    {
        private List<Element> _item;

        public List<Element> item
        {
            get
            {
                if (_item == null)
                {
                    _item = new List<Element>();
                }
                return _item;
            }
            set
            {
                _item = value;
            }
        }

        public int length
        {
            get
            {
                return item.Count();
            }
        }

        public void Add(Element element)
        {
            item.Add(element);
        }

        public void Merge(HTMLCollection collection)
        {
            foreach (var anotheritem in collection.item)
            {
                item.Add(anotheritem);
            }
        }
    }
}
