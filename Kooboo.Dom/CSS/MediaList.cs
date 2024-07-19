//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
using System;
using System.Collections.Generic;

namespace Kooboo.Dom.CSS
{
    /*
     http://dev.w3.org/csswg/cssom
     * 
  An object that implements the MediaList interface has an associated collection of media queries.
[ArrayClass]
interface MediaList {
  [TreatNullAs=EmptyString] stringifier attribute DOMString mediaText;
  readonly attribute unsigned long length;
  getter DOMString? item(unsigned long index);
  void appendMedium(DOMString medium);
  void deleteMedium(DOMString medium);
};
*/

    /// <summary>
    /// Firefox does not have MediaList, it has MediaQueryList instead, 
    /// this Medialist it is not enough.
    /// Right now, the string might contains both media and conditiontext. 
    /// </summary>
    [Serializable]
    public class MediaList
    {

        public List<string> item = new List<string>();

        public void appendMedium(string medium)
        {
            if (string.IsNullOrEmpty(medium))
            {
                return;
            }

            if (!item.Contains(medium))
            {
                item.Add(medium);
            }

        }

        public void deleteMedium(string medium)
        {
            if (string.IsNullOrEmpty(medium))
            {
                return;
            }

            if (item.Contains(medium))
            {
                item.Remove(medium);
            }
        }

    }
}
