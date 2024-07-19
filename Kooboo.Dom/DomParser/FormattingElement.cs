//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Dom
{
    public class FormattingElement
    {

        public Element element;

        public bool isMarker;

        public HtmlToken token;

        public bool isEqualTo(FormattingElement element)
        {
            if (this.isMarker != element.isMarker)
            {
                return false;
            }

            if (this.isMarker)
            {
                // compare token. 
                if (this.token.type != element.token.type)
                {
                    return false;
                }

                else if (this.token.tagName != element.token.tagName)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                // compare element. 
                return ActiveFormattingElementList.IsSameDomElement(this.element, element.element);
            }

        }

    }
}
