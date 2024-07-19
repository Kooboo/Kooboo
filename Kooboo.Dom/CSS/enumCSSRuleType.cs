//Copyright (c) 2018 Yardi Technology Limited. Http://www.kooboo.com 
//All rights reserved.
namespace Kooboo.Dom.CSS
{

    ///http://dev.w3.org/csswg/cssom/#cssrule
    ///http://wiki.csswg.org/spec/cssom-constants
    // 0	-	reserved; formerly UNKNOWN_RULE in DOM-Level-2-Style
    //1	STYLE_RULE	CSSOM
    //2	CHARSET_RULE	CSSOM
    //3	IMPORT_RULE	CSSOM
    //4	MEDIA_RULE	CSSOM
    //5	FONT_FACE_RULE	CSSOM
    //6	PAGE_RULE	CSSOM
    //7	KEYFRAMES_RULE	css3-animations
    //8	KEYFRAME_RULE	css3-animations
    //9	MARGIN_RULE	CSSOM
    //10	NAMESPACE_RULE	CSSOM
    //11	COUNTER_STYLE_RULE	css3-lists
    //12	SUPPORTS_RULE	css3-conditional
    //13	DOCUMENT_RULE	css3-conditional
    //14	FONT_FEATURE_VALUES_RULE	css3-fonts
    //15	VIEWPORT_RULE	css-device-adapt
    //16	REGION_STYLE_RULE	proposed for css3-regions
    //17	CUSTOM_MEDIA_RULE	mediaqueries
    //… 999	-	reserved for future standardization

    public enum enumCSSRuleType
    {

        STYLE_RULE = 1,
        CHARSET_RULE = 2,
        IMPORT_RULE = 3,
        MEDIA_RULE = 4,
        FONT_FACE_RULE = 5,
        PAGE_RULE = 6,
        KEYFRAMES_RULE = 7,
        KEYFRAME_RULE = 8,
        MARGIN_RULE = 9,
        NAMESPACE_RULE = 10,
        COUNTER_STYLE_RULE = 11,
        SUPPORTS_RULE = 12,
        DOCUMENT_RULE = 13,
        FONT_FEATURE_VALUES_RULE = 14,
        VIEWPORT_RULE = 15,
        REGION_STYLE_RULE = 16,
        CUSTOM_MEDIA_RULE = 17,
        reserved = 0,
        //… 999	- reserved for future standardization

    }
}
