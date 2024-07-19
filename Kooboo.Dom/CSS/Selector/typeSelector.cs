//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Dom.CSS
{
    public class typeSelector : simpleSelector
    {

        public typeSelector()
        {
            base.Type = enumSimpleSelectorType.type;
        }

        /// <summary>
        /// the qualified html tag name. 
        /// </summary>
        public string elementE;

    }
}
