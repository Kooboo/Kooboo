//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Dom.CSS
{
    public static class MediaQuery
    {

        /// <summary>
        /// check whether th user-agent match the media rule condition or not. 
        /// TODO: to be implemented.
        /// </summary>
        /// <param name="medialist"></param>
        /// <param name="mediadevice"></param>
        /// <returns></returns>
        public static bool isMatch(MediaList medialist, string mediadevice)
        {
            //if a media or import rule does not specify any device info, it applys to all. 
            if (medialist == null || medialist.item.Count == 0)
            {
                return true;
            }

            foreach (var subitem in medialist.item)
            {
                if (subitem.ToLower().Contains("all"))
                {
                    return true;
                }
                else if (subitem.ToLower().Contains(mediadevice))
                {
                    return true;
                }
            }

            return false;
        }


    }
}
