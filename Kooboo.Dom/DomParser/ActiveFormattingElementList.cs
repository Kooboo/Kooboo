//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kooboo.Dom
{

    /// <summary>
    /// Formatting The following HTML elements are those that end up in the list of active formatting elements: a, b, big, code, em, font, i, nobr, s, small, strike, strong, tt, and u.
    /// The formating elements can be element or a marker,  a marker has the tagName == "marker";
    /// </summary>
    public class ActiveFormattingElementList
    {

        private TreeConstruction _treeConstruction;

        public ActiveFormattingElementList(TreeConstruction tree)
        {
            _treeConstruction = tree;

        }

        public List<FormattingElement> item = new List<FormattingElement>();

        public int length
        {
            get
            {
                return item.Count();
            }
        }

        private void append(FormattingElement element)
        {
            item.Add(element);
        }

        private FormattingElement LastItem()
        {
            if (length == 0)
            {
                return null;
            }
            else
            {
                return item[length - 1];
            }

        }

        public void insertMarker()
        {
            FormattingElement element = new FormattingElement();
            element.isMarker = true;
            append(element);
        }

        public void clearUpToLastMarker()
        {
            //  When the steps below require the UA to clear the list of active formatting elements up to the last marker, the UA must perform the following steps:
            //Let entry be the last (most recently added) entry in the list of active formatting elements.
            //Remove entry from the list of active formatting elements.
            //If entry was a marker, then stop the algorithm at this point. The list has been cleared up to the last marker.
            //Go to step 1.
            int index = length - 1;

            List<int> IdToRemoved = new List<int>();

            for (int i = index; i >= 0; i--)
            {
                if (item[i].isMarker)
                {
                    IdToRemoved.Add(i);
                    break;
                }

                IdToRemoved.Add(i);
            }

            foreach (var removeid in IdToRemoved.OrderByDescending(o => o))
            {
                item.RemoveAt(removeid);
            }

        }

        public void Remove(string tagName, bool includeAllMatchedItems)
        {
            int index = length - 1;

            List<int> IdToRemoved = new List<int>();

            for (int i = index; i >= 0; i--)
            {
                Element element = item[i].element;

                if (element != null && element.tagName == tagName)
                {
                    if (includeAllMatchedItems)
                    {
                        IdToRemoved.Add(index);
                    }
                    else
                    {
                        item.RemoveAt(i);
                        return;

                    }
                }
            }

            foreach (var removeid in IdToRemoved.OrderByDescending(o => o))
            {
                item.RemoveAt(removeid);
            }
        }

        public void Reconstruct()
        {
            // When the steps below require the UA to reconstruct the active formatting elements, the UA must perform the following steps:

            //If there are no entries in the list of active formatting elements, then there is nothing to reconstruct; stop this algorithm.
            if (_treeConstruction.activeFormatingElements.length == 0)
            {
                return;
            }

            //If the last (most recently added) entry in the list of active formatting elements is a marker, or if it is an element that is in the stack of open elements, then there is nothing to reconstruct; stop this algorithm.

            FormattingElement lastitem = LastItem();

            if (lastitem.isMarker || (lastitem.element != null && _treeConstruction.openElements.hasElement(lastitem.element.tagName)))
            {

                return;
            }

            int index = length - 1;

            //Let entry be the last (most recently added) element in the list of active formatting elements.
            FormattingElement entry = item[index];

        //Rewind: If there are no entries before entry in the list of active formatting elements, then jump to the step labeled create.
        Rewind:

            if (index == 0)
            {
                goto Create;
            }

            //Let entry be the entry one earlier than entry in the list of active formatting elements.
            index = index - 1;
            entry = item[index];

            //If entry is neither a marker nor an element that is also in the stack of open elements, go to the step labeled rewind.
            if (!entry.isMarker && (entry.element != null && !_treeConstruction.openElements.hasTag(entry.element.tagName)))
            {
                goto Rewind;
            }

        //Advance: Let entry be the element one later than entry in the list of active formatting elements.

        Advance:

            index = index + 1;
            entry = item[index];


        Create:

            //Create: Insert an HTML element for the token for which the element entry was created, to obtain new element.
            Element newelement = _treeConstruction.createElement(entry.token);
            FormattingElement formatelement = new FormattingElement();
            formatelement.element = newelement;
            formatelement.token = entry.token;

            //Replace the entry for entry in the list with an entry for new element.
            item[index] = formatelement;

            //If the entry for new element in the list of active formatting elements is not the last entry in the list, return to the step labeled advance.
            if (index < length - 1)
            {
                goto Advance;
            }

            //This has the effect of reopening all the formatting elements that were opened in the current body, cell, or caption (whichever is youngest) that haven't been explicitly closed.

        }

        public void Push(FormattingElement element)
        {
            //When the steps below require the UA to push onto the list of active formatting elements an element element, the UA must perform the following steps:

            //If there are already three elements in the list of active formatting elements after the last list marker, if any, or anywhere in the list if there are no list markers, that have the same tag name, namespace, and attributes as element, then remove the earliest such element from the list of active formatting elements. 
            if (CounterAfterLastMarker() >= 3 || !HasMark())
            {

                for (int i = 0; i < length; i++)
                {

                    if (!item[i].isMarker)
                    {
                        if (isSameElementAndAttributes(item[i], element))
                        {
                            item.RemoveAt(i);
                            break;
                        }
                    }

                }

            }

            //For these purposes, the attributes must be compared as they were when the elements were created by the parser; two elements have the same attributes if all their parsed attributes can be paired such that the two attributes in each pair have identical names, namespaces, and values (the order of the attributes does not matter).

            //This is the Noah's Ark clause. But with three per family instead of two.

            //Add element to the list of active formatting elements.
            append(element);
        }

        public void Push(Element element, HtmlToken token)
        {
            FormattingElement formatelement = new FormattingElement();

            formatelement.element = element;
            formatelement.token = token;
            formatelement.isMarker = false;

            Push(formatelement);

        }

        /// <summary>
        /// Compare two formatting elements. 
        /// </summary>
        /// <param name="elementone"></param>
        /// <param name="elementtwo"></param>
        /// <returns></returns>
        private bool isSameElementAndAttributes(FormattingElement elementone, FormattingElement elementtwo)
        {
            //that have the same tag name, namespace, and attributes as element

            if (elementone.element == null || elementtwo.element == null)
            {
                return false;
            }

            Element one = elementone.element;
            Element two = elementtwo.element;

            return IsSameDomElement(one, two);

        }

        public static bool IsSameDomElement(Element one, Element two)
        {
            if (one.tagName != two.tagName)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(one.namespaceURI) || !string.IsNullOrEmpty(two.namespaceURI))
            {
                if (one.namespaceURI != two.namespaceURI)
                {
                    return false;
                }
            }

            //For these purposes, the attributes must be compared as they were when the elements were created by the parser; two elements have the same attributes if all their parsed attributes can be paired such that the two attributes in each pair have identical names, namespaces, and values (the order of the attributes does not matter).

            if (one.attributes.Count() != two.attributes.Count())
            {
                return false;
            }

            foreach (var item in one.attributes)
            {
                string valueInTwo = two.getAttribute(item.name);

                if (valueInTwo != item.value)
                {

                    return false;
                }

            }

            return true;
        }

        /// <summary>
        /// The number of elments in the list of active formating elements after last marker. 
        /// </summary>
        /// <returns></returns>
        private int CounterAfterLastMarker()
        {
            int index = length - 1;
            int count = 0;

            for (int i = index; i >= 0; i--)
            {
                if (item[i].isMarker)
                {
                    return count;
                }
                else
                {
                    count += 1;
                }
            }

            return count;
        }

        private bool HasMark()
        {
            foreach (var one in item)
            {
                if (one.isMarker)
                { return true; }
            }
            return false;
        }

        public bool hasElement(Element element)
        {
            foreach (var elementitem in item)
            {
                if (!elementitem.isMarker && IsSameDomElement(elementitem.element, element))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
