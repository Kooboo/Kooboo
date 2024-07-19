//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Dom
{
    public class StackTemplateMode
    {
        public List<enumInsertionMode> item = new List<enumInsertionMode>();

        public int length
        {
            get
            {
                return item.Count();
            }
        }

        /// <summary>
        /// The current is the bottommost node in this stack of open elements.
        /// </summary>
        /// <returns></returns>
        public enumInsertionMode currentMode()
        {
            int index = length - 1;
            if (index >= 0)
            {
                return item[index];
            }
            else
            {
                return enumInsertionMode.Initial;
            }
        }

        /// <summary>
        /// add/append a new open elements. 
        /// </summary>
        /// <param name="node"></param>
        public void push(enumInsertionMode mode)
        {
            item.Add(mode);
        }

        /// <summary>
        /// get one off the last append node.
        /// </summary>
        /// <param name="mode"></param>
        public void popOff(enumInsertionMode mode)
        {
            int index = length - 1;

            for (int i = index; i >= 0; i--)
            {
                if (item[i] == mode)
                {
                    item.RemoveAt(i);
                    return;
                }
            }


        }


        public void popOffCurrent()
        {
            int index = length - 1;
            if (index > -1)
            {
                item.RemoveAt(index);
            }
        }

    }
}
