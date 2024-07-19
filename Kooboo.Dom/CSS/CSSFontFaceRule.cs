//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Dom.CSS
{
    public class CSSFontFaceRule : CSSRule
    {
        public CSSFontFaceRule()
        {
            this.type = enumCSSRuleType.FONT_FACE_RULE;
            this.style = new CSSStyleDeclaration();
        }

        //family-name
        //Specifies a name that will be used as the font face value for font properties. ( i.e. font-family: <family-name>; )
        //src
        //URL for the remote font file location, or the name of a font on the user's computer in the form local("Font Name"). You can specify a font on the user's local computer by name using the local() syntax. If that font isn't found, other sources will be tried until one is found.
        //font-variant
        //A font-variant value.
        //font-stretch
        //A font-stretch value.
        //font-weight
        //A font-weight value.
        //font-style
        //A font-style value.
        //unicode-range
        //The range of unicode code points defined in the font-face rule.

        public CSSStyleDeclaration style;


    }
}
